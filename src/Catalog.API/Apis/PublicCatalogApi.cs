using System.Security.Cryptography;
using System.Text;

namespace eShop.Catalog.API;

// Public, token-protected read-only view of the catalog for external partners.
public static class PublicCatalogApi
{
    private const string BearerPrefix = "Bearer ";
    private const int MinTokenLength = 32;
    private const int MinDistinctTokenChars = 10;

    public static IEndpointRouteBuilder MapPublicCatalogApi(this IEndpointRouteBuilder app)
    {
        // The public API is opt-in: without a configured token the endpoints are not exposed at all.
        var options = app.ServiceProvider.GetRequiredService<IOptions<CatalogOptions>>().Value;
        if (options.PublicApiToken is null)
        {
            return app;
        }

        var group = app.MapGroup("catalog")
            .WithTags("PublicCatalog")
            .AddEndpointFilter(RequireApiToken);

        group.MapGet("/list", ListItems)
            .WithName("PublicListItems")
            .WithSummary("List catalog items (public, Bearer api-token)");
        group.MapGet("/get", GetItem)
            .WithName("PublicGetItem")
            .WithSummary("Get a catalog item (public, Bearer api-token)");

        return app;
    }

    public static async Task<IResult> ListItems(
        CatalogContext context,
        int pageIndex = 0,
        int pageSize = 10)
    {
        var items = await context.CatalogItems
            .OrderBy(c => c.Name)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Results.Ok(items);
    }

    public static async Task<IResult> GetItem(
        CatalogContext context,
        int id)
    {
        var item = await context.CatalogItems.FindAsync(id);
        return item is null ? Results.NotFound() : Results.Ok(item);
    }

    // Validated at startup (see Extensions.AddApplicationServices): a configured token must be long and non-trivial.
    public static bool IsStrongToken(string token) =>
        token.Length >= MinTokenLength && token.Distinct().Count() >= MinDistinctTokenChars;

    // Only the Authorization: Bearer header is accepted; any api-token query parameter is ignored.
    private static async ValueTask<object?> RequireApiToken(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
    {
        var http = ctx.HttpContext;
        var expected = http.RequestServices.GetRequiredService<IOptions<CatalogOptions>>().Value.PublicApiToken;
        var header = http.Request.Headers.Authorization.ToString();

        if (string.IsNullOrEmpty(expected) || !header.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return Results.Unauthorized();
        }

        var presented = header[BearerPrefix.Length..].Trim();
        if (presented.Length == 0 || !FixedTimeTokenEquals(presented, expected))
        {
            return Results.Unauthorized();
        }

        return await next(ctx);
    }

    // Hash both sides so the comparison is constant-time and does not leak the expected token's length.
    private static bool FixedTimeTokenEquals(string presented, string expected) =>
        CryptographicOperations.FixedTimeEquals(
            SHA256.HashData(Encoding.UTF8.GetBytes(presented)),
            SHA256.HashData(Encoding.UTF8.GetBytes(expected)));
}
