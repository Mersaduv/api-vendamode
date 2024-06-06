using api_vendace.Data;
using api_vendace.Entities.Products;
using api_vendace.Interfaces;
using api_vendace.Interfaces.IServices;
using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto.Feature;
using Microsoft.EntityFrameworkCore;

namespace api_vendace.Services.Products;
public class FeatureServices : IFeatureServices
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public FeatureServices(ApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
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

    public async Task<ServiceResponse<List<ProductFeature>>> GetFeaturesByCategory(Guid id)
    {
        var features = await _context.ProductFeatures.Where(f => f.CategoryId == id)
                                 .Include(f => f.Values)
                                 .Include(f => f.Product)
                                 .AsNoTracking()
                                 .ToListAsync();

        return new ServiceResponse<List<ProductFeature>>
        {
            Count = features.Count,
            Data = features,
        };
    }
}