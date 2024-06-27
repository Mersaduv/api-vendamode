using api_vendace.Entities.Products;

namespace api_vendace.Models.Dtos.ProductDto;

public class ProductFeatureInfo
{
    public List<ProductSizeInfo>? ProductSizeInfo { get; set; }
    public List<ColorDTO>? ColorDTOs { get; set; }
    public List<ObjectValue>? FeatureValueInfos { get; set; }

    public ProductFeatureInfo(List<ProductFeature> productFeatures)
    {
        MapProductFeatures(productFeatures);
    }

    private void MapProductFeatures(List<ProductFeature> productFeatures)
    {
        ColorDTOs = new List<ColorDTO>();
        FeatureValueInfos = new List<ObjectValue>();

        if (productFeatures != null)
        {
            foreach (var feature in productFeatures)
            {
                var featureValue = new ObjectValue
                {
                    Id = feature.Id,
                    Title = feature.Name
                };
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
                            featureValue.Value.Add(new Value { Id = value.Id, Name = value.Name });

                        }
                    }
                    if (featureValue.Value.Count > 0)
                    {
                        FeatureValueInfos.Add(featureValue);
                    }
                }
            }
        }
    }
}
