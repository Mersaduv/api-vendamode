using api_vendace.Data;
using api_vendace.Entities;
using api_vendace.Entities.Products;
using api_vendace.Interfaces;
using api_vendace.Interfaces.IServices;
using api_vendace.Models;
using api_vendace.Models.Dtos;
using api_vendace.Models.Dtos.ProductDto.Review;
using api_vendace.Models.Query;
using api_vendace.Utility;
using api_vendamode.Entities;
using api_vendamode.Models.Dtos.ProductDto.Review;
using api_vendamode.Models.Dtos.ProductDto.Review.Article;
using Microsoft.EntityFrameworkCore;

namespace api_vendace.Services.Products;

public class ReviewServices : IReviewServices
{
    private readonly ApplicationDbContext _context;
    private readonly IUserServices _userServices;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ByteFileUtility _byteFileUtility;

    public ReviewServices(ApplicationDbContext context, IUnitOfWork unitOfWork, ByteFileUtility byteFileUtility, IUserServices userServices)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _byteFileUtility = byteFileUtility;
        _userServices = userServices;
    }

    public async Task<ServiceResponse<bool>> CreateReview(ReviewCreateDTO reviewCreate)
    {
        var isExist = await CheckProductExist(reviewCreate.ProductId);
        var userId = _userServices.GetUserId();
        if (!isExist)
        {

            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "محصولی در این دیدگاه پیدا نشد."
            };
        }

        if (reviewCreate.Rating < 0 || reviewCreate.Rating > 5)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "در مقداردهی رضایت از محصول مشکلی پیش آمده."
            };
        }

        var review = new Review
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductId = reviewCreate.ProductId,
            Comment = reviewCreate.Comment,
            Rating = reviewCreate.Rating,
            Status = 1,
            Images = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Review>>(reviewCreate.ProductThumbnails, nameof(reviewCreate), false),
            NegativePoints = reviewCreate.NegativePoints?.Select(p => new Points
            {
                Id = p.Id,
                Title = p.Title
            }).ToList(),
            PositivePoints = reviewCreate.PositivePoints?.Select(p => new Points
            {
                Id = p.Id,
                Title = p.Title
            }).ToList(),
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        await _context.Reviews.AddAsync(review);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true,
            Message = "دیدگاه شما بعد از تایید ناظر منتشر خواهد شد"
        };
    }

    public async Task<ServiceResponse<bool>> DeleteReview(Guid id)
    {
        var review = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == id);
        if (review == null)
        {
            return new ServiceResponse<bool>
            {
                Message = "دیدگاهی پیدا نشد",
                Success = false
            };
        }
        _context.Reviews.Remove(review);
        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<ReviewResult>> GetProductReviews(Guid id, RequestQuery requestQuery)
    {
        var pageNumber = requestQuery.PageNumber ?? 1;
        var pageSize = requestQuery.PageSize ?? 15;
        var skipCount = (pageNumber - 1) * pageSize;
        var reviews = await _context.Reviews
            .Where(r => r.ProductId == id)
            .OrderByDescending(r => r.Created)
            .Skip(skipCount)
            .Take(pageSize)
            .Include(u => u.User)
            .ThenInclude(us => us.UserSpecification)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                Comment = r.Comment,
                NegativePoints = r.NegativePoints,
                PositivePoints = r.PositivePoints,
                Rating = r.Rating,
                Status = r.Status,
                UserName = r.User.UserSpecification.FirstName,
                ImageUrls = _byteFileUtility.GetEncryptedFileActionUrl
                            (r.Images.Select(img => new EntityImageDto
                            {
                                Id = img.Id,
                                ImageUrl = img.ImageUrl!,
                                Placeholder = img.Placeholder!
                            }).ToList(), nameof(Review)),
                UserId = r.User.Id,
                Created = r.Created,
                LastUpdated = r.LastUpdated
            })
            .ToListAsync();

        var totalReviews = await _context.Reviews.CountAsync(r => r.ProductId == id);

        var pagination = new Pagination<ReviewDto>
        {
            CurrentPage = pageNumber,
            NextPage = pageNumber + 1,
            PreviousPage = pageNumber > 1 ? pageNumber - 1 : 0,
            HasNextPage = (skipCount + pageSize) < totalReviews,
            HasPreviousPage = pageNumber > 1,
            LastPage = (int)Math.Ceiling((decimal)totalReviews / pageSize),
            TotalCount = totalReviews,
            Data = reviews
        };

        var result = new ReviewResult
        {
            ReviewsLength = totalReviews,
            Pagination = pagination
        };

        return new ServiceResponse<ReviewResult>
        {
            Data = result
        };
    }

    public Task<ServiceResponse<ReviewDto>> GetReviewBy(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<List<ReviewDto>>> GetReviews()
    {
        throw new NotImplementedException();
    }

    private async Task<bool> CheckProductExist(Guid productId)
    {
        var product = await _context.Products
           .AsNoTracking().FirstOrDefaultAsync(i => i.Id == productId);
        if (product is null)
        {
            return false;
        }
        return true;
    }

    public async Task<ServiceResponse<bool>> CreateArticleReview(ArticleReviewCreateDTO articleReviewCreate)
    {
        var isExist = await CheckArticleExist(articleReviewCreate.ArticleId);
        var userId = _userServices.GetUserId();
        if (!isExist)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "مقاله‌ای برای این دیدگاه پیدا نشد."
            };
        }

        var review = new ArticleReview
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ArticleId = articleReviewCreate.ArticleId,
            Status = 1,
            Comment = articleReviewCreate.Comment,
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        await _context.ArticleReviews.AddAsync(review);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true,
            Message = "دیدگاه شما بعد از تایید ناظر منتشر خواهد شد"
        };
    }

    public async Task<ServiceResponse<bool>> DeleteArticleReview(Guid id)
    {
        var review = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == id);
        if (review == null)
        {
            return new ServiceResponse<bool>
            {
                Message = "دیدگاهی پیدا نشد",
                Success = false
            };
        }
        _context.Reviews.Remove(review);
        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<Pagination<ArticleReviewDto>>> GetArticleReviews(Guid articleId, RequestQuery requestQuery)
    {
        var pageNumber = requestQuery.PageNumber ?? 1;
        var pageSize = requestQuery.PageSize ?? 15;
        var skipCount = (pageNumber - 1) * pageSize;

        var reviews = await _context.ArticleReviews
            .Where(r => r.ArticleId == articleId)
            .OrderByDescending(r => r.Created)
            .Skip(skipCount)
            .Take(pageSize)
            .Include(u => u.User)
            .ThenInclude(us => us.UserSpecification)
            .Select(r => new ArticleReviewDto
            {
                Id = r.Id,
                Comment = r.Comment,
                Status = r.Status,
                UserName = r.User.UserSpecification.FirstName + " " + r.User.UserSpecification.FamilyName,
                UserId = r.User.Id,
                Created = r.Created,
                LastUpdated = r.LastUpdated
            })
            .ToListAsync();

        var totalReviews = await _context.Reviews.CountAsync(r => r.ProductId == articleId);

        var pagination = new Pagination<ArticleReviewDto>
        {
            CurrentPage = pageNumber,
            NextPage = pageNumber + 1,
            PreviousPage = pageNumber > 1 ? pageNumber - 1 : 0,
            HasNextPage = (skipCount + pageSize) < totalReviews,
            HasPreviousPage = pageNumber > 1,
            LastPage = (int)Math.Ceiling((decimal)totalReviews / pageSize),
            TotalCount = totalReviews,
            Data = reviews
        };

        return new ServiceResponse<Pagination<ArticleReviewDto>>
        {
            Data = pagination
        };
    }

    public async Task<ServiceResponse<ArticleReviewDto>> GetArticleReviewBy(Guid id)
    {
        var review = await _context.Reviews
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (review == null)
        {
            return new ServiceResponse<ArticleReviewDto>
            {
                Success = false,
                Message = "دیدگاهی پیدا نشد"
            };
        }

        var reviewDto = new ArticleReviewDto
        {
            Id = review.Id,
            Comment = review.Comment,
            UserName = review.User.UserSpecification.FirstName + " " + review.User.UserSpecification.FamilyName,
            UserId = review.User.Id,
            Created = review.Created,
            LastUpdated = review.LastUpdated
        };

        return new ServiceResponse<ArticleReviewDto>
        {
            Data = reviewDto
        };
    }

    private async Task<bool> CheckArticleExist(Guid articleId)
    {
        var article = await _context.Articles
           .AsNoTracking().FirstOrDefaultAsync(i => i.Id == articleId);
        if (article is null)
        {
            return false;
        }
        return true;
    }

    public async Task<ServiceResponse<Pagination<ArticleReviewDto>>> GetAllArticleReviews(RequestQuery requestQuery)
    {
        var pageNumber = requestQuery.PageNumber ?? 1;
        var pageSize = requestQuery.PageSize ?? 15;
        var skipCount = (pageNumber - 1) * pageSize;

        var reviews = await _context.ArticleReviews
            .OrderByDescending(r => r.Created)
            .Skip(skipCount)
            .Take(pageSize)
            .Include(u => u.User)
            .ThenInclude(us => us.UserSpecification)
            .Select(r => new ArticleReviewDto
            {
                Id = r.Id,
                Comment = r.Comment,
                Status = r.Status,
                UserName = r.User.UserSpecification.FirstName + " " + r.User.UserSpecification.FamilyName,
                UserId = r.User.Id,
                Created = r.Created,
                LastUpdated = r.LastUpdated
            })
            .ToListAsync();

        var totalReviews = await _context.Reviews.CountAsync();

        var pagination = new Pagination<ArticleReviewDto>
        {
            CurrentPage = pageNumber,
            NextPage = pageNumber + 1,
            PreviousPage = pageNumber > 1 ? pageNumber - 1 : 0,
            HasNextPage = (skipCount + pageSize) < totalReviews,
            HasPreviousPage = pageNumber > 1,
            LastPage = (int)Math.Ceiling((decimal)totalReviews / pageSize),
            TotalCount = totalReviews,
            Data = reviews
        };

        return new ServiceResponse<Pagination<ArticleReviewDto>>
        {
            Data = pagination
        };
    }
}