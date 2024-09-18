using System.Text.RegularExpressions;
using api_vendace.Data;
using api_vendace.Entities;
using api_vendace.Interfaces;
using api_vendace.Interfaces.IServices;
using api_vendace.Models;
using api_vendace.Models.Dtos;
using api_vendace.Models.Dtos.ProductDto.Category;
using api_vendace.Models.Query;
using api_vendace.Utility;
using api_vendamode.Entities.Products;
using api_vendamode.Interfaces.IServices;
using api_vendamode.Models.Dtos.ArticleDto;
using api_vendamode.Models.Dtos.ProductDto.Category;
using Microsoft.EntityFrameworkCore;

namespace api_vendamode.Services.Design;

public class ArticleServices : IArticleServices
{
    private readonly ApplicationDbContext _context;
    private readonly ByteFileUtility _byteFileUtility;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserServices _userServices;
    private readonly ICategoryServices _categoryServices;

    public ArticleServices(ByteFileUtility byteFileUtility, ApplicationDbContext context, IUnitOfWork unitOfWork, IUserServices userServices, ICategoryServices categoryServices)
    {
        _byteFileUtility = byteFileUtility;
        _context = context;
        _unitOfWork = unitOfWork;
        _userServices = userServices;
        _categoryServices = categoryServices;
    }
    private async Task<string> GenerateNextArticleCodeAsync()
    {
        var lastArticle = await _context.Articles.OrderByDescending(a => a.Code).FirstOrDefaultAsync();
        var lastCode = lastArticle?.Code ?? "M10000";
        var nextCodeNumber = int.Parse(lastCode.Substring(1)) + 1;
        return $"M{nextCodeNumber:D5}";
    }
    private static string GenerateSlug(string title, string code)
    {
        // Convert to lowercase
        string slug = title.ToLowerInvariant();

        // Replace spaces with hyphens
        slug = Regex.Replace(slug, @"\s+", "-");

        // Remove invalid characters
        slug = Regex.Replace(slug, @"[^a-z0-9\u0600-\u06FF-]", "");

        // Trim hyphens from the ends
        slug = slug.Trim('-');

        // Append the product code
        slug = $"{slug}-{code}";

        return slug;
    }
    public async Task<ServiceResponse<Guid>> UpsertArticle(ArticleUpsertDTO articleUpsert)
    {
        var articleDb = await _context.Articles.Include(x => x.Image).FirstOrDefaultAsync(x => x.Id == articleUpsert.Id);
        var userId = _userServices.GetUserId();
        var userInfo = await _context.Users.Include(us => us.UserSpecification).FirstOrDefaultAsync(u => u.Id == userId);

        if (articleDb is null)
        {
            var newArticleCode = await GenerateNextArticleCodeAsync();
            var newArticleSlug = GenerateSlug(articleUpsert.Title, newArticleCode);
            var newArticleId = Guid.NewGuid();
            // Create new banner
            var newArticle = new Article
            {
                Id = newArticleId,
                Image = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Article>>([articleUpsert.Thumbnail!], nameof(Article), newArticleCode, false).First(),
                IsActive = articleUpsert.IsActive,
                Description = articleUpsert.Description,
                Place = articleUpsert.Place,
                Title = articleUpsert.Title,
                Slug = newArticleSlug,
                CategoryId = articleUpsert.CategoryId,
                Code = newArticleCode,
                Author = userInfo?.UserSpecification.FirstName + " " + userInfo?.UserSpecification.FamilyName,
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            await _context.Articles.AddAsync(newArticle);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResponse<Guid>
            {
                Message = "مقاله با موفقیت ایجاد شد",
                Data = newArticleId
            };
        }
        else
        {
            articleDb.Title = articleUpsert.Title;
            articleDb.IsActive = articleUpsert.IsActive;
            articleDb.Place = articleUpsert.Place;
            articleDb.Author = userInfo?.UserSpecification.FirstName + " " + userInfo?.UserSpecification.FamilyName;
            articleDb.Description = articleUpsert.Description;
            articleDb.CategoryId = articleUpsert.CategoryId;
            articleDb.LastUpdated = DateTime.UtcNow;


            if (articleUpsert.Thumbnail != null)
            {
                if (articleDb.Image is not null)
                {
                    _byteFileUtility.DeleteFiles([articleDb.Image], nameof(Article), articleDb.Code);
                }
                articleDb.Image = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Article>>([articleUpsert.Thumbnail], nameof(Article), articleDb.Code, false).First();
            }


            _context.Articles.Update(articleDb);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResponse<Guid>
            {
                Message = "مقاله با موفقیت بروزرسانی شد",
                Data = articleDb.Id
            };
        }
    }

    public async Task<ServiceResponse<Pagination<ArticleDto>>> GetAllArticles(RequestQuery requestQuery)
    {
        var pageNumber = requestQuery.PageNumber ?? 1;
        var pageSize = requestQuery.PageSize ?? 15;

        var articlesQuery = _context.Articles.OrderByDescending(a => a.LastUpdated)
                    .Include(a => a.ArticleReviews)
                    .Include(c => c.Image)
                    .Include(a => a.Category)
                    .ThenInclude(c => c.ParentCategory)
                    .AsQueryable();

        // Admin LIst 
        if (requestQuery.AdminList is not null)
        {
            articlesQuery = articlesQuery.Where(x => !x.IsDeleted);
        }

        // Deleted Filter
        if (requestQuery.IsDeleted is not null)
        {
            articlesQuery = articlesQuery.Where(x => x.IsDeleted);
        }

        // isActive Filter
        if (requestQuery.IsActive is not null)
        {
            articlesQuery = articlesQuery.Where(x => x.IsActive);
        }

        // isActive Filter
        if (requestQuery.InActive is not null)
        {
            articlesQuery = articlesQuery.Where(x => !x.IsActive && !x.IsDeleted);
        }
        // Place filter
        if (requestQuery.Place is not null && requestQuery.Place == "1")
        {
            articlesQuery = articlesQuery.Where(a => a.CategoryId != null);
        }

        if (requestQuery.Place is not null && requestQuery.Place == "2")
        {
            articlesQuery = articlesQuery.Where(a => a.CategoryId == null);
        }

        // Search filter
        if (!string.IsNullOrEmpty(requestQuery.Search))
        {
            string searchLower = requestQuery.Search.ToLower();
            articlesQuery = articlesQuery.Where(p => p.Title.ToLower().Contains(searchLower));
        }

        // with Category
        if (requestQuery.IsCategory is not null)
        {
            articlesQuery = articlesQuery.Where(a => a.CategoryId != null);
        }

        // Sort
        if (!string.IsNullOrEmpty(requestQuery.Sort))
        {
            switch (requestQuery.Sort)
            {
                case "1": // Sort by latest
                    articlesQuery = articlesQuery.OrderByDescending(p => p.Created);
                    break;

                case "2": // Sort by best-selling
                    articlesQuery = articlesQuery.OrderBy(p => p.Created);
                    break;
                default:
                    articlesQuery = articlesQuery.OrderByDescending(p => p.Created);
                    break;
            }
        }


        // Category filter
        if (requestQuery.CategoryId is not null && requestQuery.SingleCategory is not null)
        {
            Guid categoryId;
            if (Guid.TryParse(requestQuery.CategoryId, out categoryId))
            {
                var allCategoryIds = _categoryServices.GetAllCategoryIds(categoryId);

                articlesQuery = articlesQuery.Where(p => allCategoryIds.Contains(p.CategoryId ?? Guid.Empty));
            }
        }

        if (!string.IsNullOrEmpty(requestQuery.Category) && requestQuery.SingleCategory is not null)
        {
            var allCategoryIds = _categoryServices.GetAllCategoryIdsBy(requestQuery.Category);

            articlesQuery = articlesQuery.Where(p => allCategoryIds.Contains(p.CategoryId ?? Guid.Empty));
        }

        // if (requestQuery.CategoryId is not null && requestQuery.SingleCategory is null)
        // {
        //     articlesQuery = articlesQuery.Where(p => p.CategoryId == Guid.Parse(requestQuery.CategoryId));
        // }

        if (requestQuery.CategoryId is not null && requestQuery.SingleCategory is null)
        {

            if (requestQuery.CategoryId != "default") articlesQuery = articlesQuery.Where(p => p.CategoryId == Guid.Parse(requestQuery.CategoryId));
        }

        var totalCount = await articlesQuery.CountAsync();

        var articlesQueryList = await articlesQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        var articleList = articlesQueryList.Select(article => new ArticleDto
        {
            Id = article.Id,
            Slug = article.Slug,
            Title = article.Title,
            CategoryId = article.CategoryId,
            Category = article.Category != null ? article.Category.Name : "",

            ParentCategories = article.Category != null ? new CategoryWithAllParents
            {
                Category = new CategoryDTO
                {
                    Id = article.Category.Id,
                    Name = article.Category.Name,
                    Slug = article.Category.Name,
                    Level = article.Category.Level,
                    ParentCategoryId = article.Category.ParentCategoryId,
                    Created = article.Category.Created,
                    LastUpdated = article.Category.LastUpdated
                },
                ParentCategories = article.Category.GetParentCategories(_context)?
                    .Select(category => new CategoryDTO
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Slug = category.Name,
                        Level = category.Level,
                        Created = category.Created,
                        LastUpdated = category.LastUpdated
                    }).Reverse().ToList() ?? new List<CategoryDTO>()
            } : null, // اگر Category null باشد، ParentCategories نیز null خواهد بود.

            Description = article.Description,
            IsActive = article.IsActive,
            Place = article.Place,
            Author = article.Author,
            NumReviews = article.ArticleReviews != null ? article.ArticleReviews.Count(x => x.Status == 2) : 0,
            Code = article.Code,
            IsDeleted = article.IsDeleted,
            Image = article.Image != null ? _byteFileUtility.GetEncryptedFileActionUrl(new List<EntityImageDto>
            {
                new EntityImageDto { Id = article.Image.Id , ImageUrl = article.Image.ImageUrl ?? string.Empty, Placeholder = article.Image.Placeholder ?? string.Empty }
            }, nameof(Article),article.Code).First() : null,
            Created = article.Created,
            LastUpdated = article.LastUpdated
        }).ToList();


        var pagination = new Pagination<ArticleDto>
        {
            CurrentPage = pageNumber,
            NextPage = pageNumber < (totalCount / pageSize) ? pageNumber + 1 : pageNumber,
            PreviousPage = pageNumber > 1 ? pageNumber - 1 : 1,
            HasNextPage = pageNumber < (totalCount / pageSize),
            HasPreviousPage = pageNumber > 1,
            LastPage = (int)Math.Ceiling((double)totalCount / pageSize),
            Data = articleList,
            TotalCount = totalCount
        };

        return new ServiceResponse<Pagination<ArticleDto>>
        {
            Count = totalCount,
            Data = pagination
        };
    }
    public async Task<ServiceResponse<bool>> DeleteArticle(Guid id)
    {
        var dbArticle = await _context.Articles.Include(x => x.Image).FirstOrDefaultAsync(x => x.Id == id);
        if (dbArticle == null)
        {
            return new ServiceResponse<bool>
            {
                Data = false
            };
        }
        if (dbArticle.Image is not null)
        {
            _byteFileUtility.DeleteFiles([dbArticle.Image], nameof(Article), dbArticle.Code);
        }
        _context.Articles.Remove(dbArticle);
        await _context.SaveChangesAsync();
        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<bool>> DeleteTrashAsync(Guid id)
    {
        var article = await _context.Articles
                                    .FirstOrDefaultAsync(p => p.Id == id);


        if (article == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "مقاله پیدا نشد."
            };
        }
        article.IsDeleted = true;
        article.IsActive = false;
        _context.Articles.Update(article);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Success = false,
            Message = $"مقاله {article.Code} با موفقیت حذف شد"
        };
    }

    public async Task<ServiceResponse<bool>> RestoreArticleAsync(Guid id)
    {
        var article = await _context.Articles
                                    .FirstOrDefaultAsync(p => p.Id == id);


        if (article == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "مقاله پیدا نشد."
            };
        }
        article.IsDeleted = false;
        _context.Articles.Update(article);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Success = false,
            Message = $"مقاله {article.Code} با موفقیت بازگردانی شد"
        };
    }

    public async Task<ServiceResponse<ArticleDto>> GetArticle(Guid id)
    {
        var article = await _context.Articles.Include(a => a.ArticleReviews).Include(a => a.Category).Include(x => x.Image).FirstOrDefaultAsync(x => x.Id == id);
        if (article == null)
        {
            return new ServiceResponse<ArticleDto>
            {
                Data = null,
                Success = false,
                Message = "مقاله مورد نظر یافت نشد"
            };
        }
        var articleDto = new ArticleDto
        {
            Id = article.Id,
            Title = article.Title,
            Slug = article.Slug,
            Category = article.Category != null ? article.Category.Name : "",
            Created = article.Created,
            LastUpdated = article.LastUpdated,
            CategoryId = article.CategoryId,
            Description = article.Description,
            IsActive = article.IsActive,
            Place = article.Place,
            Author = article.Author,
            NumReviews = article.ArticleReviews != null ? article.ArticleReviews.Count(x => x.Status == 2) : 0,
            Code = article.Code,
            IsDeleted = article.IsDeleted,
            Image = article.Image != null ? _byteFileUtility.GetEncryptedFileActionUrl(new List<EntityImageDto>
            {
                new EntityImageDto { Id = article.Image.Id , ImageUrl =article.Image.ImageUrl ?? string.Empty, Placeholder=article.Image.Placeholder ?? string.Empty}
            }, nameof(Article), article.Code).First() : null,

        };

        return new ServiceResponse<ArticleDto>
        {

            Data = articleDto
        };
    }

    public async Task<ServiceResponse<ArticleDto>> GetBy(string slug)
    {
        var article = await _context.Articles.Include(a => a.ArticleReviews).Include(a => a.Category).Include(x => x.Image).FirstOrDefaultAsync(x => x.Slug == slug);
        if (article == null)
        {
            return new ServiceResponse<ArticleDto>
            {
                Data = null,
                Success = false,
                Message = "مقاله مورد نظر یافت نشد"
            };
        }
        var articleDto = new ArticleDto
        {
            Id = article.Id,
            Title = article.Title,
            Slug = article.Slug,
            Category = article.Category != null ? article.Category.Name : "",
            Created = article.Created,
            LastUpdated = article.LastUpdated,
            CategoryId = article.CategoryId,
            Description = article.Description,
            IsActive = article.IsActive,
            Place = article.Place,
            Author = article.Author,
            NumReviews = article.ArticleReviews != null ? article.ArticleReviews.Count(x => x.Status == 2) : 0,
            Code = article.Code,
            IsDeleted = article.IsDeleted,
            Image = article.Image != null ? _byteFileUtility.GetEncryptedFileActionUrl(new List<EntityImageDto>
            {
                new EntityImageDto { Id = article.Image.Id , ImageUrl =article.Image.ImageUrl ?? string.Empty, Placeholder=article.Image.Placeholder ?? string.Empty}
            }, nameof(Article), article.Code).First() : null,

        };

        return new ServiceResponse<ArticleDto>
        {

            Data = articleDto
        };
    }

    public async Task<ServiceResponse<ArticleDto>> GetBy(Guid? id)
    {
        var article = await _context.Articles.Include(a => a.ArticleReviews).Include(a => a.Category).Include(x => x.Image).FirstOrDefaultAsync(x => x.CategoryId == id);
        if (article == null)
        {
            return new ServiceResponse<ArticleDto>
            {
                Data = null,
                Success = false,
                Message = "مقاله مورد نظر یافت نشد"
            };
        }
        var articleDto = new ArticleDto
        {
            Id = article.Id,
            Title = article.Title,
            Slug = article.Slug,
            Category = article.Category != null ? article.Category.Name : "",
            Created = article.Created,
            LastUpdated = article.LastUpdated,
            CategoryId = article.CategoryId,
            Description = article.Description,
            IsActive = article.IsActive,
            Place = article.Place,
            Author = article.Author,
            NumReviews = article.ArticleReviews != null ? article.ArticleReviews.Count(x => x.Status == 2) : 0,
            Code = article.Code,
            IsDeleted = article.IsDeleted,
            Image = article.Image != null ? _byteFileUtility.GetEncryptedFileActionUrl(new List<EntityImageDto>
            {
                new EntityImageDto { Id = article.Image.Id , ImageUrl =article.Image.ImageUrl ?? string.Empty, Placeholder=article.Image.Placeholder ?? string.Empty}
            }, nameof(Article), article.Code).First() : null,

        };

        return new ServiceResponse<ArticleDto>
        {

            Data = articleDto
        };
    }
}