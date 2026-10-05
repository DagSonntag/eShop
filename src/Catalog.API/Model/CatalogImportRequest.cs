using System.ComponentModel.DataAnnotations;

namespace eShop.Catalog.API.Model;

public class CatalogImportRequest
{
    /// <summary>
    /// HTTP(S) URL of a JSON feed describing catalog items to import.
    /// The feed is expected to be a JSON array of <see cref="CatalogImportItem"/>.
    /// </summary>
    [Required]
    public string SourceUrl { get; set; } = default!;

    /// <summary>
    /// When true, picture URLs referenced by feed items are downloaded into
    /// the catalog's Pics directory. Defaults to false.
    /// </summary>
    public bool DownloadPictures { get; set; }
}

public class CatalogImportItem
{
    public int? Id { get; set; }

    [Required]
    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int CatalogTypeId { get; set; }

    public int CatalogBrandId { get; set; }

    public int AvailableStock { get; set; }

    public int RestockThreshold { get; set; }

    public int MaxStockThreshold { get; set; }

    /// <summary>
    /// Optional file name to store the image as inside the catalog's Pics directory.
    /// </summary>
    public string? PictureFileName { get; set; }

    /// <summary>
    /// Optional URL to download the picture from when <see cref="CatalogImportRequest.DownloadPictures"/> is true.
    /// </summary>
    public string? PictureUrl { get; set; }
}

public class CatalogImportResult
{
    public int Created { get; init; }

    public int Updated { get; init; }

    public int PicturesDownloaded { get; init; }
}
