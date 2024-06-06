namespace api_vendace.Models.Dtos.ProductDto;

public class GetProductsResult
{
    public int ProductsLength { get; set; }
    public double MainMaxPrice { get; set; }
    public double MainMinPrice { get; set; }
    public Pagination<ProductDTO>? Pagination { get; set; }
}

