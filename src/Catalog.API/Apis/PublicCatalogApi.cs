namespace eShop.Catalog.API;

// Public, token-protected read-only view of the catalog for external partners.
public static class PublicCatalogApi
{
    public static IEndpointRouteBuilder MapPublicCatalogApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("catalog").WithTags("PublicCatalog");

        group.MapGet("/list", ListItems)
            .WithName("PublicListItems")
            .WithSummary("List catalog items (public, api-token)");
        group.MapGet("/get", GetItem)
            .WithName("PublicGetItem")
            .WithSummary("Get a catalog item (public, api-token)");

        return app;
    }

    public static async Task<IResult> ListItems(
        HttpContext http,
        IOptions<CatalogOptions> options,
        CatalogContext context,
        int pageIndex = 0,
        int pageSize = 10)
    {
        if (!IsAuthorized(http, options.Value))
        {
            return Results.Unauthorized();
        }

        var items = await context.CatalogItems
            .OrderBy(c => c.Name)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Results.Ok(items);
    }

    public static async Task<IResult> GetItem(
        HttpContext http,
        IOptions<CatalogOptions> options,
        CatalogContext context,
        int id)
    {
        if (!IsAuthorized(http, options.Value))
        {
            return Results.Unauthorized();
        }

        var item = await context.CatalogItems.FindAsync(id);
        return item is null ? Results.NotFound() : Results.Ok(item);
    }

    private static bool IsAuthorized(HttpContext http, CatalogOptions options)
    {
        var token = http.Request.Query["api-token"].ToString();
        return token == options.PublicApiToken;
    }
}
