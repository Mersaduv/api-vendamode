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

namespace api_vendamode.Services;

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
        if (_context.Sliders.FirstOrDefaultAsync(x => x.Title == sliderCreateDto.Title).GetAwaiter().GetResult() != null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "اسلایدر با این عنوان قبلا ایجاد شده"
            };
        }
        var slider = new Slider
        {
            Id = Guid.NewGuid(),
            Title = sliderCreateDto.Title,
            Image = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Slider>>([sliderCreateDto.Thumbnail], nameof(Slider), false).First(),
            CategoryId = sliderCreateDto.CategoryId,
            Uri = sliderCreateDto.Uri,
            IsPublic = sliderCreateDto.IsPublic,
            IsMain = sliderCreateDto.IsMain,
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
        _context.Sliders.Remove(dbSlider);
        await _context.SaveChangesAsync();
        return new ServiceResponse<bool>
        {
            Data = true
        };
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
                                        }, nameof(Slider)).First() : null,
                                        IsMain = slider.IsMain,
                                        IsPublic = slider.IsPublic,
                                        Title = slider.Title,
                                        Uri = slider.Uri ?? string.Empty,
                                        Created = slider.Created,
                                        LastUpdated = slider.LastUpdated
                                    })
                                    .ToListAsync();

        var serviceResponse = new ServiceResponse<IReadOnlyList<SliderDto>> { Data = sliders };
        return serviceResponse;
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
            }, nameof(Slider)).First() : null,
            IsMain = slider.IsMain,
            IsPublic = slider.IsPublic,
            Title = slider.Title,
            Uri = slider.Uri ?? string.Empty,
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
        var sliders = await _context.Sliders.Include(x => x.Image).Select(slider => new SliderDto
        {
            Id = slider.Id,
            CategoryId = slider.CategoryId,
            Image = slider.Image != null ? _byteFileUtility.GetEncryptedFileActionUrl(new List<EntityImageDto>
            {
                new EntityImageDto { Id = slider.Image.Id , ImageUrl =slider.Image.ImageUrl ?? string.Empty, Placeholder=slider.Image.Placeholder ?? string.Empty}
            }, nameof(Slider)).First() : null,
            IsMain = slider.IsMain,
            IsPublic = slider.IsPublic,
            Title = slider.Title,
            Uri = slider.Uri ?? string.Empty,
            Created = slider.Created,
            LastUpdated = slider.LastUpdated
        }).ToListAsync();
        var serviceResponse = new ServiceResponse<IReadOnlyList<SliderDto>> { Data = sliders };
        return serviceResponse;
    }

    public async Task<ServiceResponse<bool>> UpdateSlider(SliderUpdateDto sliderDto)
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

        dbSlider.Title = sliderDto.Title;
        dbSlider.CategoryId = sliderDto.CategoryId;
        dbSlider.Uri = sliderDto.Uri;
        dbSlider.IsMain = sliderDto.IsMain;
        dbSlider.IsPublic = sliderDto.IsPublic;
        dbSlider.LastUpdated = DateTime.UtcNow;


        if (sliderDto.Thumbnail != null)
        {
            if (dbSlider.Image is not null)
            {
                _byteFileUtility.DeleteFiles([dbSlider.Image], nameof(Slider));
            }
            dbSlider.Image = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Slider>>([sliderDto.Thumbnail], nameof(Slider), false).First();
        }

        _context.Update(dbSlider);
        await _unitOfWork.SaveChangesAsync();
        var entityImageDtos = new List<EntityImageDto>
        {
            new EntityImageDto { Id = dbSlider.Image!.Id, ImageUrl = dbSlider.Image.ImageUrl!, Placeholder = dbSlider.Image.Placeholder! }
        };

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }
}
