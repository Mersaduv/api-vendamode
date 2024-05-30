using Microsoft.AspNetCore.Mvc;

namespace api_vendamode.Models.Query;

public class RequestQuery
{
    [FromQuery(Name = "page")]
    public int? PageNumber { get; set; }
    [FromQuery(Name = "pagesize")]
    public int PageSize { get; set; }
    public string? SortBy { get; set; }
    public string? Sort { get; set; }
    [FromQuery(Name = "minPrice")]
    public double? MinPrice { get; set; }

    [FromQuery(Name = "maxPrice")]
    public double? MaxPrice { get; set; }
}