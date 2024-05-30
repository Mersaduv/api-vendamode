using api_vendamode.Data;
using api_vendamode.Entities.Products;
using api_vendamode.Interfaces;
using api_vendamode.Interfaces.IRepository;
using api_vendamode.Interfaces.IServices;
using api_vendamode.Mapper;
using api_vendamode.Models;
using api_vendamode.Models.Dtos.ProductDto;
using api_vendamode.Models.Dtos.ProductDto.Category;
using api_vendamode.Models.Dtos.ProductDto.Sizes;
using api_vendamode.Models.Dtos.ProductDto.Stock;
using api_vendamode.Utility;
using Microsoft.EntityFrameworkCore;

public class ProductServices : IProductServices
{

    private readonly ApplicationDbContext _context;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ByteFileUtility _byteFileUtility;

    public ProductServices(IProductRepository productRepository, IUnitOfWork unitOfWork, ByteFileUtility byteFileUtility, ApplicationDbContext context)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _byteFileUtility = byteFileUtility;
        _context = context;
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

        // load feature
        // Null-checks 
        List<Guid> featureIds = productCreateDTO.FeatureIds ?? new List<Guid>();
        List<Guid> featureValueIds = productCreateDTO.FeatureValueIds ?? new List<Guid>();
        var features = await _context.Features
            .Include(v => v.Values)
            .Where(c => (!featureIds.Contains(c.Id)
                || c.Values != null) && c.Values!.Any(fv => featureValueIds.Contains(fv.Id)))
            .ToListAsync();

        var productScale = new ProductScale
        {
            Id = Guid.NewGuid(),
            Columns = productCreateDTO.ProductScale?.Columns,
            Rows = productCreateDTO.ProductScale?.Rows,
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };
        product.Code = GenerateProductCode();
        product.Features = features;
        product.ProductScale = productScale;

        var stockItems = productCreateDTO.StockItems.Select(stockItemDTO => new StockItem
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            FeatureId = stockItemDTO.FeatureId,
            SizeId = stockItemDTO.SizeId,
            Quantity = stockItemDTO.Quantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }).ToList();

        product.InStock = stockItems.Sum(stockItem => stockItem.Quantity);

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

    // customer
    public async Task<ServiceResponse<Pagination<ProductDTO>>> GetProductsPagination(int pageNumber, int pageSize)
    {
        try
        {
            var paginatedProducts = await _productRepository.GetPaginationAsync(pageNumber, pageSize);

            var productDTOs = paginatedProducts.Data
                .Select(product => product.ToProductResponse(_byteFileUtility))
                .ToList();

            var paginatedResult = new Pagination<ProductDTO>
            {
                CurrentPage = pageNumber,
                NextPage = pageNumber < (paginatedProducts.TotalCount / pageSize) ? pageNumber + 1 : pageNumber,
                PreviousPage = pageNumber > 1 ? pageNumber - 1 : 1,
                HasNextPage = pageNumber < (paginatedProducts.TotalCount / pageSize),
                HasPreviousPage = pageNumber > 1,
                LastPage = (int)Math.Ceiling((double)paginatedProducts.TotalCount / pageSize),
                Data = productDTOs,
                TotalCount = paginatedProducts.TotalCount
            };

            return new ServiceResponse<Pagination<ProductDTO>>
            {
                Data = paginatedResult,
                Success = true
            };
        }
        catch (Exception)
        {
            return new ServiceResponse<Pagination<ProductDTO>>
            {
                Success = false,
                Message = "خطایی رخ داد در هنگام دریافت لیست محصولات"
            };
        }
    }

    //admin
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

            var result = product.ToProductResponse(_byteFileUtility);

            var categoryLevelIds = GetCategoryLevelIds(product.Category!);
            var categories = await _context.Categories
                .Where(c => categoryLevelIds.Contains(c.Id))
                .ToListAsync();

            // tree category
            result.CategoryLevels = categories.Select(c => new CategoryLevels
            {
                Id = c.Id,
                Name = c.Name,
                Level = c.Level,
                Url = c.Name.ToLower()
            }).ToList();
            result.CategoryList = categoryLevelIds;

            // sizes guide
            var productSize = await _context.ProductSizes
            .Where(x => x.CategoryId == product.CategoryId)
            .Include(s => s.Images)
            .Include(s => s.Sizes)
            .Include(s => s.ProductSizeValues)
            .FirstOrDefaultAsync();
            var productScale = product.ProductScale;

            List<Sizes>? sizeList = null;
            if (productScale?.Columns?.Any() == true)
            {
                sizeList = await _context.Sizes
                    .Where(x => productScale.Columns.Any(c => c.Id == x.Id))
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

            // feature
            var productFeatureInfo = new ProductFeatureInfo(product);

            result.ProductFeatureInfo = productFeatureInfo;

            // Calculate the average rating
            double sumOfRatings = 0;
            if (product.Review != null && product.Review.Count > 0)
            {
                foreach (var review in product.Review)
                {
                    sumOfRatings += review.Rating;
                }
                result.Rating = sumOfRatings / product.Review.Count;
            }
            else
            {
                result.Rating = 0;
            }

            result.ReviewCount = product.Review?.Count;
            result.InStock = product.InStock;

            return new ServiceResponse<ProductDTO>
            {
                Data = result
            };
        }
        catch (Exception)
        {
            return new ServiceResponse<ProductDTO>
            {
                Success = false,
                Message = "خطایی رخ داد در هنگام دریافت محصول"
            };
        }
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

    public Task<ServiceResponse<bool>> UpdateProduct(Guid id)
    {
        throw new NotImplementedException();
    }
}