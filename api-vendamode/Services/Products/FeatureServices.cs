using api_vendamode.Data;
using api_vendamode.Entities.Products;
using api_vendamode.Interfaces;
using api_vendamode.Interfaces.IServices;
using api_vendamode.Models;
using api_vendamode.Models.Dtos.ProductDto.Feature;
using Microsoft.EntityFrameworkCore;

namespace api_vendamode.Services.Products;
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

        await _context.Features.AddAsync(productFeature);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }
    // value
    public async Task<ServiceResponse<bool>> AddFeatureValue(FeatureValueCreateDTO featureValueDto)
    {
        var feartureValue = new FeatureValue
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

        await _context.FeatureValues.AddAsync(feartureValue);
        await _unitOfWork.SaveChangesAsync();


        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<bool>> UpdateFeature(ProductFeatureUpdateDTO featureDTO)
    {
        var fearture = await _context.Features.FirstOrDefaultAsync(c => c.Id == featureDTO.Id);
        if (fearture == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "ویژگی مورد نظر یافت نشد"
            };
        }

        fearture.Name = featureDTO.Name;
        fearture.LastUpdated = DateTime.UtcNow;

        _context.Features.Update(fearture);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<bool>> UpdateFeatureValue(ProductFeatureUpdateDTO featureDTO)
    {
        var featurValue = await _context.FeatureValues.FirstOrDefaultAsync(c => c.Id == featureDTO.Id);
        if (featurValue == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "مقدار ویژگی مورد نظر یافت نشد"
            };
        }

        featurValue.Name = featureDTO.Name;
        featurValue.Description = featureDTO.Description ?? string.Empty;
        featurValue.HexCode = featureDTO.HexCode ?? null;
        featurValue.LastUpdated = DateTime.UtcNow;


        _context.FeatureValues.Update(featurValue);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<bool>> DeleteFeature(Guid id)
    {
        var feature = await _context.Features.FirstOrDefaultAsync(c => c.Id == id);
        if (feature == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "ویژگی مورد نظر یافت نشد"
            };
        }
        feature.IsDeleted = true;
        _context.Features.Update(feature);
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
        featureValue.IsDeleted = true;
        _context.FeatureValues.Update(featureValue);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<IReadOnlyList<ProductFeature>>> GetAllFeatures()
    {
        var features = await _context.Features.Include(f => f.Values)
                                        .Include(f => f.Product)
                                        .AsNoTracking()
                                        .ToListAsync();


        return new ServiceResponse<IReadOnlyList<ProductFeature>>
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
        var feature = await _context.Features.Include(f => f.Values)
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
        var features = await _context.Features.Where(f => f.CategoryId == id)
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