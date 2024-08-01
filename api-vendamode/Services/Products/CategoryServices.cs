using api_vendace.Data;
using api_vendace.Entities;
using api_vendace.Entities.Products;
using api_vendace.Interfaces;
using api_vendace.Models;
using api_vendace.Models.Dtos;
using api_vendace.Models.Dtos.ProductDto;
using api_vendace.Models.Dtos.ProductDto.Category;
using api_vendace.Models.Dtos.ProductDto.Feature;
using api_vendace.Utility;
using api_vendamode.Models.Dtos.ProductDto.Category;
using api_vendamode.Interfaces.IServices;
using Microsoft.EntityFrameworkCore;
using api_vendamode.Models.Query;
using api_vendamode.Entities.Products;
using api_vendace.Models.Query;

namespace api_vendace.Services.Products;

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

    public class SubCategoryResult
    {
        public CategoryDTO Category { get; set; } = default!;
        public List<CategoryDTO> Children { get; set; } = default!;

    }


    public async Task<ServiceResponse<bool>> AddCategory(CategoryCreateDTO categoryCreate)
    {
        var maidCategory = await _context.Categories
                            .Include(c => c.CategorySizes)
                            .ThenInclude(c => c.Size)
                            .FirstOrDefaultAsync(c => c.Id == categoryCreate.MainId);

        var parentCategory = categoryCreate.ParentCategoryId.HasValue
                ? await _context.Categories.FindAsync(categoryCreate.ParentCategoryId.Value)
                : null;

        var mainCategory = categoryCreate.MainCategoryId.HasValue
                ? await _context.Categories.FindAsync(categoryCreate.MainCategoryId.Value)
                : null;


        categoryCreate.Level = mainCategory is null ? 0 : 1;

        for (int i = 1; parentCategory != null; i++)
        {
            if (parentCategory.Level >= i || parentCategory.Level == 0)
            {
                categoryCreate.Level = parentCategory.Level + 1;
                break;
            }

            parentCategory = await _context.Categories.FindAsync(parentCategory.ParentCategoryId);
        }

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Images = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, Category>>(categoryCreate.Thumbnail!, nameof(Category), false),
            Name = categoryCreate.Name,
            Slug = categoryCreate.Name,
            IsActive = categoryCreate.IsActive,
            MainCategoryId = categoryCreate.MainCategoryId ?? null,
            ParentCategoryId = categoryCreate.ParentCategoryId ?? null,
            Level = categoryCreate.Level,
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
        };

        await _context.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        // Update the sizes for the new category
        var parentCategorySizes = maidCategory?.CategorySizes?.Select(cs => cs.SizeId).ToList();
        if (parentCategorySizes != null && parentCategorySizes.Count > 0)
        {
            var categorySizeDTO = new CategorySizeDTO
            {
                SizeIds = parentCategorySizes
            };

            await UpdateCategorySizes(category.Id, categorySizeDTO);
        }

        return new ServiceResponse<bool>
        {
            Data = true,
            Message = "دسته بندی با موفقیت ایجاد شد"
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
            SizeCount = 0,
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

    public async Task<ServiceResponse<List<CategoryDTO>>> GetAllSubCategories()
    {
        var categories = await GetCategoriesAsync();

        // Sort categories by level
        var sortedCategories = categories.OrderBy(c => c.Level).ToList();

        // Get categories with level >= 1
        var filteredCategories = sortedCategories.Where(c => c.Level >= 1).ToList();

        // Build tree recursively
        var categoriesTree = BuildCategoryTree(filteredCategories, sortedCategories);

        // Flatten the tree and select only the name and id properties
        var categoriesDto = categoriesTree.Select(c => new CategoryDTO { Id = c.Id, Name = c.Name, Level = c.Level }).ToList();
        foreach (var categoryDto in categoriesDto)
        {
            if (categoryDto.ChildCategories != null)
            {
                categoriesDto.AddRange(categoryDto.ChildCategories.Select(c => new CategoryDTO { Id = c.Id, Name = c.Name, Level = c.Level }));
            }
        }

        return new ServiceResponse<List<CategoryDTO>>
        {
            Data = categoriesDto
        };
    }

    private int GetProductCount(Category category, List<Category> allCategories)
    {
        int count = category.Products != null ? category.Products.Count : 0;

        var childCategories = allCategories.Where(c => c.ParentCategoryId == category.Id).ToList();

        foreach (var childCategory in childCategories)
        {
            count += GetProductCount(childCategory, allCategories);
        }

        return count;
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
                Slug = category.Slug,
                Level = category.Level,
                IsActive = category.IsActive,
                IsDeleted = category.IsDeleted,
                HasSizeProperty = category.HasSizeProperty,
                ParentCategoryId = category.ParentCategoryId,
                ImagesSrc = category.Images != null ? _byteFileUtility.GetEncryptedFileActionUrl(category.Images.Select(img => new EntityImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl ?? string.Empty,
                    Placeholder = img.Placeholder ?? string.Empty
                }).ToList(), nameof(Category)).First() : null,
                Count = GetProductCount(category, allCategories),
                SubCategoryCount = allCategories.Count(c => c.ParentCategoryId == category.Id),
                FeatureCount = category.CategoryProductFeatures != null ? category.CategoryProductFeatures.Count : 0,
                FeatureIds = category.CategoryProductFeatures?.Select(cpf => cpf.ProductFeature.Id).ToList(),
                SizeCount = category.CategorySizes is not null ? category.CategorySizes.Count : 0,
                ProductSizeCount = category.CategoryProductSizes is not null ? category.CategoryProductSizes.Count : 0,
                ProductSizeId = category.CategoryProductSizes is not null ? category.CategoryProductSizes.Select(x => x.ProductSizeId).ToList() : [],
                CategorySizes = new CategorySizeDTO
                {
                    Ids = category.CategorySizes?.Select(cs => cs.Id).ToList(),
                    SizeIds = category.CategorySizes?.Select(cs => cs.SizeId).ToList()
                },
                ParentCategory = category.ParentCategory is not null ? new CategoryDTO { Id = category.ParentCategory.Id, Name = category.ParentCategory.Name } : null,
                BrandCount = 0,
                Created = category.Created,
                LastUpdated = category.LastUpdated
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

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }

    public async Task<ServiceResponse<bool>> UpdateCategory(CategoryUpdateDTO categoryUpdate)
    {
        var category = await _context.Categories
            .Include(c => c.Images)
            .Include(C => C.ParentCategory)
            .Include(c => c.ChildCategories)
            .FirstOrDefaultAsync(c => c.Id == categoryUpdate.Id);

        if (category == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "دسته بندی مورد نظر یافت نشد"
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
            if (parentCategory.Level >= i || parentCategory.Level == 0)
            {
                categoryUpdate.Level = parentCategory.Level + 1;
                break;
            }

            parentCategory = await _context.Categories.FindAsync(parentCategory.ParentCategoryId);
        }

        category.Name = categoryUpdate.Name;
        category.Slug = categoryUpdate.Name;
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
            Data = true,
            Message = "دسته بندی با موفقیت بروزرسانی شد"
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
    private async Task UpdateCategorySizes(Guid categoryId, CategorySizeDTO? categorySizes)
    {
        var category = await _context.Categories
                            .Include(c => c.CategorySizes)
                            .ThenInclude(c => c.Size)
                            .FirstOrDefaultAsync(c => c.Id == categoryId);

        if (category == null)
        {
            throw new Exception("دسته بندی مورد نظر یافت نشد");
        }

        if (categorySizes != null)
        {
            var categorySizeList = await _context.CategorySizes.Where(c => categorySizes.Ids != null && categorySizes.Ids.Contains(c.Id)).ToListAsync();
            if (categorySizeList is not null)
            {
                _context.CategorySizes.RemoveRange(categorySizeList);
            }

            if (categorySizes.SizeIds != null)
            {
                foreach (var sizeId in categorySizes.SizeIds)
                {
                    var categorySize = new CategorySize
                    {
                        Id = Guid.NewGuid(),
                        SizeId = sizeId,
                        CategoryId = categoryId
                    };
                    await _context.CategorySizes.AddAsync(categorySize);
                }
            }
        }

        category.LastUpdated = DateTime.UtcNow;
        _context.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync();

        var childCategories = await GetChildCategoriesSizeUpdate(categoryId);
        foreach (var childCategory in childCategories)
        {
            await UpdateCategorySizes(childCategory.Id, categorySizes);
        }
    }
    public async Task<ServiceResponse<bool>> UpdateFeatureInCategory(CategoryFeatureUpdateDTO categoryFeatureDTO)
    {
        try
        {
            var category = await _context.Categories
                    .Include(c => c.CategoryProductFeatures)
                    .ThenInclude(cpf => cpf.ProductFeature)
                    .FirstOrDefaultAsync(c => c.Id == categoryFeatureDTO.CategoryId);

            if (category == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "دسته بندی مورد نظر یافت نشد"
                };
            }

            if (categoryFeatureDTO.FeatureIds != null)
            {
                // حذف ویژگی‌های فعلی مرتبط با این دسته‌بندی
                var currentFeatures = category.CategoryProductFeatures.ToList();
                _context.CategoryProductFeatures.RemoveRange(currentFeatures);

                // اضافه کردن ویژگی‌های جدید به دسته‌بندی
                foreach (var featureId in categoryFeatureDTO.FeatureIds)
                {
                    var feature = await _context.ProductFeatures.FirstOrDefaultAsync(f => f.Id == featureId);
                    if (feature == null)
                    {
                        return new ServiceResponse<bool>
                        {
                            Success = false,
                            Message = "ویژگی مورد نظر یافت نشد"
                        };
                    }

                    var categoryProductFeature = new CategoryProductFeature
                    {
                        CategoryId = category.Id,
                        ProductFeatureId = feature.Id
                    };

                    await _context.CategoryProductFeatures.AddAsync(categoryProductFeature);
                }
            }

            category.LastUpdated = DateTime.UtcNow;
            category.HasSizeProperty = categoryFeatureDTO.HasSizeProperty ?? false;
            _context.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            if (categoryFeatureDTO.CategorySizes != null)
            {
                await UpdateCategorySizes(categoryFeatureDTO.CategoryId, categoryFeatureDTO.CategorySizes);
            }

            return new ServiceResponse<bool>
            {
                Data = true,
                Message = "دسته بندی بروزرسانی شد"
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<List<Category>> GetChildCategoriesSizeUpdate(Guid parentId)
    {
        return await _context.Categories
            .Where(c => c.ParentCategoryId == parentId && !c.IsDeleted)
            .ToListAsync();
    }



    public async Task<ServiceResponse<CategoryDTO>> GetBy(Guid id)
    {
        var category = await _context.Categories
                    .Include(c => c.ParentCategory)
                    .Include(c => c.CategoryProductFeatures)
                        .ThenInclude(cpf => cpf.ProductFeature)
                    .Include(c => c.CategoryProductSizes)
                        .ThenInclude(ps => ps.ProductSize)
                        .ThenInclude(ps => ps.ProductSizeProductSizeValues)
                        .ThenInclude(pspsv => pspsv.ProductSizeValue)
                    .Include(c => c.CategorySizes)
                        .ThenInclude(c => c.Size)
                    .Include(c => c.ChildCategories)
                    .Include(c => c.Images)
                    .OrderByDescending(category => category.LastUpdated)
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

        var subCategoryList = (await GetAllSubCategories()).Data?.Where(c => c.Level > 0);
        var categoryDTO = new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            IsActive = category.IsActive,
            Level = category.Level,
            ImagesSrc = category.Images != null ? _byteFileUtility.GetEncryptedFileActionUrl(category.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl ?? string.Empty,
                Placeholder = img.Placeholder ?? string.Empty
            }).ToList(), nameof(Category)).First() : null,
            Categories = subCategoryList,
            ParentCategories = category.GetParentCategories(_context).Select(cate => new CategoryDTO
            {
                Id = cate.Id,
                Name = cate.Name,
                Slug = cate.Name,
                Level = cate.Level,
                IsActive = cate.IsActive,
                IsDeleted = cate.IsDeleted,
                ParentCategoryId = cate.ParentCategoryId,
                Count = cate.Products != null ? cate.Products.Count : 0,
                FeatureCount = cate.CategoryProductFeatures != null ? cate.CategoryProductFeatures.Count : 0,
                SizeCount = 0,
                BrandCount = 0,
                Created = cate.Created,
                LastUpdated = cate.LastUpdated
            }).ToList(),
            Created = category.Created,
            LastUpdated = category.LastUpdated,
            FeatureIds = category.CategoryProductFeatures?.Select(cpf => cpf.ProductFeature.Id).ToList()
        };

        var mainCategory = categoryDTO.ParentCategories.Where(c => c.Level == 0).Select(c => new Category
        {
            Id = c.Id,
            Name = c.Name,
            Level = c.Level,
            Slug = c.Slug,
        }).ToList();
        var categories = await GetCategoriesAsync();

        var sortedCategories = categories.OrderBy(c => c.Level).ToList();

        // Get root categories (categories with level 0)
        var rootCategories = sortedCategories.Where(c => c.Level == 0 && mainCategory.Any(x => x.Id == c.Id)).ToList();

        var categoriesTree = BuildCategoryTree(rootCategories, sortedCategories);
        categoryDTO.ParentCategoriesTree = categoriesTree.Count != 0 ? categoriesTree[0].ChildCategories ?? new List<CategoryDTO>() : [];

        return new ServiceResponse<CategoryDTO>
        {
            Data = categoryDTO,
            Success = true
        };
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

    public async Task<ServiceResponse<CategoryDTO>> GetBy(string name)
    {
        var category = await _context.Categories
                                    .Include(c => c.Images)
                                    .Include(c => c.ChildCategories)
                                    .FirstOrDefaultAsync(c => c.Name == name);

        if (category == null)
        {
            return new ServiceResponse<CategoryDTO>
            {
                Data = null,
                Success = false,
                Message = "دسته بندی مورد نظر یافت نشد"
            };
        }
        var subCategoryList = (await GetAllSubCategories()).Data?.Where(c => c.Level > 0);
        var categoryDTO = new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            IsActive = category.IsActive,
            Level = category.Level,
            ImagesSrc = category.Images != null ? _byteFileUtility.GetEncryptedFileActionUrl(category.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl ?? string.Empty,
                Placeholder = img.Placeholder ?? string.Empty
            }).ToList(), nameof(Category)).First() : null,
            Categories = subCategoryList,
            Created = category.Created,
            LastUpdated = category.LastUpdated
        };

        return new ServiceResponse<CategoryDTO>
        {
            Data = categoryDTO,
            Success = true
        };
    }

    public async Task<ServiceResponse<List<CategoryDTO>>> GetParentSubCategoryAsync(Guid id, RequestBy requestSub)
    {

        var categoriesQuery = _context.Categories
                                  .Include(c => c.CategoryProductFeatures)
                    .ThenInclude(cpf => cpf.ProductFeature)
                    .Include(c => c.CategoryProductSizes)
                    .ThenInclude(ps => ps.ProductSize)
                    .ThenInclude(ps => ps.ProductSizeProductSizeValues)
                    .ThenInclude(pspsv => pspsv.ProductSizeValue)
                    .Include(c => c.CategorySizes)
                    .ThenInclude(c => c.Size)
                    .Include(c => c.ChildCategories)
                    .Include(c => c.ParentCategory)
                    .Include(c => c.Images)
                    .OrderByDescending(category => category.LastUpdated)
                    .AsQueryable();
        var allCategories = new List<Category>();

        allCategories = await categoriesQuery.ToListAsync();

        categoriesQuery = categoriesQuery.Where(c => c.Id == id);
        // Pagination and data retrieval
        var totalCount = await categoriesQuery.CountAsync();
        var paginatedCategories = await categoriesQuery.ToListAsync();

        var sortedCategories = allCategories.OrderBy(c => c.Level).ToList();

        // // Get root categories (categories with level 0)
        var rootCategories = paginatedCategories.Where(c => c.Level == 0).ToList();

        var categoriesTree = BuildCategoryTree(rootCategories, sortedCategories);


        // Flatten the tree to get all categories in a single list
        var flatCategories = FlattenCategoryTree(categoriesTree);

        // Get all main categories (level 0)
        var mainCategories = flatCategories.Where(c => c.Level == 0).ToList();

        // Create a list to hold the IDs of child categories
        var childCategoryIds = new HashSet<Guid>();

        // Collect IDs of child categories
        foreach (var mainCategory in mainCategories)
        {
            if (mainCategory.ChildCategories != null && mainCategory.ChildCategories.Count > 0)
            {
                childCategoryIds.UnionWith(mainCategory.ChildCategories.Select(c => c.Id));
            }
        }

        // Apply search filter
        if (!string.IsNullOrEmpty(requestSub.SearchTerm))
        {
            var childCategoryIdsFlat = new HashSet<Guid>();

            string searchLower = requestSub.SearchTerm.ToLower();

            // Filter the search term
            flatCategories = flatCategories
                             .Where(c => c.Level != 0 && c.Slug.ToLower().Contains(searchLower))
                             .ToList();
            foreach (var mainCategory in flatCategories)
            {
                if (mainCategory.ChildCategories != null && mainCategory.ChildCategories.Count > 0)
                {
                    childCategoryIdsFlat.UnionWith(mainCategory.ChildCategories.Select(c => c.Id));
                }
            }
            flatCategories = flatCategories.Where(c => !childCategoryIdsFlat.Contains(c.Id)).ToList();

            return new ServiceResponse<List<CategoryDTO>>
            {
                Count = flatCategories.Count,
                Data = flatCategories
            };
        }

        return new ServiceResponse<List<CategoryDTO>>
        {
            Count = totalCount,
            Data = categoriesTree = categoriesTree.OrderByDescending(category => category.LastUpdated).First().ChildCategories,
        };
    }

    private List<CategoryDTO> FlattenCategoryTree(List<CategoryDTO> categories, HashSet<Guid> uniqueIds = null)
    {
        var flatList = new List<CategoryDTO>();

        if (uniqueIds == null)
        {
            uniqueIds = new HashSet<Guid>();
        }

        foreach (var category in categories)
        {
            if (!uniqueIds.Contains(category.Id))
            {
                flatList.Add(category);
                uniqueIds.Add(category.Id);
            }

            if (category.ChildCategories != null && category.ChildCategories.Count > 0)
            {
                flatList.AddRange(FlattenCategoryTree(category.ChildCategories, uniqueIds));
            }
        }

        return flatList;
    }


    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await _context.Categories
                            .Include(c => c.CategoryProductFeatures)
                            .ThenInclude(cpf => cpf.ProductFeature)
                            .Include(c => c.CategoryProductSizes)
                            .ThenInclude(ps => ps.ProductSize)
                            .ThenInclude(ps => ps.ProductSizeProductSizeValues)
                            .ThenInclude(pspsv => pspsv.ProductSizeValue)
                            .Include(c => c.ChildCategories)
                            .Include(c => c.Images)
                            .Include(c => c.Products)
                            .AsNoTracking()
                            .ToListAsync();
    }


    public Task<Category?> GetCategoryAsyncBy(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<SubCategoryResult>> GetSubCategoryAsync(RequestBy requestSub)
    {
        var parentId = requestSub.Id;
        var categorySlug = requestSub.Slug;
        var category = new Category();

        if (parentId is not null)
        {
            category = await _context.Categories.Include(c => c.ChildCategories!).ThenInclude(c => c.Images).Include(c => c.Images).FirstOrDefaultAsync(c => c.Id == parentId);
        }
        else
        {
            category = await _context.Categories.Include(c => c.ChildCategories!).ThenInclude(c => c.Images).Include(c => c.Images).FirstOrDefaultAsync(c => c.Slug == categorySlug);
        }


        if (category is null)
        {
            return new ServiceResponse<SubCategoryResult>
            {
                Data = null,
                Success = false,
                Message = "دسته بندی پدر یافت نشد"
            };
        }

        if (category.ChildCategories is null)
        {
            return new ServiceResponse<SubCategoryResult>
            {
                Data = null,
                Success = false,
                Message = "زیر دسته های مورد نظر یافت نشد"
            };
        }


        var categoriesDto = new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            ImagesSrc = category.Images != null ? _byteFileUtility.GetEncryptedFileActionUrl(category.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl ?? string.Empty,
                Placeholder = img.Placeholder ?? string.Empty
            }).ToList(), nameof(Category)).First() : null,
            ChildCategories = category.ChildCategories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name,
                IsActive = c.IsActive,
                Level = c.Level,
                Slug = c.Slug,
                ImagesSrc = c.Images != null ? _byteFileUtility.GetEncryptedFileActionUrl(c.Images.Select(img => new EntityImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl ?? string.Empty,
                    Placeholder = img.Placeholder ?? string.Empty
                }).ToList(), nameof(Category)).First() : null,
                Created = c.Created,
                LastUpdated = c.LastUpdated
            }).ToList(),
            Created = category.Created,
            LastUpdated = category.LastUpdated,
        };

        var result = new SubCategoryResult
        {
            Category = categoriesDto,
            Children = categoriesDto.ChildCategories
        };
        return new ServiceResponse<SubCategoryResult>
        {
            Data = result
        };
    }

    public IEnumerable<Guid> GetAllChildCategoriesHelper(Guid parentCategoryId, List<Category> allCategories, List<Category> allChildCategories)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Guid> GetAllChildCategories(Guid parentCategoryId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<Pagination<CategoryDTO>>> GetAllCategories(RequestQuery requestQuery)
    {
        var pageNumber = requestQuery.PageNumber ?? 1;
        var pageSize = requestQuery.PageSize ?? 15;

        var categoriesQuery = _context.Categories
                    .Include(c => c.Products)
                    .Include(c => c.CategoryProductFeatures)
                    .ThenInclude(cpf => cpf.ProductFeature)
                    .Include(c => c.CategoryProductSizes)
                    .ThenInclude(ps => ps.ProductSize)
                    .ThenInclude(ps => ps.ProductSizeProductSizeValues)
                    .ThenInclude(pspsv => pspsv.ProductSizeValue)
                    .Include(c => c.CategorySizes)
                    .ThenInclude(c => c.Size)
                    .Include(c => c.ChildCategories)
                    .Include(c => c.Images)
                    .OrderByDescending(category => category.LastUpdated)
                    .AsQueryable();
        var allCategories = new List<Category>();


        allCategories = await categoriesQuery.ToListAsync();

        categoriesQuery = categoriesQuery.Where(c => c.Level == 0);
        // Pagination and data retrieval
        var totalCount = await categoriesQuery.CountAsync();
        // Search filter
        if (!string.IsNullOrEmpty(requestQuery.Search))
        {
            string searchLower = requestQuery.Search.ToLower();
            categoriesQuery = categoriesQuery.Where(p => p.Slug.ToLower().Contains(searchLower));
        }
        var paginatedCategories = await categoriesQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        var sortedCategories = allCategories.OrderBy(c => c.Level).ToList();

        // // Get root categories (categories with level 0)
        var rootCategories = paginatedCategories.Where(c => c.Level == 0).ToList();

        var categoriesTree = BuildCategoryTree(rootCategories, sortedCategories);

        var pagination = new Pagination<CategoryDTO>
        {
            CurrentPage = pageNumber,
            NextPage = pageNumber < (totalCount / pageSize) ? pageNumber + 1 : pageNumber,
            PreviousPage = pageNumber > 1 ? pageNumber - 1 : 1,
            HasNextPage = pageNumber < (totalCount / pageSize),
            HasPreviousPage = pageNumber > 1,
            LastPage = (int)Math.Ceiling((double)totalCount / pageSize),
            Data = categoriesTree = categoriesTree.OrderByDescending(category => category.LastUpdated).ToList(),
            TotalCount = totalCount
        };

        return new ServiceResponse<Pagination<CategoryDTO>>
        {
            Count = totalCount,
            Data = pagination
        };
    }

    public async Task<ServiceResponse<CategoryResult>> GetCategories()
    {
        var categories = await GetCategoriesAsync();

        // Convert entities to DTOs
        var categoriesDto = categories.Select(category => new CategoryDTO
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
            }).ToList(), nameof(Category)).FirstOrDefault() : null,
            Count = category.Products?.Count ?? 0,
            FeatureCount = category.CategoryProductFeatures?.Count ?? 0,
            SizeCount = 0,
            BrandCount = 0,
            Created = category.Created,
            LastUpdated = category.LastUpdated
        }).ToList();

        // Method to find children recursively
        List<CategoryDTO> GetCategoriesWithChildren(CategoryDTO parentCategory)
        {
            var children = categoriesDto
                .Where(c => c.ParentCategoryId == parentCategory.Id)
                .Select(childCategory =>
                {
                    childCategory.ChildCategories = GetCategoriesWithChildren(childCategory);
                    return childCategory;
                })
                .ToList();

            return children;
        }

        // Find root categories
        var rootCategories = categoriesDto
            .Where(c => c.ParentCategoryId == null)
            .Select(rootCategory =>
            {
                rootCategory.ChildCategories = GetCategoriesWithChildren(rootCategory);
                return rootCategory;
            })
            .ToList();

        var result = new CategoryResult
        {
            CategoryDTO = categoriesDto,
            CategoryList = rootCategories
        };

        return new ServiceResponse<CategoryResult>
        {
            Count = categories.Count,
            Data = result
        };
    }

    public async Task<ServiceResponse<CategoryDTO>> GetBySlugAsync(string categorySlug)
    {
        var category = await _context.Categories
                            .Include(c => c.Images)
                            .Include(c => c.ChildCategories)
                            .FirstOrDefaultAsync(c => c.Slug == categorySlug);

        if (category == null)
        {
            return new ServiceResponse<CategoryDTO>
            {
                Data = null,
                Success = false,
                Message = "دسته بندی مورد نظر یافت نشد"
            };
        }

        var subCategoryList = await GetSubCategories(category.Id);

        var categoryDTO = new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            IsActive = category.IsActive,
            Level = category.Level,
            ImagesSrc = category.Images != null ? _byteFileUtility.GetEncryptedFileActionUrl(category.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl ?? string.Empty,
                Placeholder = img.Placeholder ?? string.Empty
            }).ToList(), nameof(Category)).First() : null,
            Categories = subCategoryList,
            Created = category.Created,
            LastUpdated = category.LastUpdated
        };

        return new ServiceResponse<CategoryDTO>
        {
            Data = categoryDTO,
            Success = true
        };
    }

    private async Task<List<CategoryDTO>> GetSubCategories(Guid categoryId)
    {
        var categories = await _context.Categories
                                .Where(c => c.ParentCategoryId == categoryId)
                                .Include(c => c.ChildCategories)
                                .ToListAsync();

        var subCategories = new List<CategoryDTO>();
        foreach (var category in categories)
        {
            subCategories.Add(new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                IsActive = category.IsActive,
                Level = category.Level,
                Created = category.Created,
                LastUpdated = category.LastUpdated,
                Categories = await GetSubCategories(category.Id) // Recursively get subcategories
            });
        }

        return subCategories;
    }
    public List<Guid> GetAllCategoryIds(Guid categoryId)
    {
        var categoryIds = new List<Guid> { categoryId };
        GetChildCategoryIds(categoryId, null, categoryIds);
        return categoryIds;
    }
    public List<Guid> GetAllCategoryIdsBy(string categorySlug)
    {
        var categoryIds = new List<Guid>();
        GetChildCategoryIds(null, categorySlug, categoryIds);
        return categoryIds;
    }

    private void GetChildCategoryIds(Guid? categoryId, string? categorySlug, List<Guid> categoryIds)
    {
        if (categoryId is not null)
        {
            var childCategories = _context.Categories.Where(c => c.ParentCategoryId == categoryId).ToList();
            foreach (var childCategory in childCategories)
            {
                categoryIds.Add(childCategory.Id);
                GetChildCategoryIds(childCategory.Id, null, categoryIds);
            }
        }
        else
        {
            var childCategories = _context.Categories.Where(c => c.ParentCategory.Slug == categorySlug).ToList();
            foreach (var childCategory in childCategories)
            {
                categoryIds.Add(childCategory.Id);
                GetChildCategoryIds(null, childCategory.Slug, categoryIds);
            }
        }
    }
}

