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
using api_vendamode.Entities.Products;
using api_vendamode.Models.Dtos;
using System.Globalization;
public class ProductServices : IProductServices
{

    private readonly ApplicationDbContext _context;
    private readonly IProductRepository _productRepository;
    private readonly ICategoryServices _categoryServices;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ByteFileUtility _byteFileUtility;

    private readonly IUserServices _userServices;

    public ProductServices(IProductRepository productRepository, IUnitOfWork unitOfWork, ByteFileUtility byteFileUtility, ApplicationDbContext context, ICategoryServices categoryServices, IUserServices userServices)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _byteFileUtility = byteFileUtility;
        _context = context;
        _categoryServices = categoryServices;
        _userServices = userServices;
    }
    public async Task<ServiceResponse<bool>> BulkUpdateProductStatus(List<Guid> productIds, string action)
    {
        var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
        if (action == "1")
        {
            foreach (var product in products)
            {
                product.IsActive = true;
                product.IsDeleted = false;
            }
        }
        else if (action == "2")
        {
            foreach (var product in products)
            {
                product.IsActive = false;
            }
        }
        else if (action == "3")
        {
            foreach (var product in products)
            {
                product.IsActive = false;
                product.IsDeleted = true;
            }
        }
        else if (action == "4")
        {
            foreach (var product in products)
            {
                product.IsDeleted = false;
            }
        }
        else if (action == "5")
        {
            // foreach (var product in products)
            // {
            //     product.IsDeleted = false;
            // }
            _context.Products.RemoveRange(products);

            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            foreach (var product in products)
            {
                product.IsDeleted = false;
            }
        }


        _context.Products.UpdateRange(products);
        var result = await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool>
        {
            Data = result > 0
        };
    }
    //admin
    public async Task<ServiceResponse<Guid>> CreateProduct(ProductCreateDTO productCreateDTO)
    {
        var productId = Guid.NewGuid();
        var productCode = GenerateProductCode();
        var productSlug = GenerateSlug(productCreateDTO.Title, productCode);
        var product = productCreateDTO.ToProducts(_byteFileUtility, productId, productCode);
        var userId = _userServices.GetUserId();
        var userInfo = await _context.Users.Include(us => us.UserSpecification).FirstOrDefaultAsync(u => u.Id == userId);

        // load feature
        // Null-checks 
        List<Guid> featureValueIds = productCreateDTO.FeatureValueIds ?? new List<Guid>();
        // Retrieve ProductFeatures from database based on FeatureValueIds
        var productFeatures = await _context.ProductFeatures
            .Include(pf => pf.Values)
            .Where(pf => pf.Values.Any(fv => featureValueIds.Contains(fv.Id)))
            .ToListAsync();

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
        product.Id = productId;
        product.Slug = productSlug;
        product.Author = userInfo?.UserSpecification.FirstName + " " + userInfo?.UserSpecification.FamilyName;
        product.Code = productCode;
        product.FeatureValueIds = featureValueIds;
        product.ProductFeatures = productFeatures;
        product.ProductScale = productScale;
        product.ProductScaleId = scaleId;
        product.ParsedDate = productCreateDTO.ParsedDate ?? DateTime.UtcNow;
        product.Date = productCreateDTO.Date ?? "";
        product.PublishTime = product.ParsedDate.HasValue && product.ParsedDate.Value.ToLocalTime() > DateTimeOffset.Now.ToLocalTime();

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
                    IsHidden = stockItemDTO.IsHidden,
                    Images = stockItemDTO.ImageStock != null ? _byteFileUtility.SaveFileInFolder<EntityImage<Guid, StockItem>>([stockItemDTO.ImageStock], nameof(Product), productCode, false) : [],
                    Idx = stockItemDTO.Idx ?? string.Empty,
                    ProductId = product.Id,
                    FeatureValueId = featureIds,
                    SizeId = sizeId2 == Guid.Empty ? null : sizeId2,
                    Quantity = stockItemDTO.Quantity,
                    Discount = stockItemDTO.Discount,
                    OfferStartTime = DateTime.UtcNow,
                    OfferEndTime = stockItemDTO.OfferTime.HasValue ? DateTime.UtcNow.AddHours(stockItemDTO.OfferTime.Value) : null,
                    OfferTime = stockItemDTO.OfferTime,
                    Price = stockItemDTO.Price,
                    PurchasePrice = stockItemDTO.PurchasePrice,
                    Weight = stockItemDTO.Weight,
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

        return new ServiceResponse<Guid>
        {
            Data = product.Id,
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

            // Admin LIst 
            if (requestQuery.AdminList is not null)
            {
                query = query.Where(x => !x.IsDeleted);
            }

            // Deleted Filter
            if (requestQuery.IsDeleted is not null)
            {
                query = query.Where(x => x.IsDeleted);
            }

            // isActive Filter
            if (requestQuery.IsActive is not null)
            {
                query = query.Where(x => x.IsActive);
            }

            // isActive Filter
            if (requestQuery.InActive is not null)
            {
                query = query.Where(x => !x.IsActive && !x.IsDeleted);
            }
            // isPending Filter
            if (requestQuery.IsPublishTime is not null)
            {
                if (requestQuery.IsPublishTime == true)
                {
                    query = query.Where(x => x.ParsedDate.HasValue && x.ParsedDate.Value > DateTimeOffset.UtcNow);
                }
                else
                {
                    query = query.Where(x => !x.ParsedDate.HasValue || x.ParsedDate.Value <= DateTimeOffset.UtcNow);
                }
            }


            // Brand filter
            if (requestQuery.Brands != null && requestQuery.Brands.Length > 0)
            {
                // Split each brand string by comma and convert to GUIDs safely
                var brandGuids = new List<Guid>();
                foreach (var brand in requestQuery.Brands)
                {
                    var brandIds = brand.Split(',');
                    foreach (var brandId in brandIds)
                    {
                        if (Guid.TryParse(brandId, out var parsedGuid))
                        {
                            brandGuids.Add(parsedGuid);
                        }
                    }
                }

                // Apply the brand filter if there are valid GUIDs
                if (brandGuids.Count > 0)
                {
                    query = query.Where(product => product.BrandId.HasValue && brandGuids.Contains(product.BrandId.Value));
                }
            }

            // Best selling product
            if (requestQuery.IsBestSeller is not null)
            {
                query = query.Where(x => x.Sold > 0).OrderBy(x => x.Sold);
            }

            // Category filter
            if (requestQuery.CategorySlug is not null && requestQuery.SingleCategory is not null)
            {
                Guid categoryId;
                if (Guid.TryParse(requestQuery.CategorySlug, out categoryId))
                {
                    var allCategoryIds = _categoryServices.GetAllCategoryIds(categoryId);

                    query = query.Where(p => allCategoryIds.Contains(p.CategoryId));
                }
            }

            if (requestQuery.CategoryId is not null && requestQuery.SingleCategory is not null)
            {
                Guid categoryId;
                if (Guid.TryParse(requestQuery.CategoryId, out categoryId))
                {
                    var allCategoryIds = _categoryServices.GetAllCategoryIds(categoryId);

                    query = query.Where(p => allCategoryIds.Contains(p.CategoryId));
                }
            }

            if (!string.IsNullOrEmpty(requestQuery.Category) && requestQuery.SingleCategory is not null)
            {
                var allCategoryIds = _categoryServices.GetAllCategoryIdsBy(requestQuery.Category);

                query = query.Where(p => allCategoryIds.Contains(p.CategoryId));
            }

            if (requestQuery.CategorySlug is not null && requestQuery.SingleCategory is null)
            {
                var allCategoryIds = _categoryServices.GetAllCategoryIdsBy(requestQuery.CategorySlug);
                query = query.Where(p => allCategoryIds.Contains(p.CategoryId));
            }

            // if (requestQuery.CategoryId is not null && requestQuery.SingleCategory is null)
            // {

            //     if (requestQuery.CategoryId != "default") query = query.Where(p => p.CategoryId == Guid.Parse(requestQuery.CategoryId));
            // }

            // Search filter
            if (!string.IsNullOrEmpty(requestQuery.Search))
            {
                string searchLower = requestQuery.Search.ToLower();
                query = query.Where(p => p.Slug.ToLower().Contains(searchLower));
            }

            // Price filter
            if (requestQuery.MinPrice.HasValue || requestQuery.MaxPrice.HasValue)
            {
                query = query.Where(p => p.StockItems.Any(si =>
                    (!requestQuery.MinPrice.HasValue || si.Price >= requestQuery.MinPrice.Value) &&
                    (!requestQuery.MaxPrice.HasValue || si.Price <= requestQuery.MaxPrice.Value)));
            }


            // Discount and stock filter using StockItem
            if (requestQuery.Discount.HasValue && requestQuery.Discount.Value)
            {
                query = query.Where(p => p.StockItems.Any(si => si.Discount >= 1) && p.InStock >= 1).OrderByDescending(p => p.StockItems.Any(si => si.Quantity > 0 && si.Price > 0)) // Prioritize products with stock and price
                            .ThenBy(p => p.LastUpdated);;
            }

            // Product filters based on ProductIds
            if (requestQuery.ProductIds != null && requestQuery.ProductIds.Length > 0)
            {
                // Split each brand string by comma and convert to GUIDs safely
                var productIdGuids = new List<Guid>();
                foreach (var product in requestQuery.ProductIds)
                {
                    var productIds = product.Split(',');
                    foreach (var productId in productIds)
                    {
                        if (Guid.TryParse(productId, out var parsedGuid))
                        {
                            productIdGuids.Add(parsedGuid);
                        }
                    }
                }

                // Apply the Product filter if there are valid GUIDs
                if (productIdGuids.Count > 0)
                {
                    query = query.Where(product => productIdGuids.Contains(product.Id));
                }
            }


            // Feature filters based on featureIds
            if (requestQuery.FeatureIds?.Any() ?? false)
            {
                foreach (var featureId in requestQuery.FeatureIds)
                {
                    query = query.Where(p => p.ProductFeatures.Any(f => f.Id == featureId));
                }
            }

            // Feature filters based on featureValueIds
            if (requestQuery.FeatureValueIds?.Any() ?? false)
            {
                foreach (var featureValueId in requestQuery.FeatureValueIds)
                {
                    query = query.Where(p => p.FeatureValueIds != null && p.FeatureValueIds.Any(x => x == featureValueId));
                }
            }


            // Size filters based on featureIds
            if (requestQuery.SizeIds?.Any() ?? false)
            {
                foreach (var sizeId in requestQuery.SizeIds)
                {
                    query = query.Where(p => p.ProductScale != null && p.ProductScale.Columns != null && p.ProductScale.Columns.Any(f => f.SizeId == sizeId));
                }
            }

            // SortBy
            if (!string.IsNullOrEmpty(requestQuery.SortBy))
            {
                if (requestQuery.SortBy == "LastUpdated" && requestQuery.Sort?.ToLower() == "desc")
                {
                    query = query
                            .OrderByDescending(p => p.StockItems.Any(si => si.Quantity > 0 && si.Price > 0)) // Prioritize products with stock and price
                            .ThenBy(p => p.LastUpdated);
                }
                else if (requestQuery.SortBy == "LastUpdated" && requestQuery.Sort?.ToLower() != "desc")
                {
                    query = query.OrderByDescending(p => EF.Property<object>(p, requestQuery.SortBy));
                }
                else
                {
                    // query = query.OrderBy(p => EF.Property<object>(p, requestQuery.SortBy));
                }
            }
            // Sort
            if (!string.IsNullOrEmpty(requestQuery.Sort))
            {
                switch (requestQuery.Sort)
                {
                    case "1": // Sort by latest with stock and price prioritized
                        query = query
                            .OrderByDescending(p => p.StockItems.Any(si => si.Quantity > 0 && si.Price > 0)) // Prioritize products with stock and price
                            .ThenByDescending(p => p.LastUpdated); // Sort by latest
                        break;

                    case "2": // Sort by best-selling
                        query = query
                        .OrderByDescending(p => p.StockItems.Any(si => si.Quantity > 0 && si.Price > 0)).ThenByDescending(p => p.Sold);
                        break;

                    case "3": // Sort by cheapest
                        query = query
                        .OrderByDescending(p => p.StockItems.Any(si => si.Quantity > 0 && si.Price > 0)).ThenBy(p => p.StockItems.Min(si => si.Price));
                        break;

                    case "4": // Sort by most expensive
                        query = query
                        .OrderByDescending(p => p.StockItems.Any(si => si.Quantity > 0 && si.Price > 0)).ThenByDescending(p => p.StockItems.Max(si => si.Price));
                        break;

                    default:
                        // query = query
                        // .OrderByDescending(p => p.StockItems.Any(si => si.Quantity > 0 && si.Price > 0)).ThenByDescending(p => p.Created);
                        break;
                }
            }
            // Order by existing and non-existent
            // if (!requestQuery.Admin.HasValue)
            // {
            //     query = query
            //     .OrderBy(p => p.StockItems.Any(si => si.Quantity > 0 && si.Price > 0) ? 0 : 1)
            //     .ThenBy(p => p.StockItems.Any(si => si.Quantity > 0) ? 0 : 1);
            // }
            if (requestQuery.InStock is not null)
            {
                if (requestQuery.InStock == "1")
                {
                    query = query.Where(x => x.InStock > 0 && x.StockItems.Any(x => x.Price > 0));
                }
                else if (requestQuery.InStock == "true")
                {
                    query = query.Where(x => x.InStock > 0 && x.StockItems.Any(xs => xs.Price > 0));
                }
                else
                {
                    query = query.Where(x => x.InStock == 0);
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
            var stockItemsQuery = query
                // .Where(p => p.InStock >= 1)
                .SelectMany(p => p.StockItems);

            var mainMaxPrice = await stockItemsQuery.MaxAsync(si => (double?)si.Price) ?? 0;
            var mainMinPrice = await stockItemsQuery.MinAsync(si => (double?)si.Price) ?? 0;


            if (requestQuery.CategoryId is not null && requestQuery.CategoryId != "default")
            {
                var productDtoListByParentCategory = productDtoList.Where(c => c.ParentCategories.ParentCategories.Any(c => c.Id == Guid.Parse(requestQuery.CategoryId)))
                             .ToList();
                if (productDtoListByParentCategory.Count == 0)
                {
                    productDtoList = productDtoList
                    .Where(c => c.ParentCategories.Category.Id == Guid.Parse(requestQuery.CategoryId))
                    .ToList();
                }
                else
                {
                    productDtoList = productDtoList.Where(c => c.ParentCategories.ParentCategories.Any(c => c.Id == Guid.Parse(requestQuery.CategoryId)))
               .ToList();
                }
            }

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
        catch (Exception ex)
        {
            System.Console.WriteLine(ex);
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

            var similarProducts = results.Pagination.Data
            .Where(c => c.CategoryId == id)
            .ToList();

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
    public static string ConvertDateTimeOffsetToPersian(DateTimeOffset dateTimeOffset)
    {
        // System.Console.WriteLine(dateTimeOffset , "dateTimeOffset");
        PersianCalendar persianCalendar = new PersianCalendar();
        int persianYear = persianCalendar.GetYear(dateTimeOffset.DateTime);
        int persianMonth = persianCalendar.GetMonth(dateTimeOffset.DateTime);
        int persianDay = persianCalendar.GetDayOfMonth(dateTimeOffset.DateTime);
        int hour = dateTimeOffset.Hour;
        int minute = dateTimeOffset.Minute;
        int second = dateTimeOffset.Second;

        // تبدیل اعداد به فارسی
        string persianDate = $"{ConvertToPersianNumbers(persianYear)}/{ConvertToPersianNumbers(persianMonth)}/{ConvertToPersianNumbers(persianDay)}";
        string persianTime = $"{ConvertToPersianNumbers(hour)}:{ConvertToPersianNumbers(minute)}:{ConvertToPersianNumbers(second)}";

        // ترکیب تاریخ و زمان
        return $"{persianTime} - {persianDate}";
    }

    public static string ConvertToPersianNumbers(int number)
    {
        string[] persianNumbers = new string[] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
        string[] englishNumbers = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        string numberString = number.ToString();
        for (int i = 0; i < englishNumbers.Length; i++)
        {
            numberString = numberString.Replace(englishNumbers[i], persianNumbers[i]);
        }

        return numberString;
    }

    private async Task<ProductDTO> BuildProductResponse(Product product)
    {
        var result = product.ToProductResponse(_byteFileUtility);
        if (product.Brand != null)
        {
            result.BrandName = product.Brand!.NameFa;
            result.BrandData = new BrandDTO
            {
                Id = product.Id,
                NameFa = product.Brand!.NameFa,
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

        if (product != null && product.ParsedDate.HasValue)
        {
            product.Date = ConvertDateTimeOffsetToPersian(product.ParsedDate.Value.ToLocalTime());
        }
        result.Date = product.Date;
        result.ParsedDate = product.ParsedDate;
        result.PublishTime = product.ParsedDate.HasValue && product.ParsedDate.Value.ToLocalTime() > DateTimeOffset.Now.ToLocalTime();

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
                Slug = product.Category.Slug,
                Level = product.Category.Level,
                IsActive = product.Category.IsActive,
                IsDeleted = product.Category.IsDeleted,
                ParentCategoryId = product.Category.ParentCategoryId,
                Count = product.Category.Products != null ? product.Category.Products.Count : 0,
                FeatureCount = product.Category.CategoryProductFeatures != null ? product.Category.CategoryProductFeatures.Count : 0,
                SizeCount = 0,
                BrandCount = 0,
                Created = product.Category.Created,
                LastUpdated = product.Category.LastUpdated
            },
            ParentCategories = product.Category.GetParentCategories(_context).Select(category => new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                Level = category.Level,
                IsActive = category.IsActive,
                IsDeleted = category.IsDeleted,
                ParentCategoryId = category.ParentCategoryId,
                Count = category.Products != null ? category.Products.Count : 0,
                FeatureCount = category.CategoryProductFeatures != null ? category.CategoryProductFeatures.Count : 0,
                SizeCount = 0,
                BrandCount = 0,
                Created = category.Created,
                LastUpdated = category.LastUpdated
            }).Reverse().ToList()
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

        if (CategoryProductSizes is not null && CategoryProductSizes.ProductSize.ProductSizeProductSizeValues?.Any() == true)
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
                }).ToList(), nameof(ProductSize), "SubProductSize").First() : null
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

        if (product.StockItems is not null)
        {
            result.StockItems = product.StockItems.Select(ps =>
            {
                string? discountRemainingTime = null;
                if (ps.OfferEndTime.HasValue && ps.OfferEndTime.Value > DateTime.UtcNow)
                {
                    var remainingTime = ps.OfferEndTime.Value - DateTime.UtcNow;

                    var totalHours = (int)remainingTime.TotalHours;
                    var minutes = remainingTime.Minutes;
                    var seconds = remainingTime.Seconds;

                    discountRemainingTime = $"{totalHours:D2}:{minutes:D2}:{seconds:D2}";
                }
                return new GetStockItemDTO
                {
                    Id = ps.Id,
                    StockId = ps.StockId,
                    IsHidden = ps.IsHidden,
                    ProductId = ps.ProductId,
                    Idx = ps.Idx,
                    ImagesSrc = _byteFileUtility.GetEncryptedFileActionUrl(ps.Images.Select(img => new EntityImageDto
                    {
                        Id = img.Id,
                        ImageUrl = img.ImageUrl!,
                        Placeholder = img.Placeholder!
                    }).ToList(), nameof(Product), result.Code),
                    Discount = ps.Discount,
                    OfferTime = ps.OfferTime,
                    Price = ps.Price,
                    PurchasePrice = ps.PurchasePrice,
                    Weight = ps.Weight,
                    FeatureValueId = ps.FeatureValueId,
                    Quantity = ps.Quantity,
                    SizeId = ps.SizeId,
                    AdditionalProperties = ps.AdditionalProperties,
                    DiscountRemainingTime = discountRemainingTime,
                    Created = ps.CreatedAt,
                    LastUpdated = ps.UpdatedAt
                };
            }).ToList();
        }


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

    public async Task<ServiceResponse<Guid>> UpdateProduct(ProductUpdateDTO productUpdateDTO)
    {
        var userId = _userServices.GetUserId();
        var userInfo = await _context.Users.Include(us => us.UserSpecification).FirstOrDefaultAsync(u => u.Id == userId);
        var product = await _context.Products
                            .Include(x => x.Brand)
                            .Include(x => x.Images)
                            .Include(x => x.MainImage)
                            .Include(x => x.ProductFeatures)
                            .Include(x => x.ProductScale)
                            .ThenInclude(x => x.Columns)
                            .Include(x => x.ProductScale)
                            .ThenInclude(x => x.Rows)
                            .Include(x => x.Review)
                            .Include(c => c.Category)
                            .Include(c => c.StockItems)
                            .ThenInclude(x => x.Images)
                            .FirstOrDefaultAsync(u => u.Id == productUpdateDTO.Id);

        if (product == null)
        {
            return new ServiceResponse<Guid>
            {
                Success = false,
                Message = "محصول مورد نظر یافت نشد"
            };
        }
        // Remove the existing product
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        var productId = Guid.NewGuid();
        var newProduct = new Product
        {
            Id = productId,
            Title = productUpdateDTO.Title,
            StockTag = productUpdateDTO.StockTag,
            Code = product.Code,
            Slug = product.Slug,
            Author = userInfo?.UserSpecification.FirstName + " " + userInfo?.UserSpecification.FamilyName,
            IsActive = productUpdateDTO.IsActive,
            CategoryId = productUpdateDTO.CategoryId,
            Status = productUpdateDTO.Status,
            Description = productUpdateDTO.Description,
            IsFake = productUpdateDTO.IsFake,
            BrandId = productUpdateDTO.BrandId,
            Created = product.Created,
            LastUpdated = DateTime.UtcNow
        };

        List<Guid> featureValueIds = productUpdateDTO.FeatureValueIds ?? new List<Guid>();
        newProduct.Date = productUpdateDTO.Date;
        newProduct.ParsedDate = productUpdateDTO.ParsedDate;
        newProduct.PublishTime = product.ParsedDate.HasValue && product.ParsedDate.Value.ToLocalTime() > DateTimeOffset.Now.ToLocalTime();
        if (product.ProductScale is not null)
        {
            var scaleId = product.ProductScale.Id;
            var newScaleId = product.ProductScale.Id;
            // Find the related SizeIds and SizeModel entries
            var sizeIdsToRemove = _context.SizeIds.Where(s => s.ProductScaleId == scaleId).ToList();
            var sizeModelsToRemove = _context.SizeModels.Where(m => m.ProductScaleId == scaleId).ToList();
            // Remove the related SizeIds and SizeModel entries
            _context.SizeIds.RemoveRange(sizeIdsToRemove);
            _context.SizeModels.RemoveRange(sizeModelsToRemove);
            _context.ProductScales.Remove(product.ProductScale);
            await _context.SaveChangesAsync();

            var productScale = new ProductScale
            {
                Id = newScaleId,
                Columns = productUpdateDTO.ProductScale?.ColumnSizes?.Select(s => new SizeIds
                {
                    Id = Guid.NewGuid(),
                    SizeId = Guid.Parse(s.Id),
                    Name = s.Name,
                    ProductScaleId = newScaleId
                }).ToList(),
                Rows = productUpdateDTO.ProductScale?.Rows?.Select(r => new SizeModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Idx = r.Id,
                    ProductSizeValueId = r.ProductSizeValueId,
                    ProductSizeValue = r.ProductSizeValue,
                    ScaleValues = r.ScaleValues,
                    ProductScaleId = newScaleId
                }).ToList(),
                ProductId = productId,
                Created = product.ProductScale.Created,
                LastUpdated = DateTime.UtcNow
            };
            newProduct.ProductScale = productScale;
        }
        newProduct.FeatureValueIds = featureValueIds;

        if (productUpdateDTO.MainThumbnail != null)
        {
            if (product.MainImage != null)
            {
                _byteFileUtility.DeleteFiles(new[] { product.MainImage }, nameof(Product), product.Code);
            }
            var newMainImage = _byteFileUtility.SaveFileInFolder<EntityMainImage<Guid, Product>>([productUpdateDTO.MainThumbnail], nameof(Product), newProduct.Code, false).First();
            newProduct.MainImage = newMainImage;
        }

        if (productUpdateDTO.Thumbnail != null)
        {
            if (product.Images != null)
            {
                _byteFileUtility.DeleteFiles(product.Images, nameof(Product), product.Code);
            }
            var newImages = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Product>>(productUpdateDTO.Thumbnail, nameof(Product), newProduct.Code, false);
            newProduct.Images = newImages;
        }
        if (productUpdateDTO.Thumbnail == null)
        {
            if (product.Images != null)
            {
                _byteFileUtility.DeleteFiles(product.Images, nameof(Product), product.Code);
            }
        }


        if (productUpdateDTO.StockItems != null)
        {
            var stockItemsDto = productUpdateDTO.StockItems.Select(stockItemDTO =>
            {
                var featureIds = new List<Guid>();
                if (stockItemDTO.FeatureValueId != null)
                {
                    featureIds.AddRange(stockItemDTO.FeatureValueId);
                }

                Guid sizeId2 = stockItemDTO.SizeId ?? Guid.Empty;

                if (stockItemDTO.ImageStock != null)
                {
                    var existingStockItem = product.StockItems?.FirstOrDefault(si => si.StockId == stockItemDTO.StockId);
                    if (existingStockItem != null && existingStockItem.Images != null)
                    {
                        _byteFileUtility.DeleteFiles(existingStockItem.Images, nameof(Product), newProduct.Code);
                    }
                }

                return new StockItem
                {
                    Id = stockItemDTO.Id,
                    StockId = stockItemDTO.StockId,
                    IsHidden = stockItemDTO.IsHidden,
                    Images = stockItemDTO.ImageStock != null ? _byteFileUtility.SaveFileInFolder<EntityImage<Guid, StockItem>>([stockItemDTO.ImageStock], nameof(Product), newProduct.Code, false) : new List<EntityImage<Guid, StockItem>>(),
                    Idx = stockItemDTO.Idx ?? string.Empty,
                    ProductId = productId,
                    FeatureValueId = featureIds,
                    SizeId = sizeId2 == Guid.Empty ? null : sizeId2,
                    Quantity = stockItemDTO.Quantity,
                    Discount = stockItemDTO.Discount,
                    OfferStartTime = DateTime.UtcNow,
                    OfferEndTime = stockItemDTO.OfferTime.HasValue ? DateTime.UtcNow.AddHours(stockItemDTO.OfferTime.Value) : null,
                    OfferTime = stockItemDTO.OfferTime,
                    Price = stockItemDTO.Price,
                    PurchasePrice = stockItemDTO.PurchasePrice,
                    Weight = stockItemDTO.Weight,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    AdditionalProperties = stockItemDTO.AdditionalProperties
                };
            }).ToList();

            newProduct.InStock = stockItemsDto.Sum(stockItem => stockItem.Quantity);
            newProduct.StockItems = stockItemsDto;
        }

        await _context.Products.AddAsync(newProduct);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<Guid>
        {
            Data = newProduct.Id,
            Message = "محصول با موفقیت بروزرسانی شد"
        };
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(Guid id)
    {
        var product = await _context.Products.Include(x => x.Images).Include(x => x.MainImage).Include(x => x.StockItems).ThenInclude(ps => ps.Images)
    .FirstOrDefaultAsync(p => p.Id == id);


        if (product == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "محصولی پیدا نشد."
            };
        }
        if (product.MainImage != null)
        {
            _byteFileUtility.DeleteFiles([product.MainImage], nameof(Product), product.Code);
        }
        if (product.Images != null)
        {
            _byteFileUtility.DeleteFiles(product.Images, nameof(Product), product.Code);
        }
        if (product.StockItems != null)
        {
            foreach (var stockItem in product.StockItems)
            {
                if (stockItem.Images != null)
                {
                    _byteFileUtility.DeleteFiles(stockItem.Images, nameof(Product), product.Code);
                }
            }
        }
        _context.Products.Remove(product);

        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Success = false,
            Message = $"محصول {product.Slug} با موفقیت حذف شد"
        };
    }

    public async Task<ServiceResponse<bool>> DeleteTrashAsync(Guid id)
    {
        var product = await _context.Products
                                    .FirstOrDefaultAsync(p => p.Id == id);


        if (product == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "محصولی پیدا نشد."
            };
        }
        product.IsDeleted = true;
        product.IsActive = false;
        _context.Products.Update(product);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Success = false,
            Message = $"محصول {product.Slug} با موفقیت حذف شد"
        };
    }

    public async Task<ServiceResponse<bool>> RestoreProductAsync(Guid id)
    {
        var product = await _context.Products
                                    .FirstOrDefaultAsync(p => p.Id == id);


        if (product == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "محصولی پیدا نشد."
            };
        }
        product.IsDeleted = false;
        _context.Products.Update(product);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Success = false,
            Message = $"محصول {product.Slug} با موفقیت بازگردانی شد"
        };
    }

    public async Task<ServiceResponse<string>> UploadMedia(Description file)
    {
        var id = Guid.NewGuid();
        var description = new DescriptionEntity
        {
            Id = id,
            Name = nameof(Description),
            Thumbnail = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, DescriptionEntity>>([file.Thumbnail], nameof(DescriptionEntity), "SubDescriptionEntity", false).First()
        };
        await _context.Descriptions.AddAsync(description);
        await _context.SaveChangesAsync();

        var mediaData = await _context.Descriptions.Include(d => d.Thumbnail).FirstOrDefaultAsync(c => c.Id == id);

        var url = _byteFileUtility.GetEncryptedFileActionUrl
              ([new EntityImageDto
                            {
                                Id = mediaData.Thumbnail.Id,
                                ImageUrl = mediaData.Thumbnail.ImageUrl,
                                Placeholder = mediaData.Thumbnail.Placeholder
                            }],
              nameof(DescriptionEntity), "SubDescriptionEntity").First();



        return new ServiceResponse<string>
        {
            Data = url.ImageUrl
        };
    }
}