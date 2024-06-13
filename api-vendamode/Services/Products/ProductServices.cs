using System.Text.RegularExpressions;
using api_vendace.Data;
using api_vendace.Entities.Products;
using api_vendace.Interfaces;
using api_vendace.Interfaces.IRepository;
using api_vendace.Interfaces.IServices;
using api_vendace.Mapper;
using api_vendace.Models;
using api_vendace.Models.Dtos.ProductDto;
using api_vendace.Models.Dtos.ProductDto.Category;
using api_vendace.Models.Dtos.ProductDto.Sizes;
using api_vendace.Models.Dtos.ProductDto.Stock;
using api_vendace.Models.Query;
using api_vendace.Utility;
using api_vendamode.Models.Dtos.ProductDto.Sizes;
using api_vendamode.Interfaces.IServices;
using Microsoft.EntityFrameworkCore;

public class ProductServices : IProductServices
{

    private readonly ApplicationDbContext _context;
    private readonly IProductRepository _productRepository;
    private readonly ICategoryServices _categoryServices;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ByteFileUtility _byteFileUtility;

    public ProductServices(IProductRepository productRepository, IUnitOfWork unitOfWork, ByteFileUtility byteFileUtility, ApplicationDbContext context, ICategoryServices categoryServices)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _byteFileUtility = byteFileUtility;
        _context = context;
        _categoryServices = categoryServices;
    }
    //admin
    public async Task<ServiceResponse<bool>> CreateProduct(ProductCreateDTO productCreateDTO)
    {
        if (_productRepository.GetAsyncBy(productCreateDTO.Title).GetAwaiter().GetResult() != null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "قبلا این محصول به ثبت رسیده"
            };
        }
        var product = productCreateDTO.ToProducts(_byteFileUtility);
        var productId = Guid.NewGuid();

        // load feature
        // Null-checks 
        List<Guid> featureValueIds = productCreateDTO.FeatureValueIds ?? new List<Guid>();
        var scaleId = Guid.NewGuid();
        var productScale = new ProductScale
        {
            Id = scaleId,
            Columns = productCreateDTO.ProductScale?.Columns?.Select(s => new SizeIds
            {
                Id = Guid.NewGuid(),
                SizeId = Guid.Parse(s.Id),
                Name = s.Name
            }).ToList(),
            Rows = productCreateDTO.ProductScale?.Rows?.Select(r => new SizeModel
            {
                Id = Guid.NewGuid().ToString(),
                ProductSizeValueId = r.ProductSizeValueId,
                ProductSizeValueName = r.ProductSizeValueName,
                ScaleValues = r.ScaleValues
            }).ToList(),
            ProductId = productId,
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };
        var productCode = GenerateProductCode();
        var productSlug = GenerateSlug(productCreateDTO.Title, productCode);
        product.Id = productId;
        product.Slug = productSlug;
        product.Code = productCode;
        product.FeatureValueIds = featureValueIds;
        product.ProductScale = productScale;
        product.ProductScaleId = scaleId;
        if (productCreateDTO.StockItems is not null)
        {
            var stockItemsDto = productCreateDTO.StockItems.Select(stockItemDTO =>
            {
                Guid featureId;
                if (!Guid.TryParse(stockItemDTO.FeatureId, out featureId))
                {
                    // Handle the error, e.g., by logging a message or throwing an exception.
                    throw new FormatException($"Invalid FeatureId '{stockItemDTO.FeatureId}'");
                }

                Guid sizeId;
                if (!Guid.TryParse(stockItemDTO.SizeId, out sizeId))
                {
                    // Handle the error, e.g., by logging a message or throwing an exception.
                    throw new FormatException($"Invalid SizeId '{stockItemDTO.SizeId}'");
                }

                return new StockItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    FeatureId = featureId,
                    SizeId = sizeId,
                    Quantity = stockItemDTO.Quantity,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
            }).ToList();

            product.InStock = stockItemsDto.Sum(stockItem => stockItem.Quantity);
        }



        await _productRepository.CreateAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true,
        };
    }

    private string GenerateProductCode()
    {
        string prefix = "K";
        long lastCodeNumber = _productRepository.GetLastProductCodeNumber();
        long newCodeNumber = lastCodeNumber + 1;
        string formattedCodeNumber = newCodeNumber.ToString("D9");
        string newCode = prefix + formattedCodeNumber;
        return newCode;
    }
    public static string GenerateSlug(string title, string code)
    {
        // Convert to lowercase
        string slug = title.ToLowerInvariant();

        // Replace spaces with hyphens
        slug = Regex.Replace(slug, @"\s+", "-");

        // Remove invalid characters
        slug = Regex.Replace(slug, @"[^a-z0-9\u0600-\u06FF-]", "");

        // Trim hyphens from the ends
        slug = slug.Trim('-');

        // Append the product code
        slug = $"{slug}-{code}";

        return slug;
    }
    // admin
    public async Task<ServiceResponse<IEnumerable<Product>>> GetAll()
    {
        var products = await _productRepository.GetAllAsync();
        var count = products.Count();


        return new ServiceResponse<IEnumerable<Product>>
        {
            Count = count,
            Data = products
        };
    }

    public async Task<ServiceResponse<GetProductsResult>> GetProductsPagination(RequestQuery requestQuery)
    {
        try
        {
            int pageNumber = requestQuery.PageNumber ?? 1;
            int pageSize = requestQuery.PageSize ?? 15;

            var paginatedProducts = await _productRepository.GetPaginationAsync(pageNumber, pageSize);
            var productList = paginatedProducts.Data.ToList();
            var productDtoList = new List<ProductDTO>();

            foreach (var product in productList)
            {
                var result = await BuildProductResponse(product);
                productDtoList.Add(result);
            }

            var pagination = new Pagination<ProductDTO>
            {
                CurrentPage = pageNumber,
                NextPage = pageNumber < (paginatedProducts.TotalCount / pageSize) ? pageNumber + 1 : pageNumber,
                PreviousPage = pageNumber > 1 ? pageNumber - 1 : 1,
                HasNextPage = pageNumber < (paginatedProducts.TotalCount / pageSize),
                HasPreviousPage = pageNumber > 1,
                LastPage = (int)Math.Ceiling((double)paginatedProducts.TotalCount / pageSize),
                Data = productDtoList,
                TotalCount = paginatedProducts.TotalCount
            };

            var results = new GetProductsResult
            {
                ProductsLength = paginatedProducts.TotalCount,
                MainMaxPrice = paginatedProducts.Data.Max(p => p.Price),
                MainMinPrice = paginatedProducts.Data.Min(p => p.Price),
                Pagination = pagination
            };

            return new ServiceResponse<GetProductsResult>
            {
                Data = results,
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<GetProductsResult>
            {
                Success = false,
                Message = $"خطایی رخ داد در هنگام دریافت لیست محصولات: {ex.Message}"
            };
        }
    }

    public async Task<ServiceResponse<GetProductsResult>> GetProducts(RequestQuery requestQuery)
    {
        try
        {
            int pageNumber = requestQuery.PageNumber ?? 1;
            int pageSize = requestQuery.PageSize ?? 15;

            // Initial query
            var query = _productRepository.GetQuery();

            // Category filter
            if (!string.IsNullOrEmpty(requestQuery.Category))
            {
                var categoryResponse = await _categoryServices.GetBySlugAsync(requestQuery.Category);
                if (categoryResponse.Success && categoryResponse.Data != null)
                {
                    var category = categoryResponse.Data;
                    var categoryIds = new List<Guid> { category.Id };
                    categoryIds.AddRange(GetAllSubCategoryIds(category));

                    query = query.Where(p => categoryIds.Contains(p.CategoryId));
                }
            }

            // Search filter
            if (!string.IsNullOrEmpty(requestQuery.Search))
            {
                query = query.Where(p => p.Title.Contains(requestQuery.Search));
            }

            // Price filter
            if (requestQuery.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= requestQuery.MinPrice.Value);
            }

            if (requestQuery.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= requestQuery.MaxPrice.Value);
            }

            // InStock filter
            if (requestQuery.InStock.HasValue && requestQuery.InStock.Value)
            {
                query = query.Where(p => p.InStock >= 1);
            }

            // Discount filter
            if (requestQuery.Discount.HasValue && requestQuery.Discount.Value)
            {
                query = query.Where(p => p.Discount >= 1 && p.InStock >= 1);
            }

            // Feature filters
            if ((requestQuery.FeatureIds?.Any() ?? false) && (requestQuery.FeatureValueIds?.Any() ?? false))
            {
                var featureFilters = requestQuery.FeatureIds.Concat(requestQuery.FeatureValueIds).ToList();
                foreach (var featureFilter in featureFilters)
                {
                    query = query.Where(p => p.ProductFeatures != null && p.ProductFeatures
                        .Any(f => f.Id == featureFilter && f.Values != null && f.Values.Any(v => v.Id == featureFilter)));
                }
            }

            // Sort
            if (!string.IsNullOrEmpty(requestQuery.SortBy))
            {
                if (requestQuery.Sort?.ToLower() == "desc")
                {
                    query = query.OrderByDescending(p => EF.Property<object>(p, requestQuery.SortBy));
                }
                else
                {
                    query = query.OrderBy(p => EF.Property<object>(p, requestQuery.SortBy));
                }
            }

            // Pagination and data retrieval
            var totalCount = await query.CountAsync();
            var paginatedProducts = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var productDtoList = new List<ProductDTO>();
            foreach (var product in paginatedProducts)
            {
                var result = await BuildProductResponse(product);
                productDtoList.Add(result);
            }

            // Calculate min and max price for the filtered products in stock
            var mainMaxPrice = await query.Where(p => p.InStock >= 1).MaxAsync(p => (double?)p.Price) ?? 0;
            var mainMinPrice = await query.Where(p => p.InStock >= 1).MinAsync(p => (double?)p.Price) ?? 0;

            // Prepare pagination information
            var pagination = new Pagination<ProductDTO>
            {
                CurrentPage = pageNumber,
                NextPage = pageNumber < (totalCount / pageSize) ? pageNumber + 1 : pageNumber,
                PreviousPage = pageNumber > 1 ? pageNumber - 1 : 1,
                HasNextPage = pageNumber < (totalCount / pageSize),
                HasPreviousPage = pageNumber > 1,
                LastPage = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = productDtoList,
                TotalCount = totalCount
            };

            // Prepare the result
            var results = new GetProductsResult
            {
                ProductsLength = totalCount,
                MainMaxPrice = mainMaxPrice,
                MainMinPrice = mainMinPrice,
                Pagination = pagination
            };

            // Return the successful response
            return new ServiceResponse<GetProductsResult>
            {
                Data = results,
                Success = true
            };
        }
        catch (Exception)
        {
            // Return the error response
            return new ServiceResponse<GetProductsResult>
            {
                Success = false,
                Message = "An error occurred while processing your request."
            };
        }
    }

    private List<Guid> GetAllSubCategoryIds(CategoryDTO category)
    {
        var categoryIds = new List<Guid> { category.Id };

        if (category.Categories != null && category.Categories.Any())
        {
            foreach (var subCategory in category.Categories)
            {
                categoryIds.AddRange(GetAllSubCategoryIds(subCategory));
            }
        }

        return categoryIds;
    }


    // public async Task<ServiceResponse<GetProductsResult>> GetProducts(RequestQuery requestQuery)
    // {
    //     try
    //     {
    //         int pageNumber = requestQuery.PageNumber ?? 1;
    //         int pageSize = requestQuery.PageSize;

    //         // Apply filtering logic based on the query parameters
    //         var productPaginationList = await _productRepository.GetPaginationAsync(pageNumber, pageSize);
    //         var query = _productRepository.GetQuery();
    //         if (!string.IsNullOrEmpty(requestQuery.Category))
    //         {
    //             query = query.Where(p => p.Category!.Name == requestQuery.Category);
    //         }

    //         if (requestQuery.MinPrice.HasValue)
    //         {
    //             query = query.Where(p => p.Price >= requestQuery.MinPrice.Value);
    //         }

    //         if (requestQuery.MaxPrice.HasValue)
    //         {
    //             query = query.Where(p => p.Price <= requestQuery.MaxPrice.Value);
    //         }

    //         if ((requestQuery.FeatureIds?.Any() ?? false) && (requestQuery.FeatureValueIds?.Any() ?? false))
    //         {
    //             var featureFilters = requestQuery.FeatureIds.Concat(requestQuery.FeatureValueIds).ToList();
    //             foreach (var featureFilter in featureFilters)
    //             {
    //                 query = query.Where(p => p.ProductFeatures != null && p.ProductFeatures
    //                     .Any(f => f.Id == featureFilter && f.Values != null && f.Values.Any(v => v.Id == featureFilter)));
    //             }
    //         }

    //         if (!string.IsNullOrEmpty(requestQuery.SortBy))
    //         {
    //             if (requestQuery.Sort?.ToLower() == "desc")
    //             {
    //                 query = query.OrderByDescending(p => EF.Property<object>(p, requestQuery.SortBy));
    //             }
    //             else
    //             {
    //                 query = query.OrderBy(p => EF.Property<object>(p, requestQuery.SortBy));
    //             }
    //         }

    //         var totalCount = await query.CountAsync();
    //         var paginatedProducts = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

    //         var productDtoList = new List<ProductDTO>();

    //         foreach (var product in paginatedProducts)
    //         {
    //             var result = await BuildProductResponse(product);
    //             productDtoList.Add(result);
    //         }

    //         var pagination = new Pagination<ProductDTO>
    //         {
    //             CurrentPage = pageNumber,
    //             NextPage = pageNumber < (totalCount / pageSize) ? pageNumber + 1 : pageNumber,
    //             PreviousPage = pageNumber > 1 ? pageNumber - 1 : 1,
    //             HasNextPage = pageNumber < (totalCount / pageSize),
    //             HasPreviousPage = pageNumber > 1,
    //             LastPage = (int)Math.Ceiling((double)totalCount / pageSize),
    //             Data = productDtoList,
    //             TotalCount = totalCount
    //         };

    //         var results = new GetProductsResult
    //         {
    //             ProductsLength = totalCount,
    //             MainMaxPrice = paginatedProducts.Max(p => p.Price),
    //             MainMinPrice = paginatedProducts.Min(p => p.Price),
    //             Pagination = pagination
    //         };

    //         return new ServiceResponse<GetProductsResult>
    //         {
    //             Data = results,
    //             Success = true
    //         };
    //     }
    //     catch (Exception)
    //     {
    //         return new ServiceResponse<GetProductsResult>
    //         {
    //             Success = false,
    //             Message = "An error occurred while processing your request."
    //         };
    //     }
    // }

    public async Task<ServiceResponse<ProductDTO>> GetSingleProductBy(Guid id)
    {
        try
        {
            var product = await _productRepository.GetAsyncBy(id);
            if (product == null)
            {
                return new ServiceResponse<ProductDTO>
                {
                    Success = false,
                    Message = "محصولی پیدا نشد."
                };
            }

            var result = await BuildProductResponse(product);
            return new ServiceResponse<ProductDTO> { Data = result };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<ProductDTO>
            {
                Success = false,
                Message = "خطایی رخ داد در هنگام دریافت محصول" + ex.Message
            };
        }
    }

    public Task<ServiceResponse<bool>> UpdateProduct(Guid id)
    {
        throw new NotImplementedException();
    }

    private async Task<ProductDTO> BuildProductResponse(Product product)
    {
        var result = product.ToProductResponse(_byteFileUtility);

        var categoryLevelIds = GetCategoryLevelIds(product.Category!);
        var categories = await _context.Categories
            .Where(c => categoryLevelIds.Contains(c.Id))
            .ToListAsync();

        result.CategoryLevels = categories.Select(c => new CategoryLevels
        {
            Id = c.Id,
            Name = c.Name,
            Level = c.Level,
            Url = c.Name.ToLower()
        }).ToList();
        result.CategoryList = categoryLevelIds;

        var productSize = await _context.ProductSizes
            .Where(x => x.CategoryId == product.CategoryId)
            .Include(s => s.Images)
            .Include(s => s.Sizes)
            .Include(s => s.ProductSizeValues)
            .FirstOrDefaultAsync();

        var productScale = await _context.ProductScales.Include(x => x.Columns).Include(x => x.Rows).FirstOrDefaultAsync(p => p.ProductId == product.Id);
        List<Sizes>? sizeList = null;
        if (productScale?.Columns?.Any() == true)
        {
            var sizeIds = productScale.Columns.Select(c => c.SizeId).ToList();
            sizeList = await _context.Sizes
                .Where(x => sizeIds.Contains(x.Id))
                .ToListAsync();
        }

        var scaleValues = new List<string>();
        if (productScale?.Rows?.Any() == true)
        {
            foreach (var row in productScale.Rows)
            {
                if (row.ScaleValues?.Any() == true)
                {
                    scaleValues.AddRange(row.ScaleValues);
                }
            }
        }

        if (productSize?.ProductSizeValues?.Any() == true)
        {
            var productSizeInfo = new ProductSizeInfo
            {
                Columns = sizeList?.Select(s => new SizeDTO
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToList(),

                Rows = productSize.ProductSizeValues.Select(ps => new SizeInfoModel
                {
                    ProductSizeValue = ps.Name,
                    ScaleValues = scaleValues
                }).ToList()
            };
            result.ProductSizeInfo = productSizeInfo;
        }

        result.ProductFeatureInfo = new ProductFeatureInfo(product);

        double sumOfRatings = 0;
        if (product.Review != null && product.Review.Count > 0)
        {
            sumOfRatings = product.Review.Sum(review => review.Rating);
            result.Rating = sumOfRatings / product.Review.Count;
        }
        else
        {
            result.Rating = 0;
        }

        result.ReviewCount = product.Review?.Count;
        result.InStock = product.InStock;

        return result;
    }


    private List<Guid?> GetCategoryLevelIds(Category category)
    {
        var categoryLevelIds = new List<Guid?>();

        if (category?.Level == 1)
        {
            categoryLevelIds.Add(category.Id);
        }
        else if (category?.Level == 2)
        {
            categoryLevelIds.Add(category.ParentCategoryId);
            categoryLevelIds.Add(category.Id);
        }
        else
        {
            categoryLevelIds.Add(category?.ParentCategory?.ParentCategoryId);
            categoryLevelIds.Add(category?.ParentCategoryId);
            categoryLevelIds.Add(category?.Id);
        }

        return categoryLevelIds;
    }

}