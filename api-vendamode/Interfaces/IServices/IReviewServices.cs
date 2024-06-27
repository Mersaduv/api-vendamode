using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto.Review;
using api_vendace.Models.Query;
using api_vendamode.Models.Dtos.ProductDto.Review;

namespace api_vendace.Interfaces.IServices;

public interface IReviewServices
{
    Task<ServiceResponse<bool>> CreateReview(ReviewCreateDTO reviewCreate);
    Task<ServiceResponse<bool>> DeleteReview(Guid id);
    Task<ServiceResponse<ReviewResult>> GetProductReviews(Guid id, RequestQuery requestQuery);
    Task<ServiceResponse<List<ReviewDto>>> GetReviews();
    Task<ServiceResponse<ReviewDto>> GetReviewBy(Guid id);
}