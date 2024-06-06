using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto.Review;
using api_vendace.Models.Query;

namespace api_vendace.Interfaces.IServices;

public interface IReviewServices
{
    Task<ServiceResponse<bool>> CreateReview(ReviewCreateDTO reviewCreate);
    Task<ServiceResponse<bool>> DeleteReview(Guid id);
    Task<ServiceResponse<Pagination<ReviewDto>>> GetProductReviews(Guid id, RequestQuery requestQuer);
    Task<ServiceResponse<List<ReviewDto>>> GetReviews();
    Task<ServiceResponse<ReviewDto>> GetReviewBy(Guid id);
}