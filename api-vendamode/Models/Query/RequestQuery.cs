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
    [FromQuery(Name = "productIds")]
    public string[]? ProductIds { get; set; }

    [FromQuery(Name = "category")]
    public string? Category { get; set; }
    [FromQuery(Name = "categorySlug")]
    public string? CategorySlug { get; set; }
    [FromQuery(Name = "categoryId")]
    public string? CategoryId { get; set; }
    [FromQuery(Name = "singleCategory")]
    public bool? SingleCategory { get; set; }
    [FromQuery(Name = "sizes")]
    public Guid[]? SizeIds { get; set; }

    [FromQuery(Name = "featureIds")]
    public Guid[]? FeatureIds { get; set; }

    [FromQuery(Name = "featureValueIds")]
    public Guid[]? FeatureValueIds { get; set; }
    [FromQuery(Name = "brands")]
    public string[]? Brands { get; set; }

    [FromQuery(Name = "search")]
    public string? Search { get; set; }

    [FromQuery(Name = "bestSelling")]
    public string? IsBestSeller { get; set; }

    [FromQuery(Name = "inStock")]
    public string? InStock { get; set; }

    [FromQuery(Name = "discount")]
    public bool? Discount { get; set; }

    [FromQuery(Name = "place")]
    public string? Place { get; set; }

    [FromQuery(Name = "isAdmin")]
    public bool? Admin { get; set; }
    [FromQuery(Name = "isActive")]
    public bool? IsActive { get; set; }

    [FromQuery(Name = "inActive")]
    public bool? InActive { get; set; }

    [FromQuery(Name = "isDeleted")]
    public bool? IsDeleted { get; set; }
    [FromQuery(Name = "adminList")]
    public bool? AdminList { get; set; }

    [FromQuery(Name = "isActiveSlider")]
    public bool? IsSlider { get; set; }
}
