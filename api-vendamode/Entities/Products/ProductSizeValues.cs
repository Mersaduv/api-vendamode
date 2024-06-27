using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using api_vendace.Models;
using api_vendamode.Entities.Products;

namespace api_vendace.Entities.Products;

public class ProductSizeValues : BaseClass<Guid>
{
    public string Name { get; set; } = string.Empty;
    public virtual List<ProductSizeProductSizeValue>? ProductSizeProductSizeValues { get; set; }
}