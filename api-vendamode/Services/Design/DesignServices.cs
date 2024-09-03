using api_vendace.Data;
using api_vendace.Entities;
using api_vendace.Entities.Products;
using api_vendace.Interfaces;
using api_vendace.Models;
using api_vendace.Models.Dtos;
using api_vendace.Models.Dtos.ProductDto.Category;
using api_vendace.Utility;
using api_vendamode.Entities.Designs;
using api_vendamode.Entities.Products;
using api_vendamode.Interfaces.IServices;
using api_vendamode.Models.Dtos.designDto;
using api_vendamode.Models.Dtos.ProductDto;
using Microsoft.EntityFrameworkCore;

namespace api_vendamode.Services.Design;

public class DesignServices : IDesignServices
{
    private readonly ApplicationDbContext _context;
    private readonly ByteFileUtility _byteFileUtility;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoryServices _categoryServices;

    public DesignServices(ByteFileUtility byteFileUtility, ApplicationDbContext context, IUnitOfWork unitOfWork, ICategoryServices categoryServices)
    {
        _byteFileUtility = byteFileUtility;
        _context = context;
        _unitOfWork = unitOfWork;
        _categoryServices = categoryServices;
    }
    //HeaderText
    public async Task<ServiceResponse<bool>> UpsertHeaderText(HeaderTextUpsertDTO headerTextDto)
    {
        var headerTextDb = await _context.HeaderTexts.FirstOrDefaultAsync(x => x.Id == headerTextDto.Id);
        if (headerTextDb is null)
        {
            // Create new HeaderText
            var newHeaderText = new HeaderText
            {
                Id = headerTextDto.Id ?? Guid.NewGuid(),
                Name = headerTextDto.Name,
                IsActive = headerTextDto.IsActive,
                Created = DateTime.UtcNow
            };
            _context.HeaderTexts.Add(newHeaderText);
        }
        else
        {
            // Update existing HeaderText
            headerTextDb.Name = headerTextDto.Name;
            headerTextDb.IsActive = headerTextDto.IsActive;
            headerTextDb.LastUpdated = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool> { Data = true, Message = "بنر بالای سایت بروزرسانی شد" };
    }

    public async Task<ServiceResponse<HeaderText>> GetHeaderText()
    {
        var headerText = await _context.HeaderTexts.ToListAsync();
        if (headerText is null)
        {
            return new ServiceResponse<HeaderText>
            {
                Success = false,
                Message = "متن متحرکی وجود ندارد"
            };
        }

        return new ServiceResponse<HeaderText>
        {
            Data = headerText.Count > 0 ? headerText.First() : null,
            Success = true
        };
    }

    // LogoImages
    public async Task<ServiceResponse<bool>> UpsertLogoImages(LogoUpsertDTO logoUpsertDTO)
    {
        var logoImagesDb = await _context.LogoImages.Include(x => x.OrgThumbnail).Include(x => x.FaviconThumbnail).FirstOrDefaultAsync(x => x.Id == logoUpsertDTO.Id);
        if (logoImagesDb is null)
        {
            // Create new HeaderText
            var newLogoImage = new LogoImages
            {
                Id = logoUpsertDTO.Id ?? Guid.NewGuid(),
                OrgThumbnail = logoUpsertDTO.OrgThumbnail != null ? _byteFileUtility.SaveFileInFolder<EntityMainImage<Guid, LogoImages>>([logoUpsertDTO.OrgThumbnail], nameof(LogoImages), false).First() : null,
                FaviconThumbnail = logoUpsertDTO.FaviconThumbnail != null ? _byteFileUtility.SaveFileInFolder<EntityImage<Guid, LogoImages>>([logoUpsertDTO.FaviconThumbnail], nameof(LogoImages), false, false).First() : null,
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };
            await _context.LogoImages.AddAsync(newLogoImage);
        }
        else
        {
            if (logoUpsertDTO.OrgThumbnail != null)
            {
                if (logoImagesDb.OrgThumbnail is not null)
                {
                    _byteFileUtility.DeleteFiles([logoImagesDb.OrgThumbnail], nameof(LogoImages));
                }
                logoImagesDb.OrgThumbnail = _byteFileUtility.SaveFileInFolder<EntityMainImage<Guid, LogoImages>>([logoUpsertDTO.OrgThumbnail], nameof(LogoImages), false).First();
            }


            if (logoUpsertDTO.FaviconThumbnail != null)
            {
                if (logoImagesDb.FaviconThumbnail is not null)
                {
                    _byteFileUtility.DeleteFiles([logoImagesDb.FaviconThumbnail], nameof(LogoImages));
                }
                logoImagesDb.FaviconThumbnail = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, LogoImages>>([logoUpsertDTO.FaviconThumbnail], nameof(LogoImages), false, false).First();
            }

            _context.LogoImages.Update(logoImagesDb);
        }

        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool> { Data = true, Message = "لوگو سایت بروزرسانی شد" };
    }

    public async Task<ServiceResponse<IReadOnlyList<LogoImagesDTO>>> GetLogoImages()
    {
        var logoImages = await _context.LogoImages
            .Include(x => x.OrgThumbnail)
            .Include(x => x.FaviconThumbnail)
            .Select(x => new LogoImagesDTO
            {
                Id = x.Id,
                OrgImage = x.OrgThumbnail != null ? _byteFileUtility.GetEncryptedFileActionUrl(new List<EntityImageDto>
                    {
                        new EntityImageDto { Id = x.OrgThumbnail.Id , ImageUrl =x.OrgThumbnail.ImageUrl ?? string.Empty, Placeholder=x.OrgThumbnail.Placeholder ?? string.Empty}
                    }, nameof(LogoImages)).First() : null,
                FaviconImage = x.FaviconThumbnail != null ? _byteFileUtility.GetEncryptedFileActionUrl(new List<EntityImageDto>
                    {
                        new EntityImageDto { Id = x.FaviconThumbnail.Id , ImageUrl =x.FaviconThumbnail.ImageUrl ?? string.Empty, Placeholder=x.FaviconThumbnail.Placeholder ?? string.Empty}
                    }, nameof(LogoImages)).First() : null,
                Created = x.Created,
                LastUpdated = x.LastUpdated
            })
            .ToListAsync();

        return new ServiceResponse<IReadOnlyList<LogoImagesDTO>>
        {
            Data = logoImages
        };
    }

    public async Task<ServiceResponse<bool>> UpsertGeneralSettings(GeneralSettingUpsertDTO settingUpsertDTO)
    {
        var generalSettingDb = await _context.GeneralSettings.FirstOrDefaultAsync(x => x.Id == settingUpsertDTO.Id);
        if (generalSettingDb is null)
        {
            // Create new HeaderText
            var newHeaderText = new GeneralSetting
            {
                Id = Guid.NewGuid(),
                Title = settingUpsertDTO.Title,
                ShortIntroduction = settingUpsertDTO.ShortIntroduction,
                GoogleTags = settingUpsertDTO.GoogleTags,
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };
            _context.GeneralSettings.Add(newHeaderText);
        }
        else
        {
            // Update existing GeneralSettings
            generalSettingDb.Title = settingUpsertDTO.Title;
            generalSettingDb.ShortIntroduction = settingUpsertDTO.ShortIntroduction;
            generalSettingDb.GoogleTags = settingUpsertDTO.GoogleTags;
            generalSettingDb.LastUpdated = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool> { Data = true, Message = "تنظیمات عمومی سایت بروزرسانی شد" };
    }

    public async Task<ServiceResponse<GeneralSetting>> GetGeneralSettings()
    {
        var generalSettings = await _context.GeneralSettings.ToListAsync();
        if (generalSettings is null)
        {
            return new ServiceResponse<GeneralSetting>
            {
                Success = false,
                Message = "تنظیمات عمومی  وجود ندارد"
            };
        }

        return new ServiceResponse<GeneralSetting>
        {
            Data = generalSettings.Count > 0 ? generalSettings.First() : null,
            Success = true
        };
    }


    // design Item
    public async Task<ServiceResponse<bool>> AddDesignItem(DesignItemUpsertDTO designItemUpsertDto)
    {
        var designItem = new DesignItem
        {
            Id = Guid.NewGuid(),
            Title = designItemUpsertDto.Title ?? string.Empty,
            Image = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, DesignItem>>([designItemUpsertDto.Thumbnail], nameof(DesignItem), false).First(),
            Link = designItemUpsertDto.Link,
            Type = designItemUpsertDto.Type,
            IsActive = designItemUpsertDto.IsActive,
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        await _context.DesignItems.AddAsync(designItem);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<bool>> UpsertDesignItems(List<DesignItemUpsertDTO> designItemUpsertDtos)
    {
        foreach (var dto in designItemUpsertDtos)
        {
            var designItem = await _context.DesignItems.Include(x => x.Image).FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (designItem == null)
            {
                var newDesignItem = new DesignItem
                {
                    Id = Guid.NewGuid(),
                    Title = dto.Title ?? string.Empty,
                    Image = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, DesignItem>>(new List<IFormFile> { dto.Thumbnail }, nameof(DesignItem), false).First(),
                    Link = dto.Link,
                    Type = dto.Type,
                    IsActive = dto.IsActive,
                    Index = dto.Index,
                    Created = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                };
                await _context.DesignItems.AddAsync(newDesignItem);
            }
            else
            {
                designItem.Title = dto.Title ?? designItem.Title;
                designItem.Link = dto.Link;
                designItem.Type = dto.Type;
                designItem.IsActive = dto.IsActive;
                designItem.Index = dto.Index;
                designItem.LastUpdated = DateTime.UtcNow;

                if (dto.Thumbnail != null)
                {
                    if (designItem.Image != null)
                    {
                        _byteFileUtility.DeleteFiles(new List<EntityImage<Guid, DesignItem>> { designItem.Image }, nameof(DesignItem));
                    }
                    designItem.Image = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, DesignItem>>(new List<IFormFile> { dto.Thumbnail }, nameof(DesignItem), false).First();
                }

                _context.Update(designItem);
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool> { Data = true };
    }

    public async Task<ServiceResponse<bool>> DeleteDesignItem(Guid id)
    {
        var designItem = await _context.DesignItems.Include(x => x.Image).FirstOrDefaultAsync(x => x.Id == id);
        if (designItem == null)
        {
            return new ServiceResponse<bool> { Data = false, Message = "DesignItem not found." };
        }

        if (designItem.Image != null)
        {
            _byteFileUtility.DeleteFiles(new List<EntityImage<Guid, DesignItem>> { designItem.Image }, nameof(DesignItem));
        }

        _context.DesignItems.Remove(designItem);
        await _context.SaveChangesAsync();
        return new ServiceResponse<bool> { Data = true };
    }

    public async Task<ServiceResponse<DesignItemDTO>> GetDesignItemBy(Guid id)
    {
        var designItem = await _context.DesignItems.Include(x => x.Image).FirstOrDefaultAsync(x => x.Id == id);
        if (designItem == null)
        {
            return new ServiceResponse<DesignItemDTO>
            {
                Data = null,
                Success = false,
                Message = "DesignItem not found."
            };
        }

        var dto = new DesignItemDTO
        {
            Id = designItem.Id,
            Title = designItem.Title,
            Image = designItem.Image != null ? _byteFileUtility.GetEncryptedFileActionUrl(new List<EntityImageDto>
                {
                    new EntityImageDto { Id = designItem.Image.Id, ImageUrl = designItem.Image.ImageUrl ?? string.Empty, Placeholder = designItem.Image.Placeholder ?? string.Empty }
                }, nameof(DesignItem)).First() : null,
            Link = designItem.Link,
            Type = designItem.Type,
            IsActive = designItem.IsActive,
            Created = designItem.Created,
            LastUpdated = designItem.LastUpdated
        };

        return new ServiceResponse<DesignItemDTO> { Data = dto };
    }

    public async Task<ServiceResponse<IReadOnlyList<DesignItemDTO>>> GetDesignItems()
    {
        var designItems = await _context.DesignItems.OrderBy(s => s.Created).Include(x => x.Image).Select(designItem => new DesignItemDTO
        {
            Id = designItem.Id,
            Title = designItem.Title,
            Image = designItem.Image != null ? _byteFileUtility.GetEncryptedFileActionUrl(new List<EntityImageDto>
                {
                    new EntityImageDto { Id = designItem.Image.Id, ImageUrl = designItem.Image.ImageUrl ?? string.Empty, Placeholder = designItem.Image.Placeholder ?? string.Empty }
                }, nameof(DesignItem)).First() : null,
            Link = designItem.Link,
            Type = designItem.Type,
            IsActive = designItem.IsActive,
            Index = designItem.Index,
            Created = designItem.Created,
            LastUpdated = designItem.LastUpdated
        }).ToListAsync();

        return new ServiceResponse<IReadOnlyList<DesignItemDTO>> { Data = designItems };
    }

    public async Task<ServiceResponse<bool>> UpsertStoreCategory(List<StoreCategory> storeCategories)
    {
        foreach (var dto in storeCategories)
        {
            var storeCategory = await _context.StoreCategories.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (storeCategory == null)
            {
                var newDesignItem = new StoreCategory
                {
                    Id = Guid.NewGuid(),
                    CategoryId = dto.CategoryId,
                    Created = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                };
                await _context.StoreCategories.AddAsync(newDesignItem);
            }
            else
            {
                storeCategory.CategoryId = dto.CategoryId;
                storeCategory.LastUpdated = DateTime.UtcNow;
                _context.Update(storeCategory);
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool> { Data = true };
    }

    public async Task<ServiceResponse<IReadOnlyList<StoreCategory>>> GetStoreCategories()
    {
        var designItems = await _context.StoreCategories.OrderBy(s => s.Created).ToListAsync();

        return new ServiceResponse<IReadOnlyList<StoreCategory>> { Data = designItems };
    }

    public async Task<ServiceResponse<IReadOnlyList<CategoryDTO>>> GetStoreCategoryList()
    {
        var designItemsData = await _context.StoreCategories.ToListAsync();
        List<Guid> storeCategoryIds = designItemsData.Select(x => x.CategoryId).ToList();
        var categoriesQuery = _context.Categories
            .Where(x => storeCategoryIds.Contains(x.Id))
            .Include(c => c.ChildCategories)
            .Include(c => c.Images)
            .OrderByDescending(category => category.LastUpdated)
            .AsQueryable();
        var allCategories = await categoriesQuery.Select(category => new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Level = category.Level,
            IsActive = category.IsActive,
            IsActiveProduct = category.IsActiveProduct,
            IsDeleted = category.IsDeleted,
            ImagesSrc = category.Images != null ? _byteFileUtility.GetEncryptedFileActionUrl(category.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl ?? string.Empty,
                Placeholder = img.Placeholder ?? string.Empty
            }).ToList(), nameof(Category)).First() : null,

        }).ToListAsync();

        return new ServiceResponse<IReadOnlyList<CategoryDTO>> { Data = allCategories };
    }

    public async Task<ServiceResponse<bool>> DeleteStoreCategory(Guid id)
    {
        var storeCategory = await _context.StoreCategories.FirstOrDefaultAsync(x => x.Id == id);
        if (storeCategory == null)
        {
            return new ServiceResponse<bool> { Data = false, Message = "Store Category." };
        }

        _context.StoreCategories.Remove(storeCategory);
        await _context.SaveChangesAsync();
        return new ServiceResponse<bool> { Data = true };
    }

    public async Task<ServiceResponse<bool>> UpsertSloganFooter(SloganFooter sloganFooter)
    {
        var sloganFooterDb = await _context.SloganFooters.FirstOrDefaultAsync(x => x.Id == sloganFooter.Id);
        if (sloganFooterDb is null)
        {
            // Create new HeaderText
            var newSloganFooter = new SloganFooter
            {
                Id = Guid.NewGuid(),
                Headline = sloganFooter.Headline,
                IntroductionText = sloganFooter.IntroductionText,
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };
            _context.SloganFooters.Add(newSloganFooter);
        }
        else
        {
            // Update existing SloganFooter
            sloganFooterDb.Headline = sloganFooter.Headline;
            sloganFooterDb.IntroductionText = sloganFooter.IntroductionText;
            sloganFooterDb.LastUpdated = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool> { Data = true, Message = "شعار فوتر سایت بروزرسانی شد" };
    }

    public async Task<ServiceResponse<SloganFooter>> GetSloganFooters()
    {
        var sloganFootersDb = await _context.SloganFooters.ToListAsync();
        if (sloganFootersDb is null)
        {
            return new ServiceResponse<SloganFooter>
            {
                Success = false,
                Message = "شعار فوتر وجود ندارد"
            };
        }

        return new ServiceResponse<SloganFooter>
        {
            Data = sloganFootersDb.Count > 0 ? sloganFootersDb.First() : null,
            Success = true
        };
    }

    public async Task<ServiceResponse<bool>> UpsertSupport(Support support)
    {
        var supportDb = await _context.Supports.FirstOrDefaultAsync(x => x.Id == support.Id);
        if (supportDb is null)
        {
            // Create new HeaderText
            var newSupport = new Support
            {
                Id = Guid.NewGuid(),
                ContactAndSupport = support.ContactAndSupport,
                ResponseTime = support.ResponseTime,
                Address = support.Address,
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };
            _context.Supports.Add(newSupport);
        }
        else
        {
            // Update existing Support
            supportDb.ContactAndSupport = support.ContactAndSupport;
            supportDb.Address = support.Address;
            supportDb.LastUpdated = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool> { Data = true, Message = "تماس و پشتیبانی سایت بروزرسانی شد" };
    }

    public async Task<ServiceResponse<Support>> GetSupport()
    {
        var supports = await _context.Supports.ToListAsync();
        if (supports is null)
        {
            return new ServiceResponse<Support>
            {
                Success = false,
                Message = "تماس و پشتیبایی وجود ندارد"
            };
        }

        return new ServiceResponse<Support>
        {
            Data = supports.Count > 0 ? supports.First() : null,
            Success = true
        };
    }

    public async Task<ServiceResponse<bool>> UpsertRedirect(Redirects redirects)
    {
        var redirectsDb = await _context.Redirects.FirstOrDefaultAsync(x => x.Id == redirects.Id);
        if (redirectsDb is null)
        {
            var newRedirect = new Redirects
            {
                Id = Guid.NewGuid(),
                ArticleId = redirects.ArticleId,
                Created = DateTime.UtcNow
            };
            _context.Redirects.Add(newRedirect);
        }
        else
        {
            redirectsDb.ArticleId = redirects.ArticleId;
            redirectsDb.LastUpdated = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool> { Data = true, Message = "ریدایرکت ها سایت بروزرسانی شد" };
    }

    public async Task<ServiceResponse<Redirects>> GetRedirect()
    {
        var redirects = await _context.Redirects.ToListAsync();
        if (redirects is null)
        {
            return new ServiceResponse<Redirects>
            {
                Success = false,
                Message = "ریدایرکت ها وجود ندارد"
            };
        }

        return new ServiceResponse<Redirects>
        {
            Data = redirects.Count > 0 ? redirects.First() : null,
            Success = true
        };
    }

    public async Task<ServiceResponse<bool>> UpsertCopyright(Copyright copyrightDto)
    {
        var copyrightDb = await _context.Copyrights.FirstOrDefaultAsync(x => x.Id == copyrightDto.Id);
        if (copyrightDb is null)
        {
            var newCopyright = new Copyright
            {
                Id = Guid.NewGuid(),
                Name = copyrightDto.Name,
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };
            _context.Copyrights.Add(newCopyright);
        }
        else
        {
            copyrightDb.Name = copyrightDto.Name;
            copyrightDb.LastUpdated = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool> { Data = true, Message = "کپی رایت بروزرسانی شد" };
    }

    public async Task<ServiceResponse<Copyright>> GetCopyright()
    {
        var copyrights = await _context.Copyrights.ToListAsync();
        if (copyrights is null)
        {
            return new ServiceResponse<Copyright>
            {
                Success = false,
                Message = "کپی رایت وجود ندارد"
            };
        }

        return new ServiceResponse<Copyright>
        {
            Data = copyrights.Count > 0 ? copyrights.First() : null,
            Success = true
        };
    }

    public async Task<ServiceResponse<bool>> UpsertColumnFooters(List<ColumnFooter> columnFooters)
    {
        foreach (var columnFooterDto in columnFooters)
        {
            var columnFooterDb = await _context.ColumnFooters.FirstOrDefaultAsync(x => x.Id == columnFooterDto.Id);

            if (columnFooterDb is null)
            {
                // Create new HeaderText
                var newColumnFooter = new ColumnFooter
                {
                    Id = Guid.NewGuid(),
                    Name = columnFooterDto.Name,
                    Created = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                };
                _context.ColumnFooters.Add(newColumnFooter);
            }
            else
            {
                // Update existing HeaderText
                columnFooterDb.Name = columnFooterDto.Name;
                columnFooterDb.LastUpdated = DateTime.UtcNow;
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool> { Data = true, Message = "ستون های فوتر بروزرسانی شد" };
    }

    public async Task<ServiceResponse<List<ColumnFooter>>> GetColumnFooters()
    {
        var columnFooters = await _context.ColumnFooters.ToListAsync();
        if (columnFooters is null)
        {
            return new ServiceResponse<List<ColumnFooter>>
            {
                Success = false,
                Message = "ستون فوترها وجود ندارد"
            };
        }

        return new ServiceResponse<List<ColumnFooter>>
        {
            Count = columnFooters.Count,
            Data = columnFooters
        };
    }
}