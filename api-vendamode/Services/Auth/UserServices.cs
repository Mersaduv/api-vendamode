using System.Security.Claims;
using api_vendace.Data;
using api_vendace.Entities.Users;
using api_vendace.Entities.Users.Security;
using api_vendace.Interfaces.IServices;
using api_vendace.Models;
using api_vendace.Models.Dtos.AuthDto;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Caching.Memory;
using api_vendace.Interfaces;
using System.Security.Cryptography;
using Newtonsoft.Json;
using api_vendace.Utility;
using api_vendace.Entities;
using api_vendace.Models.Query;
using api_vendace.Models.Dtos;
using api_vendamode.Models.Dtos.AuthDto;
using api_vendamode.Models.Dtos.AuthDto.RoleDto;

namespace api_vendace.Services.Auth;

public class UserServices : IUserServices
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppSettings _appSettings;
    private readonly IMemoryCache _cache;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ByteFileUtility _byteFileUtility;

    public UserServices(ApplicationDbContext context, IPasswordHasher passwordHasher, IHttpContextAccessor httpContextAccessor, AppSettings appSettings, IMemoryCache cache, IUnitOfWork unitOfWork, ByteFileUtility byteFileUtility)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _httpContextAccessor = httpContextAccessor;
        _appSettings = appSettings;
        _cache = cache;
        _unitOfWork = unitOfWork;
        _byteFileUtility = byteFileUtility;
    }

    public async Task<ServiceResponse<bool>> UserUpsertAsync(UserUpsertDTO userUpsert)
    {
        var userId = userUpsert.Id ?? Guid.NewGuid();

        var passwordSalt = _passwordHasher.GenerateSalt();
        var hashedPassword = _passwordHasher.HashPassword(userUpsert.PassCode, passwordSalt);
        var passCode = _passwordHasher.EncryptPassword(userUpsert.PassCode);

        var user = await _context.Users.Include(x => x.Roles)
            .Include(u => u.Addresses)
                .ThenInclude(a => a.Province)
            .Include(u => u.Addresses)
                .ThenInclude(a => a.City)
            .Include(u => u.Images)
            .Include(u => u.UserSpecification)
                .ThenInclude(us => us.IdCardImages)
                                       .FirstOrDefaultAsync(u => u.Id == userId);

        List<Role>? roles = new List<Role>();
        if (userUpsert.RoleIds is not null)
        {
            roles = await _context.Roles.Where(r => userUpsert.RoleIds.Contains(r.Id)).ToListAsync();
        }

        var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Title == "مشتری");
        if (defaultRole == null)
        {
            return new ServiceResponse<bool>
            {
                Message = "مشکلی در هنگام شناسایی سمت مشتری پیش آمده "
            };
        }

        if (roles == null)
        {
            return new ServiceResponse<bool>
            {
                Message = "مشکلی در هنگام اضافه کردن سمت برای کاربر پیش آمده "
            };
        }

        if (!roles.Any(x => x.Title == defaultRole.Title))
        {
            roles.Add(defaultRole);
        }

        if (user == null)
        {
            var specification = new UserSpecification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Roles = roles.Select(r => r.Title).ToList(),
                IdCardImages = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, UserSpecification>>(userUpsert.IdCardThumbnail!, nameof(UserSpecification), null,false),
                MobileNumber = userUpsert.MobileNumber,
                PasswordSalt = passwordSalt,
                Password = hashedPassword,
                PassCode = passCode,
                FirstName = userUpsert.FirstName,
                FamilyName = userUpsert.FamilyName,
                FatherName = userUpsert.FatherName,
                TelePhone = userUpsert.TelePhone,
                Province = userUpsert.Province,
                City = userUpsert.City,
                PostalCode = userUpsert.PostalCode,
                FirstAddress = userUpsert.FirstAddress,
                SecondAddress = userUpsert.SecondAddress,
                BirthDate = userUpsert.BirthDate,
                IdNumber = userUpsert.IdNumber,
                NationalCode = userUpsert.NationalCode,
                BankAccountNumber = userUpsert.BankAccountNumber,
                ShabaNumber = userUpsert.ShabaNumber,
                IsActive = userUpsert.IsActive,
                Note = userUpsert.Note,
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,

                StoreName = userUpsert.StoreName,
                StoreTelephone = userUpsert.StoreTelephone,
                StoreAddress = userUpsert.StoreAddress,
                BussinessLicenseNumber = userUpsert.BussinessLicenseNumber,
                IsActiveAddProduct = userUpsert.IsActiveAddProduct,
                IsPublishProduct = userUpsert.IsPublishProduct,
                IsSelectedAsSpecialSeller = userUpsert.IsSelectedAsSpecialSeller,
                CommissionType = userUpsert.CommissionType, // nullable
                PercentageValue = userUpsert.PercentageValue,
                SellerPerformance = userUpsert.SellerPerformance,
                TimelySupply = userUpsert.TimelySupply,
                ShippingCommitment = userUpsert.ShippingCommitment,
                NoReturns = userUpsert.NoReturns
            };

            user = new User
            {
                Id = userId,
                UserType = userUpsert.UserType,
                Images = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, User>>(userUpsert.Thumbnail!, nameof(User), null,false),
                UserSpecification = specification,
                Roles = roles,
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
        }
        else
        {
            if (userUpsert.Thumbnail != null)
            {
                if (user.Images != null)
                {
                    _byteFileUtility.DeleteFiles(user.Images, nameof(User),"SubUser");
                }
                user.Images = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, User>>(userUpsert.Thumbnail, nameof(User), null,false);
            }
            if (userUpsert.IdCardThumbnail != null)
            {
                if (user.UserSpecification.IdCardImages != null)
                {
                    _byteFileUtility.DeleteFiles(user.UserSpecification.IdCardImages, nameof(UserSpecification),"SubUserSpecification");
                }
                user.UserSpecification.IdCardImages = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, UserSpecification>>(userUpsert.IdCardThumbnail, nameof(UserSpecification), null,false);
            }
            user.UserType = userUpsert.UserType;
            user.UserSpecification.Roles = roles.Select(r => r.Title).ToList();
            user.UserSpecification.MobileNumber = userUpsert.MobileNumber;
            user.UserSpecification.PasswordSalt = passwordSalt;
            user.UserSpecification.Password = hashedPassword;
            user.UserSpecification.PassCode = passCode;
            user.UserSpecification.FirstName = userUpsert.FirstName;
            user.UserSpecification.FamilyName = userUpsert.FamilyName;
            user.UserSpecification.FatherName = userUpsert.FatherName;
            user.UserSpecification.TelePhone = userUpsert.TelePhone;
            user.UserSpecification.Province = userUpsert.Province;
            user.UserSpecification.City = userUpsert.City;
            user.UserSpecification.PostalCode = userUpsert.PostalCode;
            user.UserSpecification.FirstAddress = userUpsert.FirstAddress;
            user.UserSpecification.SecondAddress = userUpsert.SecondAddress;
            user.UserSpecification.BirthDate = userUpsert.BirthDate;
            user.UserSpecification.IdNumber = userUpsert.IdNumber;
            user.UserSpecification.NationalCode = userUpsert.NationalCode;
            user.UserSpecification.BankAccountNumber = userUpsert.BankAccountNumber;
            user.UserSpecification.ShabaNumber = userUpsert.ShabaNumber;
            user.UserSpecification.IsActive = userUpsert.IsActive;
            user.UserSpecification.Note = userUpsert.Note;

            user.UserSpecification.StoreName = userUpsert.StoreName;
            user.UserSpecification.StoreTelephone = userUpsert.StoreTelephone;
            user.UserSpecification.StoreAddress = userUpsert.StoreAddress;
            user.UserSpecification.BussinessLicenseNumber = userUpsert.BussinessLicenseNumber;
            user.UserSpecification.IsActiveAddProduct = userUpsert.IsActiveAddProduct;
            user.UserSpecification.IsPublishProduct = userUpsert.IsPublishProduct;
            user.UserSpecification.IsSelectedAsSpecialSeller = userUpsert.IsSelectedAsSpecialSeller;
            user.UserSpecification.CommissionType = userUpsert.CommissionType; // nullable
            user.UserSpecification.PercentageValue = userUpsert.PercentageValue;
            user.UserSpecification.SellerPerformance = userUpsert.SellerPerformance;
            user.UserSpecification.TimelySupply = userUpsert.TimelySupply;
            user.UserSpecification.ShippingCommitment = userUpsert.ShippingCommitment;
            user.UserSpecification.NoReturns = userUpsert.NoReturns;

            user.LastUpdated = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true,
        };
    }


    public async Task<ServiceResponse<Guid>> RegisterUserAsync(string mobileNumber, string password)
    {
        var response = new ServiceResponse<Guid>();
        try
        {
            if (await _context.Users.Include(us => us.UserSpecification).AnyAsync(u => u.UserSpecification.MobileNumber == mobileNumber))
            {
                return new ServiceResponse<Guid>
                {
                    Message = "کاربری با این شماره موبایل قبلا به ثبت رسیده"
                };
            }

            var passwordSalt = _passwordHasher.GenerateSalt();
            var hashedPassword = _passwordHasher.HashPassword(password, passwordSalt);
            var passCode = _passwordHasher.EncryptPassword(password);

            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Title == "مشتری");
            if (defaultRole == null)
            {
                return new ServiceResponse<Guid>
                {
                    Message = "مشکلی در هنگام شناسایی سمت مشتری پیش آمده "
                };
            }

            var userId = Guid.NewGuid();
            var specification = new UserSpecification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                MobileNumber = mobileNumber,
                Roles = new List<string> { defaultRole.Title },
                Password = hashedPassword,
                PasswordSalt = passwordSalt,
                PassCode = passCode,
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };
            var user = new User
            {
                Id = userId,
                UserSpecification = specification,
                Roles = new List<Role> { defaultRole },
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            response.Data = user.Id;
            response.Count = 1;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<LoginDTO>> AuthenticateUserAsync(string mobileNumber, string password)
    {
        var response = new ServiceResponse<LoginDTO>();
        try
        {
            var user = await _context.Users.Include(us => us.UserSpecification).Include(u => u.Roles).FirstOrDefaultAsync(u => u.UserSpecification.MobileNumber == mobileNumber);
            if (user == null)
            {
                response.Success = false;
                response.Message = "کاربر مد نظر پیدا نشد";
                return response;
            }

            if (!_passwordHasher.VerifyPassword(password, user.UserSpecification.Password, user.UserSpecification.PasswordSalt))
            {
                response.Success = false;
                response.Message = "کد یا پسورد شما اشتباه است";
                return response;
            }
            var token = CreateToken(user);
            var refreshToken = GenerateRefreshToken(user.Id);
            var userRefreshToken = await _context.UserRefreshTokens
                .SingleOrDefaultAsync(q => q.UserId == user.Id);

            if (userRefreshToken == null)
            {
                // If there is no existing refresh token, create a new one
                userRefreshToken = new UserRefreshToken
                {
                    UserId = user.Id,
                    RefreshToken = refreshToken,
                    RefreshTokenTimeout = _appSettings.AuthSettings.RefreshTokenTimeout,
                    Created = DateTime.UtcNow,
                    IsValid = true
                };

                await _context.UserRefreshTokens.AddAsync(userRefreshToken);
            }
            else
            {
                // If there is an existing refresh token, update its values
                userRefreshToken.RefreshToken = refreshToken;
                userRefreshToken.RefreshTokenTimeout = _appSettings.AuthSettings.RefreshTokenTimeout;
                userRefreshToken.Created = DateTime.UtcNow;
                userRefreshToken.LastUpdated = DateTime.UtcNow;
                userRefreshToken.IsValid = true;
                _context.UserRefreshTokens.Update(userRefreshToken);
            }
            await _unitOfWork.SaveChangesAsync();


            var result = new LoginDTO
            {
                Roles = user.UserSpecification.Roles,
                MobileNumber = user.UserSpecification.MobileNumber,
                FullName = user.UserSpecification.FirstName + " " + user.UserSpecification.FamilyName,
                Token = token,
                ExpireTime = _appSettings.AuthSettings.TokenTimeout,
                RefreshToken = refreshToken,
                RefreshTokenExpireTime = _appSettings.AuthSettings.RefreshTokenTimeout,
                LoggedIn = true
            };
            response.Data = result;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<LoginDTO>> GetUserInfo(string mobileNumber, HttpContext context)
    {

        var response = new ServiceResponse<LoginDTO>();
        try
        {
            var user = await _context.Users.Include(us => us.UserSpecification).Include(u => u.Roles).FirstOrDefaultAsync(u => u.UserSpecification.MobileNumber == mobileNumber);
            if (user == null)
            {
                response.Success = false;
                response.Message = "کاربر مد نظر پیدا نشد";
                return response;
            }

            var token = CreateToken(user);
            var refreshToken = GenerateRefreshToken(user.Id);
            var userRefreshToken = await _context.UserRefreshTokens
                .SingleOrDefaultAsync(q => q.UserId == user.Id);

            if (userRefreshToken == null)
            {
                // If there is no existing refresh token, create a new one
                userRefreshToken = new UserRefreshToken
                {
                    UserId = user.Id,
                    RefreshToken = refreshToken,
                    RefreshTokenTimeout = _appSettings.AuthSettings.RefreshTokenTimeout,
                    Created = DateTime.UtcNow,
                    IsValid = true
                };

                await _context.UserRefreshTokens.AddAsync(userRefreshToken);
            }

            await _unitOfWork.SaveChangesAsync();


            var result = new LoginDTO
            {
                Roles = user.UserSpecification.Roles,
                MobileNumber = user.UserSpecification.MobileNumber,
                FullName = user.UserSpecification.FirstName + " " + user.UserSpecification.FamilyName,
                Token = token,
                ExpireTime = _appSettings.AuthSettings.TokenTimeout,
                RefreshToken = refreshToken,
                RefreshTokenExpireTime = _appSettings.AuthSettings.RefreshTokenTimeout,
                LoggedIn = true
            };
            response.Data = result;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<List<User>>> GetAllUsersAsync()
    {
        var response = new ServiceResponse<List<User>>();
        try
        {
            var users = await _context.Users.Include(x => x.Roles)
            .Include(u => u.Addresses)
                .ThenInclude(a => a.Province)
            .Include(u => u.Addresses)
                .ThenInclude(a => a.City)
            .Include(u => u.Images)
            .Include(u => u.UserSpecification)
                .ThenInclude(us => us.IdCardImages).ToListAsync();
            response.Data = users;
            response.Count = users.Count;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }

    public Guid GetUserId()
    {
        return Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.MobilePhone, user.UserSpecification.MobileNumber),
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.AuthSettings.TokenKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(claims: claims,
            expires: DateTime.Now.AddDays(_appSettings.AuthSettings.TokenTimeout), signingCredentials: creds);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }

    public async Task<ServiceResponse<GenerateNewTokenResultDTO>> GenerateNewToken(GenerateNewTokenDTO command)
    {
        var userRefreshToken = await _context.UserRefreshTokens
        .SingleOrDefaultAsync(q => q.RefreshToken == command.RefreshToken);

        var userId = ValidateRefreshToken(command.RefreshToken);
        var user = await _context.Users.Include(u => u.Roles).Include(u => u.Images).Include(u => u.UserSpecification).FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            return new ServiceResponse<GenerateNewTokenResultDTO>()
            {
                Data = null,
                Success = false,
                Message = "User not found"
            };
        }
        if (userRefreshToken is null)
        {
            return new ServiceResponse<GenerateNewTokenResultDTO>()
            {
                Data = null,
                Success = false,
                Message = "RefreshToken not found"
            };
        }

        var token = CreateToken(user);
        var refreshToken = GenerateRefreshToken(userRefreshToken.UserId);

        // Insert or update refresh token in db
        var currentRefreshToken = await _context.UserRefreshTokens
            .SingleOrDefaultAsync(q => q.UserId == userRefreshToken.UserId);

        if (currentRefreshToken is null)
        {
            // If there is no existing refresh token, create a new one
            currentRefreshToken = new UserRefreshToken
            {
                UserId = userRefreshToken.UserId,
                RefreshToken = refreshToken,
                RefreshTokenTimeout = _appSettings.AuthSettings.RefreshTokenTimeout,
                Created = DateTime.UtcNow,
                IsValid = true
            };

            await _context.UserRefreshTokens.AddAsync(userRefreshToken);
        }
        else
        {
            currentRefreshToken.RefreshToken = refreshToken;
            currentRefreshToken.RefreshTokenTimeout = _appSettings.AuthSettings.RefreshTokenTimeout;
            currentRefreshToken.Created = DateTime.UtcNow;
            currentRefreshToken.IsValid = true;
        }

        await _unitOfWork.SaveChangesAsync();

        var result = new GenerateNewTokenResultDTO
        {
            MobileNumber = user.UserSpecification.MobileNumber,
            FullName = user.UserSpecification.FirstName + " " + user.UserSpecification.FamilyName,
            Token = token,
            RefreshToken = refreshToken,
            ExpireTime = _appSettings.AuthSettings.RefreshTokenTimeout,
            LoggedIn = true
        };

        return new ServiceResponse<GenerateNewTokenResultDTO>
        {
            Data = result
        };
    }

    private string GenerateRefreshToken(Guid userId)
    {
        var randomNumber = new byte[32];
        var refreshToken = "";
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            refreshToken = Convert.ToBase64String(randomNumber);
        }
        var userCache = new UserCacheDto { UserId = userId };
        var serializedUser = JsonConvert.SerializeObject(userCache);
        var cacheData = Encoding.UTF8.GetBytes(serializedUser);
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddDays(_appSettings.AuthSettings.RefreshTokenTimeout)
        };
        _cache.Set(refreshToken, cacheData, cacheOptions);
        return refreshToken;
    }

    private Guid ValidateRefreshToken(string refreshToken)
    {
        if (!_cache.TryGetValue(refreshToken, out var cacheData))
        {
            throw new Exception("Invalid Refresh Token");
        }
        var serializedUser = cacheData != null ? Encoding.UTF8.GetString((byte[])cacheData) : null;
        var userCache = JsonConvert.DeserializeObject<UserCacheDto>(serializedUser!);
        return userCache!.UserId;
    }

    public async Task<ServiceResponse<Pagination<UserDTO>>> GetUsers(RequestQuery requestQuery)
    {
        var pageSize = requestQuery.PageSize ?? 15;
        var totalCount = await _context.Users.CountAsync();
        var lastPage = (int)Math.Ceiling((double)totalCount / pageSize);
        var pageNumber = requestQuery.PageNumber ?? 1;
        var skipCount = (pageNumber - 1) * pageSize;
        var users = await _context.Users.Include(x => x.Roles)
            .Include(u => u.Addresses)
                .ThenInclude(a => a.Province)
            .Include(u => u.Addresses)
                .ThenInclude(a => a.City)
            .Include(u => u.Images)
            .Include(u => u.UserSpecification)
                .ThenInclude(us => us.IdCardImages)
            .Skip(skipCount)
            .Take(pageSize)
            .ToListAsync();
        var userDto = users.Select(user => new UserDTO
        {
            Id = user.Id,
            ImageSrc = user.UserSpecification != null && user.Images != null
            ? _byteFileUtility.GetEncryptedFileActionUrl(user.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl ?? string.Empty,
                Placeholder = img.Placeholder ?? string.Empty
            }).ToList(), nameof(User),"SubUser").First()
            : null,
            FullName = (user.UserSpecification?.FirstName ?? string.Empty) + " " + (user.UserSpecification?.FamilyName ?? string.Empty),
            Roles = user.UserSpecification?.Roles,
            MobileNumber = user.UserSpecification?.MobileNumber ?? string.Empty,
            Email = user.UserSpecification?.Email ?? string.Empty,
            LastActivity = user.UserSpecification?.LastActivity,
            OrderCount = 0,
            City = user.UserSpecification?.City,
            Wallet = false,
            IsActive = user.UserSpecification?.IsActive ?? false,
            UserSpecification = new UserSpecificationDTO
            {
                UserId = user.UserSpecification?.UserId ?? Guid.Empty,
                Id = user.UserSpecification?.Id ?? Guid.Empty,
                UserType = user.UserType,
                Roles = user.Roles?.ToList(),
                IsActive = user.UserSpecification?.IsActive ?? false,
                ImageScr = user.Images != null
                ? _byteFileUtility.GetEncryptedFileActionUrl(user.Images.Select(img => new EntityImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl ?? string.Empty,
                    Placeholder = img.Placeholder ?? string.Empty
                }).ToList(), nameof(User),"SubUser").First()
                : null,
                IdCardImageSrc = user.UserSpecification?.IdCardImages != null
                ? _byteFileUtility.GetEncryptedFileActionUrl(user.UserSpecification.IdCardImages.Select(img => new EntityImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl ?? string.Empty,
                    Placeholder = img.Placeholder ?? string.Empty
                }).ToList(), nameof(UserSpecification),"SubUserSpecification").First()
                : null,
                MobileNumber = user.UserSpecification?.MobileNumber ?? string.Empty,
                PassCode = user.UserSpecification != null ? _passwordHasher.DecryptPassword(user.UserSpecification.PassCode) : string.Empty,
                FirstName = user.UserSpecification?.FirstName ?? string.Empty,
                FamilyName = user.UserSpecification?.FamilyName ?? string.Empty,
                FatherName = user.UserSpecification?.FatherName ?? string.Empty,
                TelePhone = user.UserSpecification?.TelePhone ?? string.Empty,
                Province = user.UserSpecification?.Province ?? string.Empty,
                City = user.UserSpecification?.City ?? string.Empty,
                PostalCode = user.UserSpecification?.PostalCode ?? string.Empty,
                FirstAddress = user.UserSpecification?.FirstAddress ?? string.Empty,
                SecondAddress = user.UserSpecification?.SecondAddress ?? string.Empty,
                BirthDate = user.UserSpecification?.BirthDate ?? string.Empty,
                IdNumber = user.UserSpecification?.IdNumber ?? string.Empty,
                NationalCode = user.UserSpecification?.NationalCode ?? string.Empty,
                BankAccountNumber = user.UserSpecification?.BankAccountNumber ?? string.Empty,
                ShabaNumber = user.UserSpecification?.ShabaNumber ?? string.Empty,
                Note = user.UserSpecification?.Note ?? string.Empty,
                Created = user.UserSpecification?.Created ?? DateTime.MinValue,
                LastUpdated = user.UserSpecification?.LastUpdated ?? DateTime.MinValue,

                StoreName = user.UserSpecification?.StoreName ?? string.Empty,
                StoreTelephone = user.UserSpecification?.StoreTelephone ?? string.Empty,
                StoreAddress = user.UserSpecification?.StoreAddress ?? string.Empty,
                BussinessLicenseNumber = user.UserSpecification?.BussinessLicenseNumber ?? string.Empty,
                IsActiveAddProduct = user.UserSpecification?.IsActiveAddProduct ?? false,
                IsPublishProduct = user.UserSpecification?.IsPublishProduct ?? false,
                IsSelectedAsSpecialSeller = user.UserSpecification?.IsSelectedAsSpecialSeller ?? false,
                CommissionType = user.UserSpecification?.CommissionType,
                PercentageValue = user.UserSpecification?.PercentageValue ?? string.Empty,
                SellerPerformance = user.UserSpecification?.SellerPerformance ?? string.Empty,
                TimelySupply = user.UserSpecification?.TimelySupply ?? string.Empty,
                ShippingCommitment = user.UserSpecification?.ShippingCommitment ?? string.Empty,
                NoReturns = user.UserSpecification?.NoReturns ?? string.Empty,
            },
            Created = user.Created,
            LastUpdated = user.LastUpdated
        }).ToList();

        var pagination = new Pagination<UserDTO>
        {
            CurrentPage = pageNumber,
            NextPage = pageNumber + 1,
            PreviousPage = pageNumber > 1 ? pageNumber - 1 : 0,
            HasNextPage = (skipCount + pageSize) < totalCount,
            HasPreviousPage = pageNumber > 1,
            LastPage = (int)Math.Ceiling((decimal)totalCount / pageSize),
            TotalCount = totalCount,
            Data = userDto
        };

        return new ServiceResponse<Pagination<UserDTO>>
        {
            Data = pagination
        };
    }

    public async Task<ServiceResponse<UserDTO>> GetUserBy(Guid userId)
    {
        var data = await _context.Users
            .Include(x => x.Roles)
            .Include(u => u.Addresses)
                .ThenInclude(a => a.Province)
            .Include(u => u.Addresses)
                .ThenInclude(a => a.City)
            .Include(u => u.Images)
            .Include(u => u.UserSpecification)
                .ThenInclude(us => us.IdCardImages)
            .FirstOrDefaultAsync(u => u.Id == userId);


        var user = new UserDTO
        {
            Id = data!.Id,
            ImageSrc = data.UserSpecification != null && data.Images?.Count > 0 ? _byteFileUtility.GetEncryptedFileActionUrl(data.Images.Select(img => new EntityImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl ?? string.Empty,
                Placeholder = img.Placeholder ?? string.Empty
            }).ToList(), nameof(User),"SubUser").First() : null,
            FullName = data.UserSpecification != null ? data.UserSpecification!.FirstName + " " + data.UserSpecification!.FamilyName : string.Empty,
            Roles = data.UserSpecification?.Roles,
            MobileNumber = data.UserSpecification!.MobileNumber,
            Email = data.UserSpecification.Email,
            LastActivity = data.UserSpecification.LastActivity,
            OrderCount = 0,
            City = data.UserSpecification.City,
            Wallet = false,
            IsActive = data.UserSpecification.IsActive,
            UserSpecification = new UserSpecificationDTO
            {
                UserId = data.UserSpecification.UserId,
                Id = data.UserSpecification.Id,
                UserType = data.UserType,
                Roles = data.Roles.ToList(),
                Gender = data.UserSpecification.Gender,
                IsActive = data.UserSpecification.IsActive,
                ImageScr = data.Images?.Count > 0 ? _byteFileUtility.GetEncryptedFileActionUrl(data.Images.Select(img => new EntityImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl ?? string.Empty,
                    Placeholder = img.Placeholder ?? string.Empty
                }).ToList(), nameof(User),"SubUser").First() : null,
                IdCardImageSrc = data.UserSpecification?.IdCardImages?.Count > 0 ? _byteFileUtility.GetEncryptedFileActionUrl(data.UserSpecification.IdCardImages.Select(img => new EntityImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl ?? string.Empty,
                    Placeholder = img.Placeholder ?? string.Empty
                }).ToList(), nameof(UserSpecification),"SubUserSpecification").First() : null,
                MobileNumber = data.UserSpecification!.MobileNumber,
                Email = data.UserSpecification.Email,
                PassCode = _passwordHasher.DecryptPassword(data.UserSpecification.PassCode),
                FirstName = data.UserSpecification.FirstName,
                FamilyName = data.UserSpecification.FamilyName,
                FatherName = data.UserSpecification.FatherName,
                TelePhone = data.UserSpecification.TelePhone,
                Province = data.UserSpecification.Province,
                City = data.UserSpecification.City,
                PostalCode = data.UserSpecification.PostalCode,
                FirstAddress = data.UserSpecification.FirstAddress,
                SecondAddress = data.UserSpecification.SecondAddress,
                BirthDate = data.UserSpecification.BirthDate,
                IdNumber = data.UserSpecification.IdNumber,
                NationalCode = data.UserSpecification.NationalCode,
                BankAccountNumber = data.UserSpecification.BankAccountNumber,
                ShabaNumber = data.UserSpecification.ShabaNumber,
                Note = data.UserSpecification.Note,
                Created = data.UserSpecification.Created,
                LastUpdated = data.UserSpecification.LastUpdated
            },
            Addresses = data.Addresses,
            Created = data.Created,
            LastUpdated = data.LastUpdated
        };

        return new ServiceResponse<UserDTO>
        {
            Data = user
        };
    }

    public async Task<ServiceResponse<UserDTO>> GetUserInfoMe(HttpContext context)
    {
        var userId = GetUserId();
        var result = (await GetUserBy(userId)).Data;

        return new ServiceResponse<UserDTO>
        {
            Data = result
        };
    }

    public async Task<ServiceResponse<Guid>> EditUserProfileAsync(UserProfileUpdateDTO userProfileUpdate)
    {
        var userId = GetUserId();
        var user = await _context.Users
                                .Include(u => u.Roles)
                                .Include(u => u.Images)
                                .Include(u => u.UserSpecification)
                                .ThenInclude(u => u.IdCardImages)
                                .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {

            return new ServiceResponse<Guid>
            {
                Success = false,
                Message = "کاربری با این آیدی پیدا نشد",
            };
        }

        user.UserSpecification.MobileNumber = userProfileUpdate.MobileNumber;
        user.UserSpecification.Gender = userProfileUpdate.Gender;
        user.UserSpecification.FirstName = userProfileUpdate.FirstName;
        user.UserSpecification.FamilyName = userProfileUpdate.FamilyName;
        user.UserSpecification.NationalCode = userProfileUpdate.NationalCode;
        user.UserSpecification.BirthDate = userProfileUpdate.BirthDate;
        user.UserSpecification.BankAccountNumber = userProfileUpdate.BankAccountNumber;
        user.UserSpecification.ShabaNumber = userProfileUpdate.ShabaNumber;
        user.UserSpecification.Email = userProfileUpdate.Email;

        _context.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<Guid>
        {
            Data = userId
        };
    }

    public async Task<ServiceResponse<bool>> UpsertPermission(PermissionUpsertDTO permissionUpsert)
    {
        string message = "";
        var permissionDb = await _context.Permissions.FirstOrDefaultAsync(x => x.Id == permissionUpsert.Id);
        if (permissionDb is null)
        {
            // Create new HeaderText
            var newPermission = new Permission
            {
                Id = Guid.NewGuid(),
                Name = permissionUpsert.Name,
                IsActive = permissionUpsert.IsActive,
                ParentPermissionId = permissionUpsert.ParentPermissionId,
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };
            _context.Permissions.Add(newPermission);
            message = "دسترسی با موفقیت ایجاد شد";
        }
        else
        {
            // Update existing Permission
            permissionDb.Name = permissionUpsert.Name;
            permissionDb.IsActive = permissionUpsert.IsActive;
            permissionDb.ParentPermissionId = permissionUpsert.ParentPermissionId;
            permissionDb.LastUpdated = DateTime.UtcNow;
            message = "دسترسی با موفقیت بروزرسانی شد";
        }

        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool> { Data = true, Message = message };
    }

    public async Task<ServiceResponse<List<Permission>>> GetPermissions()
    {
        var permissions = await _context.Permissions.Include(x => x.ChildPermissions).ToListAsync();

        var permissionsDto = BuildPermissions(permissions);

        return new ServiceResponse<List<Permission>>
        {
            Count = permissions.Count,
            Data = permissionsDto
        };
    }
    private List<Permission> BuildPermissions(List<Permission> permissions)
    {
        return permissions.Select(permission => new Permission
        {
            Id = permission.Id,
            Name = permission.Name,
            IsActive = permission.IsActive,
            ParentPermissionId = permission.ParentPermissionId,
            ParentPermission = permission.ParentPermission != null ? new Permission { Id = permission.ParentPermission.Id, Name = permission.ParentPermission.Name, ParentPermissionId = permission.ParentPermissionId } : null,
            ChildPermissions = BuildPermissions(permission.ChildPermissions ?? []),
            Created = permission.Created,
            LastUpdated = permission.LastUpdated
        }).ToList();
    }

    public async Task<ServiceResponse<bool>> UpsertRole(RoleUpsertDTO roleUpsert)
    {
        string message = "";
        var roleDb = await _context.Roles.Include(x => x.Permissions).FirstOrDefaultAsync(x => x.Id == roleUpsert.Id);
        var permissionDb = await _context.Permissions.Where(x => roleUpsert.PermissionIds.Contains(x.Id)).ToListAsync();
        if (roleDb is null)
        {
            // Create new HeaderText
            var newRole = new Role
            {
                Id = Guid.NewGuid(),
                Title = roleUpsert.Title,
                IsActive = roleUpsert.IsActive,
                Permissions = permissionDb,
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };
            _context.Roles.Add(newRole);
            message = "سمت با موفقیت ایجاد شد";
        }
        else
        {
            // Update existing Role
            roleDb.Title = roleUpsert.Title;
            roleDb.Permissions = permissionDb;
            roleDb.IsActive = roleUpsert.IsActive;
            roleDb.LastUpdated = DateTime.UtcNow;
            message = "سمت با موفقیت بروزرسانی شد";
        }

        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool> { Data = true, Message = message };
    }

    public async Task<ServiceResponse<List<Role>>> GetRoles()
    {

        var roles = await _context.Roles.Include(x => x.Permissions).ThenInclude(x => x.ChildPermissions).ToListAsync();

        var rolesDto = roles.Select(role => new Role
        {
            Id = role.Id,
            Title = role.Title,
            IsActive = role.IsActive,
            Permissions = BuildPermissions(role.Permissions),
            Created = role.Created,
            LastUpdated = role.LastUpdated
        }).ToList();
        return new ServiceResponse<List<Role>>
        {
            Count = roles.Count,
            Data = roles
        };
    }
}
