using Microsoft.AspNetCore.Mvc;

namespace api_vendamode.Models.Query;

public class RequestSubCategory
{
    [FromQuery(Name = "id")]
    public Guid? Id { get; set; }
    [FromQuery(Name = "slug")]
    public string? Slug { get; set; }
}
