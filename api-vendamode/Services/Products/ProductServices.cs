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
using api_vendace.Models.Dtos;
using api_vendamode.Models.Dtos.ProductDto.Category;
using api_vendace.Entities;
using api_vendamode.Models.Dtos.ProductDto.Stock;
using api_vendace.Models.Dtos.ProductDto.Brand;

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
            Columns = productCreateDTO.ProductScale?.ColumnSizes?.Select(s => new SizeIds
            {
                Id = Guid.NewGuid(),
                SizeId = Guid.Parse(s.Id),
                Name = s.Name
            }).ToList(),
            Rows = productCreateDTO.ProductScale?.Rows?.Select(r => new SizeModel
            {
                Id = Guid.NewGuid().ToString(),
                Idx = r.Id,
                ProductSizeValueId = r.ProductSizeValueId,
                ProductSizeValue = r.ProductSizeValue,
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
                var featureIds = new List<Guid>();
                if (stockItemDTO.FeatureValueId is not null)
                {
                    foreach (var featureIdStr in stockItemDTO.FeatureValueId)
                    {
                        featureIds.Add(featureIdStr);
                    }
                }
                Guid sizeId2 = Guid.Empty;
                if (stockItemDTO.SizeId is not null)
                {
                    sizeId2 = stockItemDTO.SizeId ?? Guid.Empty;
                }

                return new StockItem
                {
                    Id = stockItemDTO.Id,
                    StockId = stockItemDTO.StockId,
                    Images = stockItemDTO.ImageStock != null ? _byteFileUtility.SaveFileInFolder<EntityImage<Guid, StockItem>>([stockItemDTO.ImageStock], nameof(StockItem), false) : [],
                    Idx = stockItemDTO.Idx ?? string.Empty,
                    ProductId = product.Id,
                    FeatureValueId = featureIds,
                    SizeId = sizeId2 == Guid.Empty ? null : sizeId2,
                    Quantity = stockItemDTO.Quantity,
                    Discount = stockItemDTO.Discount,
                    Price = stockItemDTO.Price,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    AdditionalProperties = stockItemDTO.AdditionalProperties
                };
            }).ToList();

            product.InStock = stockItemsDto.Sum(stockItem => stockItem.Quantity);
            product.StockItems = stockItemsDto;
        }

        await _productRepository.CreateAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true,
            Message = "محصول به ثبت رسید"
        };
    }

    private string GenerateProductCode()
    {
        string prefix = "Km";
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

    public async Task<ServiceResponse<ProductDTO>> GetBy(string slug)
    {
        try
        {
            var product = await _productRepository.GetAsyncBy(slug);
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

    public async Task<ServiceResponse<List<ProductDTO>>> GetByCategory(Guid id)
    {
        try
        {
            int pageNumber = 1;
            int pageSize = 15;

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

            var similarProducts = results.Pagination.Data.Where(c => c.CategoryId == id).ToList();

            return new ServiceResponse<List<ProductDTO>>
            {
                Data = similarProducts,
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<List<ProductDTO>>
            {
                Success = false,
                Message = $"خطایی رخ داد در هنگام دریافت لیست محصولات مشابه با دسته مورد نظر: {ex.Message}"
            };
        }
    }

    private async Task<ProductDTO> BuildProductResponse(Product product)
    {
        var result = product.ToProductResponse(_byteFileUtility);
        if (product.Brand != null)
        {
            result.BrandName = product.Brand!.Name;
            result.BrandData = new BrandDTO
            {
                Id = product.Id,
                Name = product.Brand!.Name,
            };

        }
        var productScaleData = new ProductScaleDTO
        {
            ColumnSizes = product.ProductScale.Columns.Select(c => new SizeIdsDTO
            {
                Id = c.Id.ToString(),
                Name = c.Name
            }).ToList(),
            Rows = product.ProductScale.Rows.Select(row => new SizeModel
            {
                Id = row.Id,
                ProductSizeValue = row.ProductSizeValue,
                Idx = row.Idx,
                ProductSizeValueId = row.ProductSizeValueId,
                ScaleValues = row.ScaleValues

            }).ToList(),
            ProductId = product.ProductScale.ProductId
        };
        result.ProductScale = productScaleData;

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
        var categoryParents = new CategoryWithAllParents
        {
            Category = new CategoryDTO
            {
                Id = product.Category!.Id,
                Name = product.Category.Name,
                Slug = product.Category.Name,
                Level = product.Category.Level,
                IsActive = product.Category.IsActive,
                IsDeleted = product.Category.IsDeleted,
                ParentCategoryId = product.Category.ParentCategoryId,
                Count = product.Category.Products != null ? product.Category.Products.Count : 0,
                FeatureCount = product.Category.ProductFeatures != null ? product.Category.ProductFeatures.Count : 0,
                SizeCount = 0,
                BrandCount = 0,
                Created = product.Category.Created,
                LastUpdated = product.Category.LastUpdated
            },
            ParentCategories = product.Category.GetParentCategories(_context).Select(category => new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Name,
                Level = category.Level,
                IsActive = category.IsActive,
                IsDeleted = category.IsDeleted,
                ParentCategoryId = category.ParentCategoryId,
                Count = category.Products != null ? category.Products.Count : 0,
                FeatureCount = category.ProductFeatures != null ? category.ProductFeatures.Count : 0,
                SizeCount = 0,
                BrandCount = 0,
                Created = category.Created,
                LastUpdated = category.LastUpdated
            }).ToList()
        };
        result.ParentCategories = categoryParents;


        var CategoryProductSizes = await _context.CategoryProductSizes
        .Where(x => x.CategoryId == product.CategoryId)
        .Include(x => x.ProductSize)
        .ThenInclude(s => s.Images)
        .Include(x => x.ProductSize)
        .ThenInclude(s => s.ProductSizeProductSizeValues)
        .ThenInclude(pspsv => pspsv.ProductSizeValue)
        .FirstOrDefaultAsync();

        if (CategoryProductSizes == null)
        {
            return null; // یا هرگونه هندلینگ خطای مناسب دیگری
        }

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

        if (CategoryProductSizes.ProductSize.ProductSizeProductSizeValues?.Any() == true)
        {

            var productSizeInfo = new ProductSizeInfo
            {
                SizeType = CategoryProductSizes.ProductSize.SizeType,
                Columns = sizeList?.Select(s => new SizeDTO
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToList(),

                Rows = productScaleData.Rows,

                ImagesSrc = CategoryProductSizes.ProductSize.Images != null ? _byteFileUtility.GetEncryptedFileActionUrl(CategoryProductSizes.ProductSize.Images.Select(img => new EntityImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl ?? string.Empty,
                    Placeholder = img.Placeholder ?? string.Empty
                }).ToList(), nameof(ProductSize)).First() : null
            };

            result.ProductSizeInfo = productSizeInfo;
        }

        var featureValueIds = product.FeatureValueIds ?? new List<Guid>();


        var featreValueDb = await _context.FeatureValues.ToListAsync();

        var featureValueData = featreValueDb
            .Where(fv => featureValueIds.Contains(fv.Id))
            .ToList();

        // Load featureData and include FeatureValues
        var featureData = await _context.ProductFeatures
            .Include(pf => pf.Values)
            .Where(pf => pf.Values != null ? pf.Values.Any(v => featureValueIds.Contains(v.Id)) : default)
            .ToListAsync();

        // Filter FeatureValues for each ProductFeature
        foreach (var productFeature in featureData)
        {
            productFeature.Values = productFeature.Values?
                .Where(fv => featureValueData.Any(fvd => fvd.Id == fv.Id))
                .ToList();
        }

        result.ProductFeatureInfo = new ProductFeatureInfo(featureData);

        result.StockItems = product.StockItems is not null ? product.StockItems.Select(ps => new GetStockItemDTO
        {
            Id = ps.Id,
            StockId = ps.StockId,
            ProductId = ps.ProductId,
            Idx = ps.Idx,
            ImagesSrc = _byteFileUtility.GetEncryptedFileActionUrl
            (ps.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl!,
                Placeholder = img.Placeholder!
            }).ToList(), nameof(StockItem)),
            Discount = ps.Discount,
            Price = ps.Price,
            FeatureValueId = ps.FeatureValueId,
            Quantity = ps.Quantity,
            SizeId = ps.SizeId,
            AdditionalProperties = ps.AdditionalProperties
        }).ToList() : [];


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

    public async Task<ServiceResponse<bool>> UpdateProduct(ProductUpdateDTO productUpdateDTO)
    {
        var product = await _context.Products.FindAsync(productUpdateDTO.Id);

        if (product == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "محصول مورد نظر یافت نشد"
            };
        }

        if (await _productRepository.GetAsyncBy(productUpdateDTO.Title) != null && productUpdateDTO.Title != product.Title)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "این محصول قبلا به ثبت رسیده"
            };
        }

        // Update basic product properties
        product.Title = productUpdateDTO.Title;
        product.IsActive = productUpdateDTO.IsActive;
        product.CategoryId = productUpdateDTO.CategoryId;
        product.Description = productUpdateDTO.Description;
        product.IsFake = productUpdateDTO.IsFake;
        product.BrandId = productUpdateDTO.BrandId;
        product.LastUpdated = DateTime.UtcNow;

        // Update FeatureValueIds
        product.FeatureValueIds = productUpdateDTO.FeatureValueIds ?? new List<Guid>();

        // Update ProductScale
        if (productUpdateDTO.ProductScale != null)
        {
            var scaleId = product.ProductScaleId;
            product.ProductScale.Columns = productUpdateDTO.ProductScale.ColumnSizes?.Select(s => new SizeIds
            {
                Id = Guid.NewGuid(),
                SizeId = Guid.Parse(s.Id),
                Name = s.Name
            }).ToList();
            product.ProductScale.Rows = productUpdateDTO.ProductScale.Rows?.Select(r => new SizeModel
            {
                Id = Guid.NewGuid().ToString(),
                ProductSizeValueId = r.ProductSizeValueId,
                ProductSizeValue = r.ProductSizeValue,
                ScaleValues = r.ScaleValues
            }).ToList();
            product.ProductScale.LastUpdated = DateTime.UtcNow;
        }

        // Update MainThumbnail
        if (productUpdateDTO.MainThumbnail != null)
        {
            if (product.MainImage != null)
            {
                _byteFileUtility.DeleteFiles([product.MainImage], nameof(Product));
            }
            product.MainImage = _byteFileUtility.SaveFileInFolder<EntityMainImage<Guid, Product>>([productUpdateDTO.MainThumbnail], nameof(Product), true).First();
        }

        // Update Thumbnail images
        if (productUpdateDTO.Thumbnail != null)
        {
            if (product.Images != null)
            {
                _byteFileUtility.DeleteFiles(product.Images, nameof(Product));
            }
            product.Images = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Product>>(productUpdateDTO.Thumbnail, nameof(Product), false);
        }

        // Update StockItems
        if (productUpdateDTO.StockItems != null)
        {
            var stockItemsDto = productUpdateDTO.StockItems.Select(stockItemDTO =>
            {
                var featureIds = new List<Guid>();
                if (stockItemDTO.FeatureValueId != null)
                {
                    foreach (var featureIdStr in stockItemDTO.FeatureValueId)
                    {

                        featureIds.Add(featureIdStr);

                    }
                }
                Guid sizeId2 = Guid.Empty;
                if (stockItemDTO.SizeId != null)
                {
                    sizeId2 = stockItemDTO.SizeId ?? Guid.Empty;
                }

                return new StockItem
                {
                    Id = stockItemDTO.Id,
                    Idx = stockItemDTO.Idx ?? string.Empty,
                    ProductId = product.Id,
                    FeatureValueId = featureIds,
                    SizeId = sizeId2 == Guid.Empty ? null : sizeId2,
                    Quantity = stockItemDTO.Quantity,
                    Discount = stockItemDTO.Discount,
                    Price = stockItemDTO.Price,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
            }).ToList();

            product.InStock = stockItemsDto.Sum(stockItem => stockItem.Quantity);
            product.StockItems = stockItemsDto;
        }

        _context.Products.Update(product);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true,
            Message = "محصول با موفقیت بروزرسانی شد"
        };
    }

}