using api_vendace.Data;
using api_vendace.Entities.Products;
using api_vendace.Interfaces;
using api_vendace.Interfaces.IServices;
using api_vendace.Models;
using api_vendace.Models.Dtos;
using api_vendace.Models.Dtos.ProductDto.Feature;
using api_vendace.Models.Dtos.ProductDto.Sizes;
using api_vendace.Utility;
using api_vendamode.Models.Dtos.ProductDto;
using api_vendamode.Models.Dtos.ProductDto.Sizes;
using Microsoft.EntityFrameworkCore;

namespace api_vendace.Services.Products;
public class FeatureServices : IFeatureServices
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ByteFileUtility _byteFileUtility;

    public FeatureServices(ApplicationDbContext context, IUnitOfWork unitOfWork, ByteFileUtility byteFileUtility)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _byteFileUtility = byteFileUtility;
    }

    public async Task<ServiceResponse<bool>> AddFeature(ProductFeatureCreateDTO productFeatureCreate)
    {
        var productFeature = new ProductFeature
        {
            Id = Guid.NewGuid(),
            Name = productFeatureCreate.Name,
            Count = 0,
            Values = null,
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        await _context.ProductFeatures.AddAsync(productFeature);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }
    // value
    public async Task<ServiceResponse<bool>> AddFeatureValue(FeatureValueCreateDTO featureValueDto)
    {
        var featureValue = new FeatureValue
        {
            Id = Guid.NewGuid(),
            Name = featureValueDto.Name,
            Description = featureValueDto.Description,
            HexCode = featureValueDto.HexCode ?? null,
            ProductFeatureId = featureValueDto.ProductFeatureId,
            Count = 0,
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        await _context.FeatureValues.AddAsync(featureValue);
        await _unitOfWork.SaveChangesAsync();


        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<bool>> UpdateFeature(ProductFeatureUpdateDTO featureDTO)
    {
        var feature = await _context.ProductFeatures.FirstOrDefaultAsync(c => c.Id == featureDTO.Id);
        if (feature == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "ویژگی مورد نظر یافت نشد"
            };
        }

        feature.Name = featureDTO.Name;
        feature.LastUpdated = DateTime.UtcNow;

        _context.ProductFeatures.Update(feature);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<bool>> UpdateFeatureValue(ProductFeatureUpdateDTO featureDTO)
    {
        var featureValue = await _context.FeatureValues.FirstOrDefaultAsync(c => c.Id == featureDTO.Id);
        if (featureValue == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "مقدار ویژگی مورد نظر یافت نشد"
            };
        }

        featureValue.Name = featureDTO.Name;
        featureValue.Description = featureDTO.Description ?? string.Empty;
        featureValue.HexCode = featureDTO.HexCode ?? null;
        featureValue.LastUpdated = DateTime.UtcNow;


        _context.FeatureValues.Update(featureValue);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<bool>> DeleteFeature(Guid id)
    {
        var feature = await _context.ProductFeatures.FirstOrDefaultAsync(c => c.Id == id);
        if (feature == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "ویژگی مورد نظر یافت نشد"
            };
        }
        _context.ProductFeatures.Remove(feature);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<bool>> DeleteFeatureValue(Guid id)
    {
        var featureValue = await _context.FeatureValues.FirstOrDefaultAsync(c => c.Id == id);
        if (featureValue == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "مقدار ویژگی مورد نظر یافت نشد"
            };
        }
        _context.FeatureValues.Remove(featureValue);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<List<ProductFeature>>> GetAllFeatures()
    {
        var features = await _context.ProductFeatures
                                        .Include(f => f.Values)
                                        .Include(f => f.Product)
                                        .Include(c => c.Category)
                                        .AsNoTracking()
                                        .ToListAsync();


        return new ServiceResponse<List<ProductFeature>>
        {
            Count = features.Count,
            Data = features,
        };
    }

    public async Task<ServiceResponse<IReadOnlyList<FeatureValue>>> GetAllFeatureValues()
    {
        var featureValues = await _context.FeatureValues
                                        .AsNoTracking()
                                        .ToListAsync();


        return new ServiceResponse<IReadOnlyList<FeatureValue>>
        {
            Count = featureValues.Count,
            Data = featureValues,
        };
    }

    public async Task<ServiceResponse<ProductFeature>> GetFeatureBy(Guid id)
    {
        var feature = await _context.ProductFeatures.Include(f => f.Values)
                                 .Include(f => f.Product)
                                 .FirstOrDefaultAsync(f => f.Id == id);

        if (feature == null)
        {
            return new ServiceResponse<ProductFeature>
            {
                Data = null,
                Message = "ویژگی مورد نظر یافت نشد"
            };
        }

        return new ServiceResponse<ProductFeature>
        {
            Data = feature,
        };
    }

    public async Task<ServiceResponse<FeatureValue>> GetFeatureValueBy(Guid id)
    {
        var featureValue = await _context.FeatureValues
                         .FirstOrDefaultAsync(f => f.Id == id);

        if (featureValue == null)
        {
            return new ServiceResponse<FeatureValue>
            {
                Data = null,
                Message = "مقدار ویژگی مورد نظر یافت نشد"
            };
        }

        return new ServiceResponse<FeatureValue>
        {
            Data = featureValue,
        };
    }

    public async Task<ServiceResponse<GetCategoryFeaturesByCategory>> GetFeaturesByCategory(Guid categoryId)
    {
        // Fetch features related to the category
        var features = await _context.ProductFeatures
            .Where(f => f.CategoryId == categoryId)
            .Include(f => f.Values)
            .Include(f => f.Product)
            .ToListAsync();

        // Fetch product sizes related to the category
        var productSizes = await _context.ProductSizes
            .Include(ps => ps.Images)
            .Include(ps => ps.ProductSizeProductSizeValues)
            .ThenInclude(pspsv => pspsv.ProductSizeValue)
            .Include(ps => ps.CategoryProductSizes)
            .ThenInclude(cps => cps.Category)
            .Where(ps => ps.CategoryProductSizes.Any(cps => cps.CategoryId == categoryId))
            .ToListAsync();


        var categorySizes = await _context.Sizes
        .Where(ps => ps.CategorySizes.Any(cps => cps.CategoryId == categoryId))
        .ToListAsync();

        var sizesDto = categorySizes.Select(size => new SizeDTO
        {
            Id = size.Id,
            Count = 0,
            Description = size.Description,
            Name = size.Name,
            Created = size.Created,
            LastUpdated = size.LastUpdated
        }).ToList();

        // Map product sizes to ProductSizeDTO
        var productSizeDto = productSizes.Select(productSize => new ProductSizeDTO
        {
            Id = productSize.Id,
            ImagesSrc = productSize.Images != null ? _byteFileUtility.GetEncryptedFileActionUrl(productSize.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl ?? string.Empty,
                Placeholder = img.Placeholder ?? string.Empty
            }).ToList(), nameof(ProductSize)).First() : null,
            SizeType = productSize.SizeType,
            ProductSizeValues = productSize.ProductSizeProductSizeValues?.Select(sv => new ProductSizeValuesDTO
            {
                Id = sv.ProductSizeValue!.Id,
                Name = sv.ProductSizeValue.Name,
                ProductSizeId = sv.ProductSizeId
            }).ToList(),
            Created = productSize.Created,
            LastUpdated = productSize.LastUpdated
        }).ToList();

        // Return the response
        return new ServiceResponse<GetCategoryFeaturesByCategory>
        {
            Count = features.Count,
            Data = new GetCategoryFeaturesByCategory
            {
                ProductFeatures = features,
                ProductSizes = productSizeDto,
                SizeDTOs = sizesDto
            },
        };
    }
}