using api_vendace.Data;
using api_vendace.Entities;
using api_vendace.Entities.Products;
using api_vendace.Interfaces;
using api_vendace.Interfaces.IServices;
using api_vendace.Models;
using api_vendace.Models.Dtos;
using api_vendace.Models.Dtos.ProductDto.Brand;
using api_vendace.Models.Query;
using api_vendace.Utility;
using Microsoft.EntityFrameworkCore;

namespace api_vendace.Services.Products;

public class BrandServices : IBrandServices
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ByteFileUtility _byteFileUtility;

    public BrandServices(ApplicationDbContext context, ByteFileUtility byteFileUtility, IUnitOfWork unitOfWork)
    {
        _context = context;
        _byteFileUtility = byteFileUtility;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResponse<bool>> AddBrand(BrandCommandDTO brandCreate)
    {
        if (await _context.Brands.FirstOrDefaultAsync(b => b.NameFa == brandCreate.NameFa) != null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "برند وارد شده قبلا ایجاد شده."
            };
        }

        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            NameFa = brandCreate.NameFa,
            NameEn = brandCreate.NameEn,
            Count = 0,
            Images = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Brand>>(brandCreate.Thumbnail!, nameof(Brand), false),
            Description = brandCreate.Description ?? "",
            InSlider = brandCreate.InSlider,
            IsActive = brandCreate.IsActive,
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        var result = await _context.Brands.AddAsync(brand);
        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool>
        {
            Data = true,
        };
    }

    public async Task<ServiceResponse<bool>> DeleteBrand(Guid id)
    {
        var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);
        if (brand == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "برندی پیدا نشد"
            };
        }
        _context.Brands.Remove(brand);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<BrandDTO>> GetBrandBy(Guid id)
    {
        var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);
        if (brand == null)
        {
            return new ServiceResponse<BrandDTO>
            {
                Success = false,
                Message = "برندی پیدا نشد"
            };
        }

        var result = new BrandDTO
        {
            Id = brand.Id,
            ImagesSrc = brand.Images != null ? _byteFileUtility.GetEncryptedFileActionUrl(brand.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl ?? string.Empty,
                Placeholder = img.Placeholder ?? string.Empty
            }).ToList(), nameof(Brand)).First() : null,
            NameFa = brand.NameFa,
            NameEn = brand.NameEn,
            Description = brand.Description,
            Count = brand.Products is not null ? brand.Products.Count : 0,
            InSlider = brand.InSlider,
            IsActive = brand.IsActive,
            Created = brand.Created,
            LastUpdated = brand.LastUpdated
        };

        return new ServiceResponse<BrandDTO>
        {
            Data = result
        };
    }

    public async Task<ServiceResponse<IReadOnlyList<Brand>>> GetAllBrands()
    {
        var result = await _context.Brands.ToListAsync();

        return new ServiceResponse<IReadOnlyList<Brand>>
        {
            Data = result
        };
    }

    public async Task<ServiceResponse<Pagination<BrandDTO>>> GetBrands(RequestQuery requestQuery)
    {
        var pageSize = requestQuery.PageSize ?? 15;
        var pageNumber = requestQuery.PageNumber ?? 1;

        var brandsQuery = _context.Brands
                            .Include(b => b.Images)
                            .OrderByDescending(b => b.LastUpdated)
                            .AsNoTracking()
                            .AsQueryable();


        // Search filter
        if (!string.IsNullOrEmpty(requestQuery.Search))
        {
            string searchLower = requestQuery.Search.ToLower();
            brandsQuery = brandsQuery.Where(f => f.NameFa.ToLower().Contains(searchLower) || f.NameEn.ToLower().Contains(searchLower));
        }

        // active brand Slider Filter
        if (requestQuery.IsSlider is not null)
        {
            brandsQuery = brandsQuery.Where(x => x.InSlider);
        }

        // isActive Filter
        if (requestQuery.IsActive is not null)
        {
            brandsQuery = brandsQuery.Where(x => x.IsActive);
        }

        // InActive Filter
        if (requestQuery.InActive is not null)
        {
            brandsQuery = brandsQuery.Where(x => !x.IsActive);
        }

        var totalCount = await brandsQuery.CountAsync();

        var brands = await brandsQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(brand => new BrandDTO
            {
                Id = brand.Id,
                ImagesSrc = brand.Images != null ? _byteFileUtility.GetEncryptedFileActionUrl(brand.Images.Select(img => new EntityImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl ?? string.Empty,
                    Placeholder = img.Placeholder ?? string.Empty
                }).ToList(), nameof(Brand)).First() : null,
                NameFa = brand.NameFa,
                NameEn = brand.NameEn,
                Description = brand.Description,
                Count = brand.Products != null ? brand.Products.Count : 0,
                InSlider = brand.InSlider,
                IsActive = brand.IsActive,
                Created = brand.Created,
                LastUpdated = brand.LastUpdated
            })
            .ToListAsync();

        var pagination = new Pagination<BrandDTO>
        {
            CurrentPage = pageNumber,
            NextPage = pageNumber < (totalCount / pageSize) ? pageNumber + 1 : pageNumber,
            PreviousPage = pageNumber > 1 ? pageNumber - 1 : 1,
            HasNextPage = pageNumber < (totalCount / pageSize),
            HasPreviousPage = pageNumber > 1,
            LastPage = (int)Math.Ceiling((double)totalCount / pageSize),
            Data = brands,
            TotalCount = totalCount
        };

        return new ServiceResponse<Pagination<BrandDTO>>
        {
            Count = totalCount,
            Data = pagination
        };
    }


    public async Task<ServiceResponse<bool>> UpdateBrand(BrandCommandDTO brandUpdate)
    {
        var brand = await _context.Brands.Include(b => b.Images).FirstOrDefaultAsync(b => b.Id == brandUpdate.Id);
        if (brand == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "برندی پیدا نشد"
            };
        }

        brand.NameFa = brandUpdate.NameFa;
        brand.NameEn = brandUpdate.NameEn;
        brand.Description = brandUpdate.Description ?? "";
        brand.InSlider = brandUpdate.InSlider;
        brand.IsActive = brandUpdate.IsActive;
        brand.LastUpdated = DateTime.UtcNow;

        if (brandUpdate.Thumbnail != null)
        {
            if (brand.Images != null)
            {
                _byteFileUtility.DeleteFiles(brand.Images, nameof(Brand));
            }
            brand.Images = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Brand>>(brandUpdate.Thumbnail, nameof(Brand), false);
        }

        _context.Brands.Update(brand);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }


}

