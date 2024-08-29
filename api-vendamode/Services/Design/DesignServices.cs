using api_vendace.Data;
using api_vendace.Entities;
using api_vendace.Interfaces;
using api_vendace.Models;
using api_vendace.Models.Dtos;
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

    public DesignServices(ByteFileUtility byteFileUtility, ApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _byteFileUtility = byteFileUtility;
        _context = context;
        _unitOfWork = unitOfWork;
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
                Copyright = support.ContactAndSupport,
                Created = DateTime.UtcNow
            };
            _context.Supports.Add(newSupport);
        }
        else
        {
            // Update existing Support
            supportDb.ContactAndSupport = support.ContactAndSupport;
            supportDb.Copyright = support.ContactAndSupport;
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
            // Create new HeaderText
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
            // Update existing HeaderText
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
}