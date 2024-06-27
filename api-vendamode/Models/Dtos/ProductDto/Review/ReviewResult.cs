using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto.Review;

namespace api_vendamode.Models.Dtos.ProductDto.Review;

public class ReviewResult
{
    public int ReviewsLength { get; set; }
    public Pagination<ReviewDto> Pagination { get; set; } = default!;
}