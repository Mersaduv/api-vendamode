using api_vendace.Models;
using api_vendamode.Entities.Products;
using api_vendamode.Models.Dtos.ProductDto;
namespace api_vendamode.Interfaces.IServices;
public interface ISliderServices
{
    Task<ServiceResponse<bool>> AddSlider(SliderCreateDto slider);
    Task<ServiceResponse<bool>> UpdateSlider(SliderUpdateDto slider);
    Task<ServiceResponse<bool>> DeleteSlider(Guid id);
    Task<ServiceResponse<SliderDto>> GetSliderBy(Guid id);
    Task<ServiceResponse<IReadOnlyList<SliderDto>>> GetSliders();
    Task<ServiceResponse<IReadOnlyList<SliderDto>>> GetMainSliders();

}
