using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using api_vendace.Models;

namespace api_vendace.Entities.Products;

public class ProductSizeValues : BaseClass<Guid>
{
    public string Name { get; set; } = string.Empty;
    public Guid ProductSizeId { get; set; }
    public ProductSize? ProductSize { get; set; }
}