using System.Drawing;
using api_vendace.Data;
using api_vendace.Entities;
using api_vendace.Entities.Products;
using api_vendace.Interfaces;
using api_vendace.Interfaces.IServices;
using api_vendace.Models;
using api_vendace.Models.Dtos;
using api_vendace.Models.Dtos.ProductDto;
using api_vendace.Models.Dtos.ProductDto.Sizes;
using api_vendace.Utility;
using api_vendamode.Models.Dtos.ProductDto.Sizes;
using Microsoft.EntityFrameworkCore;

public class ProductSizeServices : IProductSizeServices
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ByteFileUtility _byteFileUtility;
    private readonly IHttpContextAccessor _httpContext;

    public ProductSizeServices(ApplicationDbContext context, IUnitOfWork unitOfWork, ByteFileUtility byteFileUtility, IHttpContextAccessor httpContext)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _byteFileUtility = byteFileUtility;
        _httpContext = httpContext;
    }

    public async Task<ServiceResponse<bool>> AddProductSize(ProductSizeCreateDTO productSizeCreate)
    {
        var sizeValueListDatabase = await _context.ProductSizeValues
                                        .Include(s => s.ProductSize)
                                        .AsNoTracking()
                                        .ToListAsync();
        var productSizeId = Guid.NewGuid();
        var sizeValueList = new List<ProductSizeValues>();
        if (productSizeCreate.ProductSizeValues is not null)
        {
            foreach (var sizeValue in productSizeCreate.ProductSizeValues)
            {
                // Check if sizeValue.Name already exists in the database
                var existingSizeValue = sizeValueListDatabase.FirstOrDefault(s => s.Name == sizeValue);

                if (existingSizeValue != null)
                {
                    // If it exists, map the existing object with the correct ID
                    sizeValueList.Add(existingSizeValue);
                }
                else
                {
                    // If it doesn't exist, create a new ProductSizeValues object
                    var nameValue = new ProductSizeValues
                    {
                        Id = Guid.NewGuid(),
                        Name = sizeValue,
                        ProductSizeId = productSizeId,
                        Created = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow
                    };
                    sizeValueList.Add(nameValue);
                }
            }
        }

        var productSize = new ProductSize
        {
            Id = productSizeId,
            Images = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, ProductSize>>(productSizeCreate.Thumbnail!, nameof(ProductSize), false),
            SizeType = productSizeCreate.SizeType,
            ProductSizeValues = sizeValueList,
            CategoryId = productSizeCreate.CategoryId,

            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
        };

        await _context.ProductSizes.AddAsync(productSize);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<bool>> UpdateProductSize(ProductSizeUpdateDTO productSizeUpdate)
    {
        var sizeValueListDatabase = await _context.ProductSizeValues
                                        .Include(s => s.ProductSize)
                                        .AsNoTracking()
                                        .ToListAsync();
        var productSize = await _context.ProductSizes
        .Include(s => s.Images)
        .Include(s => s.ProductSizeValues)
        .Include(s => s.Sizes)
        .FirstOrDefaultAsync(ps => ps.Id == productSizeUpdate.Id);

        var sizeValueList = new List<ProductSizeValues>();
        if (productSizeUpdate.ProductSizeValues is not null)
        {
            foreach (var sizeValue in productSizeUpdate.ProductSizeValues)
            {
                // Check if sizeValue.Name already exists in the database
                var existingSizeValue = sizeValueListDatabase.FirstOrDefault(s => s.Name == sizeValue.Name);

                if (existingSizeValue != null)
                {
                    // If it exists, map the existing object with the correct ID
                    existingSizeValue.LastUpdated = DateTime.UtcNow;
                    sizeValueList.Add(existingSizeValue);
                }
                else
                {
                    // If it doesn't exist, create a new ProductSizeValues object
                    var nameValue = new ProductSizeValues
                    {
                        Id = Guid.NewGuid(),
                        Name = sizeValue.Name,
                        ProductSizeId = sizeValue.ProductSizeId,
                        Created = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow
                    };
                    sizeValueList.Add(nameValue);
                }
            }
        }

        if (productSize == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "سایز مد نظر یافت نشد"
            };
        }
        productSize.Images = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, ProductSize>>(productSizeUpdate.Thumbnail!, nameof(ProductSize), false);
        productSize.ProductSizeValues = sizeValueList;
        productSize.CategoryId = productSizeUpdate.CategoryId;
        productSize.Created = productSizeUpdate.Created;
        productSize.LastUpdated = DateTime.UtcNow;

        _context.ProductSizes.Update(productSize);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<ProductSizeDTO>> GetProductSizeByCategory(Guid id)
    {
        var productSize = await _context.ProductSizes
            .Include(s => s.Images)
            .Include(s => s.Sizes)
            .Include(s => s.ProductSizeValues)
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (productSize == null)
        {
            return new ServiceResponse<ProductSizeDTO>
            {
                Data = null,
                Success = false,
                Message = "سایزبندی های دسته بندی یافت نشد"
            };
        }

        var productSizeDto = new ProductSizeDTO
        {
            Id = productSize.Id,
            Sizes = productSize.Sizes,
            ImagesSrc = productSize.Images != null ? _byteFileUtility.GetEncryptedFileActionUrl(productSize.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl ?? string.Empty,
                Placeholder = img.Placeholder ?? string.Empty
            }).ToList(), nameof(ProductSize)).First() : null,
            SizeType = productSize.SizeType,
            ProductSizeValues = productSize.ProductSizeValues?.Select(s => new ProductSizeValuesDTO
            {
                Id = s.Id,
                Name = s.Name,
                ProductSizeId = s.ProductSizeId
            }).ToList(),
            Created = productSize.Created,
            LastUpdated = productSize.LastUpdated
        };

        return new ServiceResponse<ProductSizeDTO>
        {
            Data = productSizeDto
        };
    }

    public async Task<ServiceResponse<bool>> AddSize(SizeCreateDTO sizeCreate)
    {
        var sizes = new Sizes
        {
            Id = Guid.NewGuid(),
            Name = sizeCreate.Name,
            Description = sizeCreate.Description,
            ProductSizeId = sizeCreate.ProductSizeId,
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
        };
        await _context.Sizes.AddAsync(sizes);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<bool>> DeleteSize(Guid id)
    {
        var sizes = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);
        if (sizes == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "سایز مورد نظر یافت نشد"
            };
        }
        _context.Sizes.Remove(sizes);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<bool>> UpdateSize(SizeUpdateDTO sizeUpdate)
    {
        var size = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == sizeUpdate.Id);
        if (size == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "سایز مورد نظر یافت نشد"
            };
        }

        size.Name = sizeUpdate.Name;
        size.Description = sizeUpdate.Description ?? string.Empty;
        size.LastUpdated = DateTime.UtcNow;

        _context.Sizes.Update(size);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<Sizes>> GetSizeBy(Guid id)
    {
        var size = await _context.Sizes.Include(s => s.ProductSize)
                                 .FirstOrDefaultAsync(s => s.Id == id);

        if (size == null)
        {
            return new ServiceResponse<Sizes>
            {
                Data = null,
                Message = "سایز مورد نظر یافت نشد"
            };
        }

        return new ServiceResponse<Sizes>
        {
            Data = size,
        };
    }

    public async Task<ServiceResponse<ProductSizeDTO>> GetProductSizeBy(Guid id)
    {
        var productSize = await _context.ProductSizes.Include(s => s.Sizes)
                         .FirstOrDefaultAsync(s => s.Id == id);

        if (productSize == null)
        {
            return new ServiceResponse<ProductSizeDTO>
            {
                Data = null,
                Message = "سایز مورد نظر یافت نشد"
            };
        }

        var productSizeDto = new ProductSizeDTO
        {
            Id = productSize.Id,
            ImagesSrc = productSize.Images != null ? _byteFileUtility.GetEncryptedFileActionUrl(productSize.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl ?? string.Empty,
                Placeholder = img.Placeholder ?? string.Empty
            }).ToList(), nameof(ProductSize)).First() : null
        };

        return new ServiceResponse<ProductSizeDTO>
        {
            Data = productSizeDto,
        };
    }

    public async Task<ServiceResponse<IReadOnlyList<SizeDTO>>> GetAllSizes()
    {
        var sizeList = await _context.Sizes.Include(s => s.ProductSize)
                                        .AsNoTracking()
                                        .ToListAsync();
        // find product == size count 
        var sizesDto = sizeList.Select(size => new SizeDTO
        {
            Id = size.Id,
            Count = 0,
            Description = size.Description,
            Name = size.Name,
            Created = size.Created,
            LastUpdated = size.LastUpdated
        }).ToList();


        return new ServiceResponse<IReadOnlyList<SizeDTO>>
        {
            Count = sizeList.Count,
            Data = sizesDto,
        };
    }

    public async Task<ServiceResponse<IReadOnlyList<ProductSize>>> GetAllProductSizes()
    {
        var productSizeList = await _context.ProductSizes
                                        .Include(s => s.Sizes)
                                        .AsNoTracking()
                                        .ToListAsync();

        return new ServiceResponse<IReadOnlyList<ProductSize>>
        {
            Count = productSizeList.Count,
            Data = productSizeList,
        };
    }
}