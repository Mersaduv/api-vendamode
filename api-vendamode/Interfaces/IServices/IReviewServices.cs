using api_vendamode.Models;
using api_vendamode.Models.Dtos.ProductDto.Review;
using api_vendamode.Models.Query;

namespace api_vendamode.Interfaces.IServices;

public interface IReviewServices
{
    Task<ServiceResponse<bool>> CreateReview(ReviewCreateDTO reviewCreate);
    Task<ServiceResponse<bool>> DeleteReview(Guid id);
    Task<ServiceResponse<Pagination<ReviewDto>>> GetProductReviews(Guid id, RequestQuery requestQuer);
    Task<ServiceResponse<List<ReviewDto>>> GetReviews();
    Task<ServiceResponse<ReviewDto>> GetReviewBy(Guid id);
}