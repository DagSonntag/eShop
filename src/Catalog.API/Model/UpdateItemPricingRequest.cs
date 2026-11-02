namespace eShop.Catalog.API.Model;

/// <summary>
/// Request body for adjusting the price and stock levels of a catalog item.
/// </summary>
public record UpdateItemPricingRequest(
    decimal Price,
    int AvailableStock,
    int RestockThreshold,
    int MaxStockThreshold,
    string? Reason);
