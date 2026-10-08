var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationServices();
builder.AddDefaultAuthentication();
builder.Services.AddProblemDetails();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = (context, _) =>
    {
        context.HttpContext.Response.Headers.RetryAfter = "60";
        return ValueTask.CompletedTask;
    };
    options.AddPolicy("CatalogImport", context =>
    {
        var partitionKey = context.User.FindFirst("sub")?.Value
            ?? context.User.Identity?.Name
            ?? context.Connection.RemoteIpAddress?.ToString()
            ?? "anonymous";
        return RateLimitPartition.GetTokenBucketLimiter(partitionKey, _ => new TokenBucketRateLimiterOptions
        {
            TokenLimit = 5,
            TokensPerPeriod = 5,
            ReplenishmentPeriod = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            AutoReplenishment = true
        });
    });
});

var withApiVersioning = builder.Services.AddApiVersioning(options =>
{
    // Include "api-supported-versions" and "api-deprecated-versions" headers in all responses
    options.ReportApiVersions = true;
});

builder.AddDefaultOpenApi(withApiVersioning);

var app = builder.Build();

// Convert any unhandled exception into a generic ProblemDetails response (no stack
// trace or internal detail) in all environments; full detail is logged server-side.
app.UseExceptionHandler();

// Throttle the expensive bulk-import endpoint (see RequireRateLimiting on the route).
app.UseRateLimiter();

app.MapDefaultEndpoints();

app.UseStatusCodePages();

app.MapCatalogApi();

app.UseDefaultOpenApi();
app.Run();
