using api_vendace.Entities.Products;

namespace api_vendace.Models.Dtos.ProductDto;

public class ProductFeatureInfo
{
    public List<ProductSizeInfo>? ProductSizeInfo { get; set; }
    public List<ColorDTO>? ColorDTOs { get; set; }
    public List<ObjectValue>? FeatureValueInfos { get; set; }

    public ProductFeatureInfo(Product product)
    {
        MapProductFeatures(product);
    }

    private void MapProductFeatures(Product product)
    {
        ColorDTOs = new List<ColorDTO>();
        FeatureValueInfos = new List<ObjectValue>();

        if (product.ProductFeatures != null)
        {
            foreach (var feature in product.ProductFeatures)
            {
                if (feature.Values != null)
                {
                    foreach (var value in feature.Values)
                    {
                        if (!string.IsNullOrEmpty(value.HexCode))
                        {
                            ColorDTOs.Add(new ColorDTO { Id = value.Id, HexCode = value.HexCode, Name = value.Name });
                        }
                        else
                        {
                            FeatureValueInfos.Add(new ObjectValue { Id = feature.Id, Title = feature.Name, Value = value.Name });
                        }
                    }
                }
            }
        }
    }
}
