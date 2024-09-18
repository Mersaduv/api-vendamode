using api_vendace.Data;
using api_vendace.Entities;
using api_vendace.Interfaces;
using api_vendace.Models;
using api_vendace.Models.Dtos;
using api_vendace.Utility;
using api_vendamode.Entities.Products;
using api_vendamode.Interfaces.IServices;
using api_vendamode.Models.Dtos.ProductDto;
using Microsoft.EntityFrameworkCore;

namespace api_vendamode.Services.Design;

public class SliderServices : ISliderServices
{
    private readonly ApplicationDbContext _context;
    private readonly ByteFileUtility _byteFileUtility;
    private readonly IUnitOfWork _unitOfWork;

    public SliderServices(ByteFileUtility byteFileUtility, ApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _byteFileUtility = byteFileUtility;
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResponse<bool>> AddSlider(SliderCreateDto sliderCreateDto)
    {
        var slider = new Slider
        {
            Id = Guid.NewGuid(),
            CategoryId = sliderCreateDto.CategoryId,
            Image = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Slider>>([sliderCreateDto.Thumbnail], nameof(Slider), null, false).First(),
            Link = sliderCreateDto.Link,
            Type = sliderCreateDto.Type,
            IsActive = sliderCreateDto.IsActive,
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        await _context.Sliders.AddAsync(slider);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<bool>> UpsertSliders(List<SliderUpsertDto> sliderUpsertDtos)
    {
        foreach (var sliderUpsertDto in sliderUpsertDtos)
        {
            // Your existing upsert logic for each slider
            var sliderDb = await _context.Sliders.Include(x => x.Image).FirstOrDefaultAsync(x => x.Id == sliderUpsertDto.Id);
            if (sliderDb is null)
            {
                // Create new slider
                var newSlider = new Slider
                {
                    Id = Guid.NewGuid(),
                    CategoryId = sliderUpsertDto.CategoryId,
                    Image = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Slider>>(new List<IFormFile> { sliderUpsertDto.Thumbnail }, nameof(Slider), "SubSlider", false).First(),
                    Link = sliderUpsertDto.Link,
                    Type = sliderUpsertDto.Type,
                    IsActive = sliderUpsertDto.IsActive,
                    Created = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                };
                await _context.Sliders.AddAsync(newSlider);
            }
            else
            {
                sliderDb.CategoryId = sliderUpsertDto.CategoryId;
                sliderDb.Link = sliderUpsertDto.Link;
                sliderDb.Type = sliderUpsertDto.Type;
                sliderDb.IsActive = sliderUpsertDto.IsActive;
                sliderDb.LastUpdated = DateTime.UtcNow;

                if (sliderUpsertDto.Thumbnail != null)
                {
                    if (sliderDb.Image is not null)
                    {
                        _byteFileUtility.DeleteFiles(new List<EntityImage<Guid, Slider>> { sliderDb.Image }, nameof(Slider), "SubSlider");
                    }

                    sliderDb.Image = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Slider>>(new List<IFormFile> { sliderUpsertDto.Thumbnail }, nameof(Slider), "SubSlider", false).First();
                }

                _context.Update(sliderDb);
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool> { Data = true };
    }

    public async Task<ServiceResponse<bool>> DeleteSlider(Guid id)
    {
        var dbSlider = await _context.Sliders.Include(x => x.Image).FirstOrDefaultAsync(x => x.Id == id);
        if (dbSlider == null)
        {
            return new ServiceResponse<bool>
            {
                Data = false
            };
        }
        if (dbSlider.Image is not null)
        {
            _byteFileUtility.DeleteFiles(new List<EntityImage<Guid, Slider>> { dbSlider.Image }, nameof(Slider), "SubSlider");
        }

        _context.Sliders.Remove(dbSlider);
        await _context.SaveChangesAsync();
        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<SliderDto>> GetSliderBy(Guid id)
    {
        var slider = await _context.Sliders.Include(x => x.Image).FirstOrDefaultAsync(x => x.Id == id);
        if (slider == null)
        {
            return new ServiceResponse<SliderDto>
            {
                Data = null,
                Success = false,
                Message = "اسلایدر مد نظر پیدا نشد."
            };
        }

        var result = new SliderDto
        {
            Id = slider.Id,
            CategoryId = slider.CategoryId,
            Image = slider.Image != null ? _byteFileUtility.GetEncryptedFileActionUrl(new List<EntityImageDto>
            {
                new EntityImageDto { Id = slider.Image.Id , ImageUrl =slider.Image.ImageUrl ?? string.Empty, Placeholder=slider.Image.Placeholder ?? string.Empty}
            }, nameof(Slider),"SubSlider").First() : null,
            Link = slider.Link,
            Type = slider.Type,
            IsActive = slider.IsActive,
            Created = slider.Created,
            LastUpdated = slider.LastUpdated
        };

        return new ServiceResponse<SliderDto>
        {
            Data = result
        };
    }

    public async Task<ServiceResponse<IReadOnlyList<SliderDto>>> GetSliders()
    {
        var sliders = await _context.Sliders.OrderBy(s => s.Created).Include(x => x.Image).Select(slider => new SliderDto
        {
            Id = slider.Id,
            CategoryId = slider.CategoryId,
            Image = slider.Image != null ? _byteFileUtility.GetEncryptedFileActionUrl(new List<EntityImageDto>
            {
                new EntityImageDto { Id = slider.Image.Id , ImageUrl =slider.Image.ImageUrl ?? string.Empty, Placeholder=slider.Image.Placeholder ?? string.Empty}
            }, nameof(Slider), "SubSlider").First() : null,
            Link = slider.Link,
            Type = slider.Type,
            IsActive = slider.IsActive,
            Created = slider.Created,
            LastUpdated = slider.LastUpdated
        }).ToListAsync();

        return new ServiceResponse<IReadOnlyList<SliderDto>> { Data = sliders };
    }

    public async Task<ServiceResponse<IReadOnlyList<SliderDto>>> GetMainSliders()
    {
        var sliders = await _context.Sliders
                                    .AsNoTracking()
                                    .Where(s => s.CategoryId == null)
                                    .Include(x => x.Image)
                                    .Select(slider => new SliderDto
                                    {
                                        Id = slider.Id,
                                        CategoryId = slider.CategoryId,
                                        Image = slider.Image != null ? _byteFileUtility.GetEncryptedFileActionUrl(new List<EntityImageDto>
                                        {
                                        new EntityImageDto { Id = slider.Image.Id, ImageUrl = slider.Image.ImageUrl ?? string.Empty, Placeholder = slider.Image.Placeholder ?? string.Empty}
                                        }, nameof(Slider), "SubSlider").First() : null,
                                        Link = slider.Link,
                                        Type = slider.Type,
                                        IsActive = slider.IsActive,
                                        Created = slider.Created,
                                        LastUpdated = slider.LastUpdated
                                    })
                                    .ToListAsync();

        return new ServiceResponse<IReadOnlyList<SliderDto>> { Data = sliders };
    }

    public async Task<ServiceResponse<bool>> UpdateSlider(SliderUpsertDto sliderDto)
    {
        var dbSlider = await _context.Sliders.Include(x => x.Image).FirstOrDefaultAsync(x => x.Id == sliderDto.Id);
        if (dbSlider == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "اسلایدر مد نظر پیدا نشد."
            };
        }

        dbSlider.CategoryId = sliderDto.CategoryId;
        dbSlider.Link = sliderDto.Link;
        dbSlider.Type = sliderDto.Type;
        dbSlider.IsActive = sliderDto.IsActive;
        dbSlider.LastUpdated = DateTime.UtcNow;

        if (sliderDto.Thumbnail != null)
        {
            if (dbSlider.Image is not null)
            {
                _byteFileUtility.DeleteFiles([dbSlider.Image], nameof(Slider),"SubSlider");
            }
            dbSlider.Image = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Slider>>([sliderDto.Thumbnail], nameof(Slider), null, false).First();
        }

        _context.Update(dbSlider);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

}

// good looking - suki waterhouse
// technike ultra slowed
// witiwant super slowed
// duvet  - boa
// enemy sped up tommee profitt, beacon light 
