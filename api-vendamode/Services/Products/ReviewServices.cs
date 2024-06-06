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
                Message = "در مقداردهی رضایت از محصول مشکی پیش آمده."
            };
        }

        var review = new Review
        {
            UserId = userId,
            ProductId = reviewCreate.ProductId,
            Comment = reviewCreate.Comment,
            Rating = reviewCreate.Rating,
            Images = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Review>>(reviewCreate.ProductThumbnails, nameof(reviewCreate), false),
            NegativePoints = reviewCreate.NegativePoints?.Select(p => new Points
            {
                Id = Guid.NewGuid(),
                Title = p.Title
            }).ToList(),
            PositivePoints = reviewCreate.PositivePoints?.Select(p => new Points
            {
                Id = Guid.NewGuid(),
                Title = p.Title
            }).ToList(),
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        await _context.Reviews.AddAsync(review);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
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

    public async Task<ServiceResponse<Pagination<ReviewDto>>> GetProductReviews(Guid id, RequestQuery requestQuery)
    {
        var pageNumber = requestQuery.PageNumber ?? 1;
        var pageSize = requestQuery.PageSize;
        var skipCount = (pageNumber - 1) * pageSize;
        var reviews = await _context.Reviews
            .Where(r => r.ProductId == id)
            .OrderByDescending(r => r.Created)
            .Skip(skipCount)
            .Take(pageSize)
            .Include(u => u.User)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                Comment = r.Comment,
                NegativePoints = r.NegativePoints,
                PositivePoints = r.PositivePoints,
                Rating = r.Rating,
                UserName = r.User.UserSpecification.FirstName + " " + r.User.UserSpecification.FamilyName,
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

        return new ServiceResponse<Pagination<ReviewDto>>
        {
            Data = pagination
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
}