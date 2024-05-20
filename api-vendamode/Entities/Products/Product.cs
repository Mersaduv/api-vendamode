using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api_vendamode.Enums;
using api_vendamode.Models;
using api_vendamode.Models.Dtos;

namespace api_vendamode.Entities.Products;

public class Product : BaseClass<Guid>
{
    public string Title { get; set; } = string.Empty;
    public List<EntityImage<Guid, Product>> Images { get; set; } = [];
    public string Code { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public double Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public double? Discount { get; set; }
    public int CategoryId { get; set; }
    public CategoryLevels? CategoryLevels { get; set; }
    public virtual Category? Category { get; set; }
    public int? BrandId { get; set; }
    public virtual Brand? Brand { get; set; }
    public List<ProductColor>? Colors { get; set; }
    public List<string>? Size { get; set; }
    public ICollection<ProductInfo>? Info { get; set; } = new List<ProductInfo>();
    public ICollection<ProductSpecification>? Specifications { get; set; } = new List<ProductSpecification>();
    public int InStock { get; set; }
    public int? Sold { get; set; }
    public double? Rating { get; set; }
    public int? NumReviews { get; set; }
    public virtual List<Review>? Review { get; set; }
    public OptionType OptionType { get; set; } = OptionType.None;
}
