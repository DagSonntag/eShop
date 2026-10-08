using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eShop.Catalog.API.Model;

public record PaginationRequest(
    [property: Description("Number of items to return in a single page of results")]
    [property: DefaultValue(10)]
    [property: Range(1, 100)]
    int PageSize = 10,

    [property: Description("The index of the page of results to return")]
    [property: DefaultValue(0)]
    int PageIndex = 0
);
