namespace eShop.Catalog.API;

public class CatalogOptions
{
    public string? PicBaseUrl { get; set; }
    public bool UseCustomizationData { get; set; }

    // Maximum number of bytes a single imported picture may occupy. Downloads
    // that exceed this cap are aborted and any partial file removed.
    public long MaxPictureDownloadBytes { get; set; } = 10 * 1024 * 1024; // 10 MB
}
