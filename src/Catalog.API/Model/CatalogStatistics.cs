namespace eShop.Catalog.API.Model;

public class CatalogStatistics
{
    public long TotalItems { get; init; }

    public long TotalBrands { get; init; }

    public long TotalTypes { get; init; }

    public long ItemsOutOfStock { get; init; }

    public long ItemsOnReorder { get; init; }

    public long TotalAvailableStock { get; init; }

    public decimal AveragePrice { get; init; }

    public decimal MinPrice { get; init; }

    public decimal MaxPrice { get; init; }
}
