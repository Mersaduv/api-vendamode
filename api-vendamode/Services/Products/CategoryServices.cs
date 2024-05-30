using api_vendamode.Data;
using api_vendamode.Entities.Products;
using api_vendamode.Interfaces;
using api_vendamode.Models;
using api_vendamode.Models.Dtos;
using api_vendamode.Models.Dtos.ProductDto;
using api_vendamode.Models.Dtos.ProductDto.Category;
using api_vendamode.Models.Dtos.ProductDto.Feature;
using api_vendamode.Utility;
using ApiAryanakala.Interfaces.IServices;
using Microsoft.EntityFrameworkCore;

namespace api_vendamode.Services.Products;

public class CategoryServices : ICategoryServices
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ByteFileUtility _byteFileUtility;
    private readonly IHttpContextAccessor _httpContext;
    public CategoryServices(ApplicationDbContext context, IUnitOfWork unitOfWork, ByteFileUtility byteFileUtility, IHttpContextAccessor httpContext)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _byteFileUtility = byteFileUtility;
        _httpContext = httpContext;
    }


    public async Task<ServiceResponse<bool>> AddCategory(CategoryCreateDTO categoryCreate)
    {
        if (await GetBy(categoryCreate.Name) != null)
        {
            return new ServiceResponse<bool>
            {
                Data = false,
                Success = false,
                Message = "این دسته بندی قبلا به ثبت رسیده"
            };
        }

        var parentCategory = categoryCreate.ParentCategoryId.HasValue
                ? await _context.Categories.FindAsync(categoryCreate.ParentCategoryId.Value)
                : null;

        var mainCategory = categoryCreate.MainCategoryId.HasValue
                ? await _context.Categories.FindAsync(categoryCreate.MainCategoryId.Value)
                : null;


        categoryCreate.Level = mainCategory is null ? 0 : 1;

        for (int i = 1; parentCategory != null; i++)
        {
            if (parentCategory.Level == i)
            {
                categoryCreate.Level = i + 1;
                break;
            }

            parentCategory = await _context.Categories.FindAsync(parentCategory.ParentCategoryId);
        }

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Images = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Category>>(categoryCreate.Thumbnail!, nameof(Category), false),
            Name = categoryCreate.Name,
            IsActive = categoryCreate.IsActive,
            MainCategoryId = categoryCreate.MainCategoryId ?? null,
            ParentCategoryId = categoryCreate.ParentCategoryId ?? null,
            Level = categoryCreate.Level,
        };

        await _context.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<IEnumerable<CategoryDTO>>> GetMainCategoriesAsync()
    {
        var categories = await GetCategoriesAsync();

        var categoryTasks = categories.Where(c => c.Level == 0)
        .Select(async category => new CategoryDTO
        {
            Id = category.Id,
            ImagesSrc = category.Images != null ? _byteFileUtility.GetEncryptedFileActionUrl(category.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl ?? string.Empty,
                Placeholder = img.Placeholder ?? string.Empty
            }).ToList(), nameof(Category)).First() : null,
            Name = category.Name,
            SizeCount = category.ProductSizes != null && category.ProductSizes.ProductSizeValues != null ? category.ProductSizes.ProductSizeValues.Count : 0,
            Level = category.Level,
            BrandCount = 0,
            SubCategoryCount = await GetSubcategoriesCount(category.Id),
            Count = category.Products != null ? category.Products.Count : 0,
            IsActive = category.IsActive,
            IsDeleted = category.IsDeleted
        });

        var categoriesDto = await Task.WhenAll(categoryTasks);

        return new ServiceResponse<IEnumerable<CategoryDTO>>
        {
            Data = categoriesDto
        };
    }

    private async Task<int> GetSubcategoriesCount(Guid mainCategoryId)
    {
        var subcategoriesCount = 0;

        // Get the main category.
        var mainCategory = await _context.Categories
            .Where(c => c.MainCategoryId == mainCategoryId && !c.IsDeleted)
            .Include(c => c.ChildCategories)
            .FirstOrDefaultAsync();

        if (mainCategory != null && mainCategory.ChildCategories != null)
        {
            // Traverse through the child categories and increment the count.
            foreach (var childCategory in mainCategory.ChildCategories)
            {
                subcategoriesCount++;
                subcategoriesCount += await GetSubcategoriesCount(childCategory.Id);
            }
        }

        return subcategoriesCount;
    }


    public async Task<ServiceResponse<IEnumerable<CategoryDTO>>> GetCategoriesTreeAsync()
    {
        var categories = await GetCategoriesAsync();

        var sortedCategories = categories.OrderBy(c => c.Level).ToList();

        // Get root categories (categories with level 0)
        var rootCategories = sortedCategories.Where(c => c.Level == 0).ToList();

        var categoriesTree = BuildCategoryTree(rootCategories, sortedCategories);

        return new ServiceResponse<IEnumerable<CategoryDTO>>
        {
            Data = categoriesTree
        };
    }

    public async Task<ServiceResponse<IEnumerable<CategoryDTO>>> GetAllSubCategories()
    {
        var categories = await GetCategoriesAsync();

        // Sort categories by level
        var sortedCategories = categories.OrderBy(c => c.Level).ToList();

        // Get categories with level >= 1
        var filteredCategories = sortedCategories.Where(c => c.Level >= 1).ToList();

        // Build tree recursively
        var categoriesTree = BuildCategoryTree(filteredCategories, sortedCategories);

        // Flatten the tree and select only the name and id properties
        var categoriesDto = categoriesTree.Select(c => new CategoryDTO { Id = c.Id, Name = c.Name }).ToList();
        foreach (var categoryDto in categoriesDto)
        {
            if (categoryDto.ChildCategories != null)
            {
                categoriesDto.AddRange(categoryDto.ChildCategories.Select(c => new CategoryDTO { Id = c.Id, Name = c.Name }));
            }
        }

        return new ServiceResponse<IEnumerable<CategoryDTO>>
        {
            Data = categoriesDto
        };
    }


    private List<CategoryDTO> BuildCategoryTree(List<Category> categories, List<Category> allCategories)
    {
        var categoriesDto = new List<CategoryDTO>();

        foreach (var category in categories)
        {
            var categoryDto = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Level = category.Level,
                IsActive = category.IsActive,
                IsDeleted = category.IsDeleted,
                ParentCategoryId = category.ParentCategoryId,
                ImagesSrc = category.Images != null ? _byteFileUtility.GetEncryptedFileActionUrl(category.Images.Select(img => new EntityImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl ?? string.Empty,
                    Placeholder = img.Placeholder ?? string.Empty
                }).ToList(), nameof(Category)).First() : null,
                Count = category.Products != null ? category.Products.Count : 0,
                SubCategoryCount = allCategories.Count(c => c.ParentCategoryId == category.Id),
                FeatureCount = category.ProductFeatures != null ? category.ProductFeatures.Count : 0,
                SizeCount = category.ProductSizes != null && category.ProductSizes.ProductSizeValues != null ? category.ProductSizes.ProductSizeValues.Count : 0,
                BrandCount = 0
            };

            // Get child categories recursively
            categoryDto.ChildCategories = BuildCategoryTree(allCategories.Where(c => c.ParentCategoryId == category.Id).ToList(), allCategories);

            categoriesDto.Add(categoryDto);
        }

        return categoriesDto;
    }


    public async Task<ServiceResponse<bool>> Delete(Guid id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "دسته بندی مورد نظر یافت نشد"
            };
        }

        var hasChildCategories = await _context.Categories.CountAsync(c => c.ParentCategoryId == id) > 0;
        if (hasChildCategories)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "نمی‌توان یک دسته بندی با زیر دسته بندی حذف کرد"
            };
        }

        var hasAssociatedProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
        if (hasAssociatedProducts)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "نمی‌توان یک دسته بندی با محصولات مرتبط حذف کرد"
            };
        }

        category.IsDeleted = true;
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<bool>> UpdateCategory(CategoryUpdateDTO categoryUpdate)
    {
        var category = await _context.Categories.FindAsync(categoryUpdate.Id);

        if (category == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "دسته بندی مورد نظر یافت نشد"
            };
        }

        if (await GetBy(categoryUpdate.Name) != null && categoryUpdate.Name != category.Name)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "این دسته بندی قبلا به ثبت رسیده"
            };
        }

        var parentCategory = categoryUpdate.ParentCategoryId.HasValue
                ? await _context.Categories.FindAsync(categoryUpdate.ParentCategoryId.Value)
                : null;

        var mainCategory = categoryUpdate.MainCategoryId.HasValue
                ? await _context.Categories.FindAsync(categoryUpdate.MainCategoryId.Value)
                : null;

        categoryUpdate.Level = mainCategory is null ? 0 : 1;

        for (int i = 1; parentCategory != null; i++)
        {
            if (parentCategory.Level == i)
            {
                categoryUpdate.Level = i + 1;
                break;
            }

            parentCategory = await _context.Categories.FindAsync(parentCategory.ParentCategoryId);
        }

        category.Name = categoryUpdate.Name;
        category.IsActive = categoryUpdate.IsActive;
        category.MainCategoryId = categoryUpdate.MainCategoryId ?? null;
        category.ParentCategoryId = categoryUpdate.ParentCategoryId ?? null;
        category.Level = categoryUpdate.Level;

        if (categoryUpdate.Thumbnail != null)
        {
            if (category.Images is not null)
            {
                _byteFileUtility.DeleteFiles(category.Images, nameof(Category));
            }
            category.Images = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Category>>(categoryUpdate.Thumbnail, nameof(Category), false);
        }
        _context.Update(category);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }


    public async Task<ServiceResponse<bool>> UpdateIsShowCategory(bool isActive, Guid id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "دسته بندی مورد نظر یافت نشد"
            };
        }
        category.IsActive = isActive;

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<bool>> UpdateFeatureInCategory(CategoryFeatureUpdateDTO categoryFeatureDTO)
    {
        var category = await _context.Categories
                            .Include(c => c.ProductFeatures)
                            .FirstOrDefaultAsync(c => c.Id == categoryFeatureDTO.CategoryId);

        var featureListDto = new List<ProductFeature>();

        if (category == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "دسته بندی مورد نظر یافت نشد"
            };
        }

        if (categoryFeatureDTO.Features != null)
        {
            foreach (var featureDTO in categoryFeatureDTO.Features)
            {
                // Find the feature in the category's features
                var feature = category.ProductFeatures?.FirstOrDefault(f => f.Id == featureDTO.Id);

                if (feature == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "ویژگی مورد نظر یافت نشد"
                    };
                }

                featureListDto.Add(feature);
            }
        }

        category.ProductFeatures = featureListDto;
        category.LastUpdated = DateTime.UtcNow;

        _context.Categories.Update(category);

        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<CategoryDTO>> GetBy(Guid id)
    {
        var category = await _context.Categories
            .Include(c => c.Images)
            .Include(c => c.ChildCategories)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return new ServiceResponse<CategoryDTO>
            {
                Data = null,
                Success = false,
                Message = "دسته بندی مورد نظر یافت نشد"
            };
        }

        var categoryDTO = new CategoryDTO
        {
            Id = category.Id,
            IsActive = category.IsActive,
            Level = category.Level,
            ImagesSrc = category.Images != null ? _byteFileUtility.GetEncryptedFileActionUrl(category.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl ?? string.Empty,
                Placeholder = img.Placeholder ?? string.Empty
            }).ToList(), nameof(Category)).First() : null,
            Categories = category.Level > 0 ? await GetCategorList() : null
        };

        return new ServiceResponse<CategoryDTO>
        {
            Data = categoryDTO,
            Success = true
        };
    }

    private async Task<List<CategoryDTO>> GetCategorList()
    {
        List<Category> getCategoryList = await _context.Categories.AsNoTracking().Include(c => c.ChildCategories).Include(c => c.Images).Include(c => c.ParentCategory).ToListAsync();

        var getCategoryListDTO = new List<CategoryDTO>();

        foreach (var category in getCategoryList)
        {
            var categoryDTO = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name
            };

            getCategoryListDTO.Add(categoryDTO);

            if (category.ChildCategories != null && category.ChildCategories.Any())
            {
                var childCategoriesDTO = GetChildCategories(category.ChildCategories);
                getCategoryListDTO.AddRange(childCategoriesDTO);
            }
        }

        return getCategoryListDTO;
    }

    private List<CategoryDTO> GetChildCategories(List<Category> childCategories)
    {
        var childCategoriesDTO = new List<CategoryDTO>();

        foreach (var childCategory in childCategories)
        {
            var childCategoryDTO = new CategoryDTO
            {
                Id = childCategory.Id,
                Name = childCategory.Name
            };

            childCategoriesDTO.Add(childCategoryDTO);

            if (childCategory.ChildCategories != null && childCategory.ChildCategories.Any())
            {
                var subChildCategoriesDTO = GetChildCategories(childCategory.ChildCategories);
                childCategoriesDTO.AddRange(subChildCategoriesDTO);
            }
        }

        return childCategoriesDTO;
    }

    public Task<ServiceResponse<CategoryDTO?>> GetBy(string name)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await _context.Categories
                    .Include(c => c.ProductFeatures)
                    .Include(c => c.ProductSizes)
                    .Include(c => c.ChildCategories)
                    .Include(c => c.Images)
                    .AsNoTracking()
                    .ToListAsync();
    }

    public Task<Category?> GetCategoryAsyncBy(Guid id)
    {
        throw new NotImplementedException();
    }



    public IEnumerable<Guid> GetAllChildCategoriesHelper(Guid parentCategoryId, List<Category> allCategories, List<Category> allChildCategories)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Guid> GetAllChildCategories(Guid parentCategoryId)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<IEnumerable<CategoryDTO>>> GetAllCategories(int? page, int? pageSize)
    {
        throw new NotImplementedException();
    }
}

