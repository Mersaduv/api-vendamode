using api_vendace.Data;
using api_vendace.Entities.Products;
using api_vendace.Interfaces;
using api_vendace.Interfaces.IServices;
using api_vendace.Models;
using api_vendace.Models.Dtos;
using api_vendace.Models.Dtos.ProductDto.Feature;
using api_vendace.Models.Dtos.ProductDto.Sizes;
using api_vendace.Models.Query;
using api_vendace.Utility;
using api_vendamode.Models.Dtos;
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

    public async Task<ServiceResponse<Pagination<ProductFeatureDto>>> GetAllFeatures(RequestQuery requestQuery)
    {
        var pageNumber = requestQuery.PageNumber ?? 1;
        var pageSize = requestQuery.PageSize ?? 15;

        var featuresQuery = _context.ProductFeatures
                                    .Include(f => f.Values)
                                    .Include(f => f.Products)
                                    .Include(c => c.CategoryProductFeatures)
                                    .ThenInclude(c => c.Category)
                                    .AsNoTracking()
                                    .AsQueryable();

        featuresQuery = featuresQuery.OrderByDescending(f => f.Values.Any(v => !string.IsNullOrEmpty(v.HexCode)))
                                     .ThenByDescending(f => f.LastUpdated);


        // Search filter
        if (!string.IsNullOrEmpty(requestQuery.Search))
        {
            string searchLower = requestQuery.Search.ToLower();
            featuresQuery = featuresQuery.Where(f => f.Name.ToLower().Contains(searchLower));
        }

        var totalCount = await featuresQuery.CountAsync();

        var paginatedFeatures = await featuresQuery
                                        .Skip((pageNumber - 1) * pageSize)
                                        .Take(pageSize)
                                        .Select(f => new ProductFeatureDto
                                        {
                                            Id = f.Id,
                                            Name = f.Name,
                                            Values = f.Values,
                                            // Count = f.Values.Sum(v => _context.Products.Count(p => p.FeatureValueIds.Contains(v.Id))),
                                            Count = f.Products.Count,
                                            ValueCount = f.Values != null ? f.Values.Count : f.Values!.Count,
                                            IsDeleted = f.IsDeleted,
                                            Created = f.Created,
                                            LastUpdated = f.LastUpdated
                                        })
                                        .ToListAsync();

        var pagination = new Pagination<ProductFeatureDto>
        {
            CurrentPage = pageNumber,
            NextPage = pageNumber < (totalCount / pageSize) ? pageNumber + 1 : pageNumber,
            PreviousPage = pageNumber > 1 ? pageNumber - 1 : 1,
            HasNextPage = pageNumber < (totalCount / pageSize),
            HasPreviousPage = pageNumber > 1,
            LastPage = (int)Math.Ceiling((double)totalCount / pageSize),
            Data = paginatedFeatures,
            TotalCount = totalCount
        };

        return new ServiceResponse<Pagination<ProductFeatureDto>>
        {
            Count = totalCount,
            Data = pagination
        };
    }


    public async Task<ServiceResponse<Pagination<FeatureValue>>> GetAllFeatureValues(RequestQuery requestQuery)
    {
        var pageNumber = requestQuery.PageNumber ?? 1;
        var pageSize = requestQuery.PageSize ?? 15;

        var featureValuesQuery = _context.FeatureValues
                                            .Include(f => f.ProductFeature)
                                            .OrderByDescending(f => f.LastUpdated)
                                            .AsNoTracking()
                                            .AsQueryable();

        // Feature filters based on featureIds
        if (requestQuery.FeatureIds?.Any() ?? false)
        {
            foreach (var featureId in requestQuery.FeatureIds)
            {
                featureValuesQuery = featureValuesQuery.Where(p => p.ProductFeature.Id == featureId);
            }
        }

        // Search filter
        if (!string.IsNullOrEmpty(requestQuery.Search))
        {
            string searchLower = requestQuery.Search.ToLower();
            featureValuesQuery = featureValuesQuery.Where(f => f.Name.ToLower().Contains(searchLower));
        }


        var totalCount = await featureValuesQuery.CountAsync();

        var paginatedFeatureValues = await featureValuesQuery
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .Select(fv => new FeatureValue
                                {
                                    Id = fv.Id,
                                    Name = fv.Name,
                                    Description = fv.Description,
                                    HexCode = fv.HexCode,
                                    Count = _context.Products.Count(p => p.FeatureValueIds != null && p.FeatureValueIds.Contains(fv.Id)),
                                    IsDeleted = fv.IsDeleted,
                                    Created = fv.Created,
                                    LastUpdated = fv.LastUpdated,
                                    ProductFeatureId = fv.ProductFeatureId
                                })
                                .ToListAsync();

        var pagination = new Pagination<FeatureValue>
        {
            CurrentPage = pageNumber,
            NextPage = pageNumber < (totalCount / pageSize) ? pageNumber + 1 : pageNumber,
            PreviousPage = pageNumber > 1 ? pageNumber - 1 : 1,
            HasNextPage = pageNumber < (totalCount / pageSize),
            HasPreviousPage = pageNumber > 1,
            LastPage = (int)Math.Ceiling((double)totalCount / pageSize),
            Data = paginatedFeatureValues,
            TotalCount = totalCount
        };

        return new ServiceResponse<Pagination<FeatureValue>>
        {
            Count = totalCount,
            Data = pagination
        };
    }

    public async Task<ServiceResponse<ProductFeature>> GetFeatureBy(Guid id)
    {
        var feature = await _context.ProductFeatures.Include(f => f.Values)
                                 .Include(f => f.Products)
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
        var featuresQuery = _context.ProductFeatures
                            .Include(f => f.Values)
                            .Include(f => f.Products)
                            .Include(c => c.CategoryProductFeatures)
                            .ThenInclude(c => c.Category)
                            .Where(f =>  f.CategoryProductFeatures.Any(x => x.CategoryId == categoryId))
                            .AsNoTracking()
                            .AsQueryable();
        
        // Fetch features related to the category
        var features = await featuresQuery
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