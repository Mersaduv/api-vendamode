using Microsoft.AspNetCore.Mvc;

namespace api_vendace.Models.Query;
public class RequestQuery
{
    [FromQuery(Name = "page")]
    public int? PageNumber { get; set; } = 1;

    [FromQuery(Name = "pagesize")]
    public int? PageSize { get; set; } = 15;

    [FromQuery(Name = "sortBy")]
    public string? SortBy { get; set; }

    [FromQuery(Name = "sort")]
    public string? Sort { get; set; }

    [FromQuery(Name = "minPrice")]
    public double? MinPrice { get; set; }

    [FromQuery(Name = "maxPrice")]
    public double? MaxPrice { get; set; }

    [FromQuery(Name = "category")]
    public string? Category { get; set; }
    [FromQuery(Name = "categoryId")]
    public string? CategoryId { get; set; }

    [FromQuery(Name = "featureIds")]
    public Guid[]? FeatureIds { get; set; }

    [FromQuery(Name = "featureValueIds")]
    public Guid[]? FeatureValueIds { get; set; }

    [FromQuery(Name = "search")]
    public string? Search { get; set; }

    [FromQuery(Name = "inStock")]
    public string? InStock { get; set; }

    [FromQuery(Name = "discount")]
    public bool? Discount { get; set; }
}
