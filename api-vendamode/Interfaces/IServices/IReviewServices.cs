using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto.Review;
using api_vendace.Models.Query;
using api_vendamode.Models.Dtos.ProductDto.Review;
using api_vendamode.Models.Dtos.ProductDto.Review.Article;

namespace api_vendace.Interfaces.IServices;

public interface IReviewServices
{
    Task<ServiceResponse<bool>> CreateReview(ReviewCreateDTO reviewCreate);
    Task<ServiceResponse<bool>> DeleteReview(Guid id);
    Task<ServiceResponse<ReviewResult>> GetProductReviews(Guid id, RequestQuery requestQuery);
    Task<ServiceResponse<List<ReviewDto>>> GetReviews();
    Task<ServiceResponse<ReviewDto>> GetReviewBy(Guid id);

    Task<ServiceResponse<bool>> CreateArticleReview(ArticleReviewCreateDTO articleReviewCreate);
    Task<ServiceResponse<bool>> DeleteArticleReview(Guid id);
    Task<ServiceResponse<Pagination<ArticleReviewDto>>> GetArticleReviews(Guid articleId, RequestQuery requestQuery);
    Task<ServiceResponse<Pagination<ArticleReviewDto>>> GetAllArticleReviews(RequestQuery requestQuery);
    Task<ServiceResponse<ArticleReviewDto>> GetArticleReviewBy(Guid id);
}