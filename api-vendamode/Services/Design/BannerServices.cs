using api_vendace.Data;
using api_vendace.Entities;
using api_vendace.Interfaces;
using api_vendace.Models;
using api_vendace.Models.Dtos;
using api_vendace.Utility;
using api_vendamode.Entities.Products;
using api_vendamode.Interfaces.IServices;
using api_vendamode.Models.Dtos.designDto;
using Microsoft.EntityFrameworkCore;

namespace api_vendamode.Services.Design;

public class BannerServices : IBannerServices
{
    private readonly ApplicationDbContext _context;
    private readonly ByteFileUtility _byteFileUtility;
    private readonly IUnitOfWork _unitOfWork;

    public BannerServices(ByteFileUtility byteFileUtility, ApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _byteFileUtility = byteFileUtility;
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResponse<bool>> AddBanner(BannerCreateDto bannerCreateDto)
    {
        var banner = new Banner
        {
            Id = Guid.NewGuid(),
            CategoryId = bannerCreateDto.CategoryId,
            Image = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Banner>>([bannerCreateDto.Thumbnail], nameof(Banner), false).First(),
            Link = bannerCreateDto.Link,
            Type = bannerCreateDto.Type,
            IsActive = bannerCreateDto.IsActive,
            Index = bannerCreateDto.Index,
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        await _context.Banners.AddAsync(banner);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<bool>> UpsertBanners(List<BannerUpsertDto> bannerUpsertDtos)
    {
        foreach (var bannerUpsertDto in bannerUpsertDtos)
        {
            var bannerDb = await _context.Banners.Include(x => x.Image).FirstOrDefaultAsync(x => x.Id == bannerUpsertDto.Id);

            if (bannerDb is null)
            {
                var newBanner = new Banner
                {
                    Id = Guid.NewGuid(),
                    CategoryId = bannerUpsertDto.CategoryId,
                    Image = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Banner>>([bannerUpsertDto.Thumbnail!], nameof(Banner), false).First(),
                    Link = bannerUpsertDto.Link,
                    Type = bannerUpsertDto.Type,
                    IsActive = bannerUpsertDto.IsActive,
                    Index = bannerUpsertDto.Index,
                    Created = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                };

                await _context.Banners.AddAsync(newBanner);
            }
            else
            {
                bannerDb.CategoryId = bannerUpsertDto.CategoryId;
                bannerDb.Link = bannerUpsertDto.Link;
                bannerDb.Type = bannerUpsertDto.Type;
                bannerDb.IsActive = bannerUpsertDto.IsActive;
                bannerDb.Index = bannerUpsertDto.Index;
                bannerDb.LastUpdated = DateTime.UtcNow;

                if (bannerUpsertDto.Thumbnail != null)
                {
                    if (bannerDb.Image is not null)
                    {
                        _byteFileUtility.DeleteFiles([bannerDb.Image], nameof(Banner));
                    }
                    bannerDb.Image = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Banner>>([bannerUpsertDto.Thumbnail], nameof(Banner), false).First();
                }

                _context.Banners.Update(bannerDb);
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<bool>> UpsertFooterBanner(FooterBannerUpsertDto bannerUpsertDto)
    {
        var bannerDb = await _context.FooterBanners.Include(x => x.Image).FirstOrDefaultAsync(x => x.Id == bannerUpsertDto.Id);
        if (bannerDb is null)
        {
            // Create new banner
            var newBanner = new FooterBanner
            {
                Id = Guid.NewGuid(),
                CategoryId = bannerUpsertDto.CategoryId,
                Image = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, FooterBanner>>([bannerUpsertDto.Thumbnail!], nameof(FooterBanner), false).First(),
                Link = bannerUpsertDto.Link,
                Type = bannerUpsertDto.Type,
                IsActive = bannerUpsertDto.IsActive,
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            await _context.FooterBanners.AddAsync(newBanner);
        }
        else
        {
            // Update existing banner
            bannerDb.CategoryId = bannerUpsertDto.CategoryId;
            bannerDb.Link = bannerUpsertDto.Link;
            bannerDb.Type = bannerUpsertDto.Type;
            bannerDb.IsActive = bannerUpsertDto.IsActive;
            bannerDb.LastUpdated = DateTime.UtcNow;


            if (bannerUpsertDto.Thumbnail != null)
            {
                if (bannerDb.Image is not null)
                {
                    _byteFileUtility.DeleteFiles([bannerDb.Image], nameof(FooterBanner));
                }
                bannerDb.Image = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, FooterBanner>>([bannerUpsertDto.Thumbnail], nameof(FooterBanner), false).First();
            }


            _context.FooterBanners.Update(bannerDb);
        }

        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<bool>> UpdateBanner(BannerUpsertDto banner)
    {
        var bannerDb = await _context.Banners.Include(x => x.Image).FirstOrDefaultAsync(x => x.Id == banner.Id);
        if (bannerDb is null)
        {
            return new ServiceResponse<bool>
            {
                Data = false,
            };
        }

        bannerDb.CategoryId = banner.CategoryId;
        bannerDb.Link = banner.Link;
        bannerDb.Type = banner.Type;
        bannerDb.IsActive = banner.IsActive;
        bannerDb.Index = banner.Index;
        bannerDb.LastUpdated = DateTime.UtcNow;

        if (banner.Thumbnail != null)
        {
            if (bannerDb.Image is not null)
            {
                _byteFileUtility.DeleteFiles([bannerDb.Image], nameof(Banner));
            }
            bannerDb.Image = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Banner>>([banner.Thumbnail], nameof(Banner), false).First();
        }

        _context.Banners.Update(bannerDb);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<bool>> DeleteBanner(Guid id)
    {
        var bannerDb = await _context.Banners.Include(x => x.Image).FirstOrDefaultAsync(x => x.Id == id);
        if (bannerDb == null)
        {
            return new ServiceResponse<bool>
            {
                Data = false,
            };
        }

        _context.Banners.Remove(bannerDb);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<BannerDto>> GetBannerBy(Guid id)
    {
        var banner = await _context.Banners
            .Include(x => x.Image)
            .Select(x => new BannerDto
            {
                Id = x.Id,
                CategoryId = x.CategoryId,
                Image = x.Image != null ? _byteFileUtility.GetEncryptedFileActionUrl(new List<EntityImageDto>
            {
                new EntityImageDto { Id = x.Image.Id , ImageUrl =x.Image.ImageUrl ?? string.Empty, Placeholder=x.Image.Placeholder ?? string.Empty}
            }, nameof(Banner)).First() : null,
                Link = x.Link,
                Type = x.Type,
                IsActive = x.IsActive,
                Index = x.Index,
                Created = x.Created,
                LastUpdated = x.LastUpdated
            })
            .FirstOrDefaultAsync(x => x.Id == id);

        return new ServiceResponse<BannerDto>
        {
            Data = banner
        };
    }

    public async Task<ServiceResponse<IReadOnlyList<BannerDto>>> GetBanners()
    {
        var banners = await _context.Banners
            .Include(x => x.Image)
            .OrderByDescending(x => x.Index)
            .Select(x => new BannerDto
            {
                Id = x.Id,
                CategoryId = x.CategoryId,
                Image = x.Image != null ? _byteFileUtility.GetEncryptedFileActionUrl(new List<EntityImageDto>
                    {
                        new EntityImageDto { Id = x.Image.Id , ImageUrl =x.Image.ImageUrl ?? string.Empty, Placeholder=x.Image.Placeholder ?? string.Empty}
                    }, nameof(Banner)).First() : null,
                Link = x.Link,
                Type = x.Type,
                IsActive = x.IsActive,
                Index = x.Index,
                Created = x.Created,
                LastUpdated = x.LastUpdated
            })
            .ToListAsync();

        return new ServiceResponse<IReadOnlyList<BannerDto>>
        {
            Data = banners
        };
    }

    public async Task<ServiceResponse<IReadOnlyList<FooterBannerDto>>> GetFooterBanners()
    {
        var banners = await _context.FooterBanners
            .Include(x => x.Image)
            .OrderByDescending(x => x.Index)
            .Select(x => new FooterBannerDto
            {
                Id = x.Id,
                CategoryId = x.CategoryId,
                Image = x.Image != null ? _byteFileUtility.GetEncryptedFileActionUrl(new List<EntityImageDto>
                    {
                        new EntityImageDto { Id = x.Image.Id , ImageUrl =x.Image.ImageUrl ?? string.Empty, Placeholder=x.Image.Placeholder ?? string.Empty}
                    }, nameof(FooterBanner)).First() : null,
                Link = x.Link,
                Type = x.Type,
                IsActive = x.IsActive,
                Index = x.Index,
                Created = x.Created,
                LastUpdated = x.LastUpdated
            })
            .ToListAsync();

        return new ServiceResponse<IReadOnlyList<FooterBannerDto>>
        {
            Data = banners
        };
    }

    public async Task<ServiceResponse<bool>> UpsertArticleBanners(List<ArticleBannerUpsertDto> articleBannerUpserts)
    {
        foreach (var articleUpsertDto in articleBannerUpserts)
        {
            // Check if ArticleId is null
            if (articleUpsertDto.ArticleId == null)
            {
                // Get the Id of the articleBanner
                var articleBannerId = articleUpsertDto.Id;

                if (articleBannerId.HasValue)
                {
                    // Find the record in the database
                    var articleBannerToDelete = await _context.ArticleBanners
                        .FirstOrDefaultAsync(x => x.Id == articleBannerId.Value);

                    if (articleBannerToDelete != null)
                    {
                        // Remove the record from the database
                        _context.ArticleBanners.Remove(articleBannerToDelete);
                        continue; // Skip the rest of the loop iteration
                    }
                }
            }

            var articleBannerDb = await _context.ArticleBanners
                .Include(x => x.Article)
                .FirstOrDefaultAsync(x => x.Id == articleUpsertDto.Id);

            if (articleUpsertDto.ArticleId != null)
            {
                if (articleBannerDb is null)
                {
                    var newArticleBanner = new ArticleBanner
                    {
                        Id = Guid.NewGuid(),
                        ArticleId = (Guid)articleUpsertDto.ArticleId,
                        Index = articleUpsertDto.Index,
                        IsActive = articleUpsertDto.IsActive,
                        Created = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow
                    };

                    await _context.ArticleBanners.AddAsync(newArticleBanner);
                }
                else
                {
                    articleBannerDb.ArticleId = (Guid)articleUpsertDto.ArticleId;
                    articleBannerDb.Index = articleUpsertDto.Index;
                    articleBannerDb.IsActive = articleUpsertDto.IsActive;
                    articleBannerDb.LastUpdated = DateTime.UtcNow;

                    _context.ArticleBanners.Update(articleBannerDb);
                }
            }

        }

        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }


    public async Task<ServiceResponse<IReadOnlyList<ArticleBannerDto>>> GetArticleBanners()
    {
        var articleBanners = await _context.ArticleBanners
            .Include(x => x.Article)
            .OrderByDescending(x => x.Index)
            .Select(x => new ArticleBannerDto
            {
                Id = x.Id,
                ArticleId = x.ArticleId,
                ImagesSrc = x.Article != null && x.Article.Image != null ? _byteFileUtility.GetEncryptedFileActionUrl(new List<EntityImageDto>
                    {
                        new EntityImageDto { Id = x.Article.Image.Id , ImageUrl =x.Article.Image.ImageUrl ?? string.Empty, Placeholder=x.Article.Image.Placeholder ?? string.Empty}
                    }, nameof(Article)).First() : null,
                Title = x.Article != null ? x.Article.Title : "",
                Index = x.Index,
                IsActive = x.IsActive,
                Created = x.Created,
                LastUpdated = x.LastUpdated
            })
            .ToListAsync();

        return new ServiceResponse<IReadOnlyList<ArticleBannerDto>>
        {
            Data = articleBanners
        };
    }
}