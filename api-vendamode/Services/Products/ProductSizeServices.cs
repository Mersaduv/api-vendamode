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
using api_vendace.Models.Query;
using api_vendace.Utility;
using api_vendamode.Entities.Products;
using api_vendamode.Models.Dtos.ProductDto.Sizes;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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
        var productSizeId = Guid.NewGuid();
        var sizeValueList = new List<ProductSizeProductSizeValue>();

        if (productSizeCreate.ProductSizeValues is not null)
        {
            foreach (var sizeValue in productSizeCreate.ProductSizeValues)
            {
                // Check if sizeValue.Name already exists in the database
                var existingSizeValue = await _context.ProductSizeValues
                    .FirstOrDefaultAsync(s => s.Name == sizeValue);

                if (existingSizeValue != null)
                {
                    // Add the existing size value to the junction table
                    sizeValueList.Add(new ProductSizeProductSizeValue
                    {
                        ProductSizeId = productSizeId,
                        ProductSizeValueId = existingSizeValue.Id
                    });
                }
                else
                {
                    // Create a new ProductSizeValues object and add it to the junction table
                    var nameValue = new ProductSizeValues
                    {
                        Id = Guid.NewGuid(),
                        Name = sizeValue,
                        Created = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow
                    };

                    _context.ProductSizeValues.Add(nameValue);
                    sizeValueList.Add(new ProductSizeProductSizeValue
                    {
                        ProductSizeId = productSizeId,
                        ProductSizeValueId = nameValue.Id
                    });
                }
            }
        }

        var productSize = new ProductSize
        {
            Id = productSizeId,
            Images = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, ProductSize>>(productSizeCreate.Thumbnail!, nameof(ProductSize), false),
            SizeType = productSizeCreate.SizeType,
            ProductSizeProductSizeValues = sizeValueList,
            IsDeleted = false,
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
        };

        await _context.ProductSizes.AddAsync(productSize);

        await _context.ProductSizeProductSizeValues.AddRangeAsync(sizeValueList);

        try
        {
            await _unitOfWork.SaveChangesAsync();


            // Add the relationship between the ProductSize and Categories
            if (productSizeCreate.CategoryIds is not null)
            {
                var categoryProductSizes = productSizeCreate.CategoryIds.Select(categoryId => new CategoryProductSize
                {
                    CategoryId = categoryId,
                    ProductSizeId = productSizeId
                }).ToList();
                await _context.CategoryProductSizes.AddRangeAsync(categoryProductSizes);
                await _unitOfWork.SaveChangesAsync();
            }
        }
        catch (DbUpdateException ex)
        {
            // Log the error
            // Handle the duplicate key exception here
            var postgresException = ex.GetBaseException() as PostgresException;
            if (postgresException != null && postgresException.SqlState == "23505")
            {
                // Duplicate key exception
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Duplicate key value violates unique constraint."
                };
            }
            else
            {
                throw;
            }
        }

        return new ServiceResponse<bool>
        {
            Data = true,
            Message = "اندازه دسته بندی با موفقیت ایجاد شد"
        };
    }


    public async Task<ServiceResponse<bool>> UpdateProductSize(ProductSizeUpdateDTO productSizeUpdate)
    {
        var productSize = await _context.ProductSizes
                     .Include(ps => ps.Images)
            .Include(ps => ps.Sizes)
            .Include(ps => ps.ProductSizeProductSizeValues)
            .ThenInclude(pspsv => pspsv.ProductSizeValue)
            .Include(ps => ps.CategoryProductSizes)
            .ThenInclude(cps => cps.Category)
            .FirstOrDefaultAsync(ps => ps.Id == productSizeUpdate.Id);

        if (productSize == null)
        {
            return new ServiceResponse<bool>
            {
                Data = false,
                Message = "Product size not found."
            };
        }

        var sizeValueList = new List<ProductSizeProductSizeValue>();

        if (productSizeUpdate.ProductSizeValues != null)
        {
            foreach (var sizeValue in productSizeUpdate.ProductSizeValues)
            {
                var existingSizeValue = await _context.ProductSizeValues
                    .FirstOrDefaultAsync(s => s.Name == sizeValue);

                if (existingSizeValue != null)
                {
                    sizeValueList.Add(new ProductSizeProductSizeValue
                    {
                        ProductSizeId = productSize.Id,
                        ProductSizeValueId = existingSizeValue.Id
                    });
                }
                else
                {
                    var nameValue = new ProductSizeValues
                    {
                        Id = Guid.NewGuid(),
                        Name = sizeValue,
                        Created = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow
                    };

                    _context.ProductSizeValues.Add(nameValue);
                    sizeValueList.Add(new ProductSizeProductSizeValue
                    {
                        ProductSizeId = productSize.Id,
                        ProductSizeValueId = nameValue.Id
                    });
                }
            }
        }

        productSize.SizeType = productSizeUpdate.SizeType;
        productSize.ProductSizeProductSizeValues = sizeValueList;
        productSize.LastUpdated = DateTime.UtcNow;

        if (productSizeUpdate.Thumbnail != null)
        {
            if (productSize.Images is not null)
            {
                _byteFileUtility.DeleteFiles(productSize.Images, nameof(ProductSize));
            }
            productSize.Images = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, ProductSize>>(productSizeUpdate.Thumbnail, nameof(ProductSize), false);
        }
        var existingProductSizeValues = _context.ProductSizeProductSizeValues
          .Where(psv => psv.ProductSizeId == productSize.Id);

        _context.ProductSizeProductSizeValues.RemoveRange(existingProductSizeValues);
        await _context.ProductSizeProductSizeValues.AddRangeAsync(sizeValueList);

        try
        {
            await _unitOfWork.SaveChangesAsync();

            if (productSizeUpdate.CategoryIds != null)
            {
                var existingCategories = await _context.CategoryProductSizes.Where(cps => cps.ProductSizeId == productSize.Id).ToListAsync();
                _context.CategoryProductSizes.RemoveRange(existingCategories);

                var categoryProductSizes = productSizeUpdate.CategoryIds.Select(categoryId => new CategoryProductSize
                {
                    CategoryId = categoryId,
                    ProductSizeId = productSize.Id
                }).ToList();

                await _context.CategoryProductSizes.AddRangeAsync(categoryProductSizes);
                await _unitOfWork.SaveChangesAsync();
            }
        }
        catch (DbUpdateException ex)
        {
            var postgresException = ex.GetBaseException() as PostgresException;
            if (postgresException != null && postgresException.SqlState == "23505")
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Duplicate key value violates unique constraint."
                };
            }
            else
            {
                throw;
            }
        }

        return new ServiceResponse<bool>
        {
            Data = true,
            Message = "اندازه دسته بندی بروز رسانی شد."
        };
    }


    public async Task<ServiceResponse<ProductSizeDTO>> GetProductSizeByCategory(Guid id)
    {
        var productSize = await _context.ProductSizes
            .Include(ps => ps.Images)
            .Include(ps => ps.Sizes)
            .Include(ps => ps.ProductSizeProductSizeValues)
            .ThenInclude(pspsv => pspsv.ProductSizeValue)
            .Include(ps => ps.CategoryProductSizes)
            .ThenInclude(cps => cps.Category)
            .FirstOrDefaultAsync(ps => ps.CategoryProductSizes.Any(cps => cps.CategoryId == id));

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

            ImagesSrc = productSize.Images != null ? _byteFileUtility.GetEncryptedFileActionUrl(productSize.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl ?? string.Empty,
                Placeholder = img.Placeholder ?? string.Empty
            }).ToList(), nameof(ProductSize)).First() : null,
            SizeType = productSize.SizeType,
            ProductSizeValues = productSize.ProductSizeProductSizeValues?.Select(sv => new ProductSizeValuesDTO
            {
                Id = sv.ProductSizeValue.Id,
                Name = sv.ProductSizeValue.Name,
                ProductSizeId = sv.ProductSizeId
            }).ToList(),
            Created = productSize.Created,
            LastUpdated = productSize.LastUpdated
        };

        return new ServiceResponse<ProductSizeDTO>
        {
            Data = productSizeDto
        };
    }

    public async Task<ServiceResponse<ProductSizeDTO>> GetCategoryByProductSize(Guid Id)
    {
        var productSize = await _context.ProductSizes
            .Include(ps => ps.Images)
            .Include(ps => ps.Sizes)
            .Include(ps => ps.ProductSizeProductSizeValues)
            .ThenInclude(pspsv => pspsv.ProductSizeValue)
            .Include(ps => ps.CategoryProductSizes)
            .ThenInclude(cps => cps.Category)
            .FirstOrDefaultAsync(ps => ps.CategoryProductSizes.Any(cps => cps.ProductSizeId == Id));

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

            ImagesSrc = productSize.Images != null ? _byteFileUtility.GetEncryptedFileActionUrl(productSize.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl ?? string.Empty,
                Placeholder = img.Placeholder ?? string.Empty
            }).ToList(), nameof(ProductSize)).First() : null,
            SizeType = productSize.SizeType,
            ProductSizeValues = productSize.ProductSizeProductSizeValues?.Select(sv => new ProductSizeValuesDTO
            {
                Id = sv.ProductSizeValue.Id,
                Name = sv.ProductSizeValue.Name,
                ProductSizeId = sv.ProductSizeId
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
        var size = await _context.Sizes
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

    public async Task<ServiceResponse<Sizes>> GetSizeByCategory(Guid id)
    {
        var size = await _context.Sizes
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

    public async Task<ServiceResponse<Pagination<SizeDTO>>> GetAllSizes(RequestQuery requestQuery)
    {
        var pageNumber = requestQuery.PageNumber ?? 1;
        var pageSize = requestQuery.PageSize ?? 15;

        var sizeQuery = _context.Sizes
                                    .OrderByDescending(f => f.LastUpdated)
                                    .AsNoTracking()
                                    .AsQueryable();

        // Search filter
        if (!string.IsNullOrEmpty(requestQuery.Search))
        {
            string searchLower = requestQuery.Search.ToLower();
            sizeQuery = sizeQuery.Where(f => f.Name.ToLower().Contains(searchLower));
        }

        var totalCount = await sizeQuery.CountAsync();

        // find product == size count 
        var sizesDto = sizeQuery.Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .Select(size => new SizeDTO
                                {
                                    Id = size.Id,
                                    Count = _context.Products.Include(x => x.ProductScale)
                                                                .ThenInclude(x => x.Columns).Count(p => p.ProductScale != null && p.ProductScale.Columns != null && p.ProductScale.Columns.Any(x => x.SizeId == size.Id)),
                                    Description = size.Description,
                                    Name = size.Name,
                                    Created = size.Created,
                                    LastUpdated = size.LastUpdated
                                }).ToList();

        var pagination = new Pagination<SizeDTO>
        {
            CurrentPage = pageNumber,
            NextPage = pageNumber < (totalCount / pageSize) ? pageNumber + 1 : pageNumber,
            PreviousPage = pageNumber > 1 ? pageNumber - 1 : 1,
            HasNextPage = pageNumber < (totalCount / pageSize),
            HasPreviousPage = pageNumber > 1,
            LastPage = (int)Math.Ceiling((double)totalCount / pageSize),
            Data = sizesDto,
            TotalCount = totalCount
        };


        return new ServiceResponse<Pagination<SizeDTO>>
        {
            Count = totalCount,
            Data = pagination,
        };
    }

    public async Task<ServiceResponse<IReadOnlyList<ProductSize>>> GetAllProductSizes()
    {
        var productSizeList = await _context.ProductSizes
                                        .Include(s => s.Images)
                                        .Include(s => s.ProductSizeProductSizeValues)
                                        .ThenInclude(pspsv => pspsv.ProductSizeValue)
                                        .AsNoTracking()
                                        .ToListAsync();

        return new ServiceResponse<IReadOnlyList<ProductSize>>
        {
            Count = productSizeList.Count,
            Data = productSizeList,
        };
    }
}