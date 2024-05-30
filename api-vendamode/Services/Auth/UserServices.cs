using System.Security.Claims;
using api_vendamode.Data;
using api_vendamode.Entities.Users;
using api_vendamode.Entities.Users.Security;
using api_vendamode.Interfaces.IServices;
using api_vendamode.Models;
using api_vendamode.Models.Dtos.AuthDto;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Caching.Memory;
using api_vendamode.Interfaces;
using System.Security.Cryptography;
using Newtonsoft.Json;
using api_vendamode.Utility;
using api_vendamode.Entities;
using api_vendamode.Models.Query;
using api_vendamode.Models.Dtos;

namespace api_vendamode.Services.Auth;

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

    public async Task<ServiceResponse<bool>> CreateUserAsync(UserCreateDTO userCreate)
    {
        var passwordSalt = _passwordHasher.GenerateSalt();
        var hashedPassword = _passwordHasher.HashPassword(userCreate.PassCode, passwordSalt);
        var userId = Guid.NewGuid();

        var specification = new UserSpecification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Images = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, User>>(userCreate.Thumbnail!, nameof(User), false),
            IdCardImages = _byteFileUtility.SaveFileInFolder<EntityImage<Guid, UserSpecification>>(userCreate.IdCardThumbnail!, nameof(UserSpecification), false),
            MobileNumber = userCreate.MobileNumber,
            PasswordSalt = passwordSalt,
            Password = hashedPassword,
            FirstName = userCreate.FirstName,
            FamilyName = userCreate.FamilyName,
            FatherName = userCreate.FatherName,
            TelePhone = userCreate.TelePhone,
            Province = userCreate.Province,
            City = userCreate.City,
            PostalCode = userCreate.PostalCode,
            FirstAddress = userCreate.FirstAddress,
            SecondAddress = userCreate.SecondAddress,
            BirthDate = userCreate.BirthDate,
            IdNumber = userCreate.IdNumber,
            NationalCode = userCreate.NationalCode,
            BankAccountNumber = userCreate.BankAccountNumber,
            ShabaNumber = userCreate.ShabaNumber,
            IsActive = userCreate.IsActive,
            Note = userCreate.Note,
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Title == "customer");
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == userCreate.RoleId);
        if (defaultRole == null)
        {
            return new ServiceResponse<bool>
            {
                Message = "مشکلی در هنگام شناسایی سمت مشتری پیش آمده "
            };
        }
        if (role == null)
        {
            return new ServiceResponse<bool>
            {
                Message = "مشکلی در هنگام اضافه کردن سمت برای کاربر پیش آمده "
            };
        }
        var user = new User
        {
            Id = userId,
            UserType = userCreate.UserType,
            UserSpecification = specification,
            Roles = new List<Role> { defaultRole, role },
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        return new ServiceResponse<bool>
        {
            Data = true,
        };
    }

    public async Task<ServiceResponse<Guid>> RegisterUserAsync(string mobileNumber, string passCode)
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
            var hashedPassword = _passwordHasher.HashPassword(passCode, passwordSalt);

            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Title == "customer");
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
                Password = hashedPassword,
                PasswordSalt = passwordSalt,
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };
            var user = new User
            {
                UserSpecification = specification,
                Roles = new List<Role> { defaultRole }
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

    public async Task<ServiceResponse<LoginDTO>> AuthenticateUserAsync(string mobileNumber, string passCode)
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

            if (!_passwordHasher.VerifyPassword(passCode, user.UserSpecification.Password, user.UserSpecification.PasswordSalt))
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
            }
            await _unitOfWork.SaveChangesAsync();


            var result = new LoginDTO
            {
                MobileNumber = user.UserSpecification.MobileNumber,
                FullName = user.UserSpecification.FirstName + " " + user.UserSpecification.FamilyName,
                Token = token,
                ExpireTime = _appSettings.AuthSettings.TokenTimeout,
                RefreshToken = refreshToken,
                RefreshTokenExpireTime = _appSettings.AuthSettings.RefreshTokenTimeout
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

    public async Task<ServiceResponse<User>> GetUserByIdAsync(Guid userId)
    {
        var response = new ServiceResponse<User>();
        try
        {
            var user = await _context.Users.Include(u => u.Roles).Include(us => us.UserSpecification).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return new ServiceResponse<User>
                {
                    Message = "کاربری با این آیدی پیدا نشد"
                };
            }

            response.Data = user;
            response.Count = 1;
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
            var users = await _context.Users.Include(u => u.Roles).Include(us => us.UserSpecification).ToListAsync();
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

    public Guid GetUserId() => Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

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
            expires: DateTime.Now.AddMinutes(_appSettings.AuthSettings.TokenTimeout), signingCredentials: creds);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }

    public async Task<ServiceResponse<GenerateNewTokenDTO>> GenerateNewToken(GenerateNewTokenDTO command)
    {
        var userRefreshToken = await _context.UserRefreshTokens
        .SingleOrDefaultAsync(q => q.RefreshToken == command.RefreshToken);

        var userId = ValidateRefreshToken(command.RefreshToken);
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            return new ServiceResponse<GenerateNewTokenDTO>()
            {
                Data = null,
                Success = false,
                Message = "User not found"
            };
        }
        if (userRefreshToken is null)
        {
            return new ServiceResponse<GenerateNewTokenDTO>()
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

        var result = new GenerateNewTokenDTO
        {
            Token = token,
            RefreshToken = refreshToken
        };

        return new ServiceResponse<GenerateNewTokenDTO>
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
            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(_appSettings.AuthSettings.RefreshTokenTimeout)
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
        var totalCount = await _context.Users.CountAsync();
        var lastPage = (int)Math.Ceiling((double)totalCount / requestQuery.PageSize);
        var pageNumber = requestQuery.PageNumber ?? 1;
        var pageSize = requestQuery.PageSize;
        var users = await _context.Users.Include(us => us.UserSpecification)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(user => new UserDTO
            {
                Id = user.Id,
                ImageSrc = user.UserSpecification != null && user.UserSpecification.Images != null ? _byteFileUtility.GetEncryptedFileActionUrl(user.UserSpecification.Images.Select(img => new EntityImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl ?? string.Empty,
                    Placeholder = img.Placeholder ?? string.Empty
                }).ToList(), nameof(User)).First() : null,
                FullName = user.UserSpecification != null ? user.UserSpecification.FirstName : string.Empty + " " + user.UserSpecification.FamilyName,
                RoleNames = user.Roles.Select(role => role.Title).ToList(),
                MobileNumber = user.UserSpecification.MobileNumber,
                LastActivity = user.UserSpecification.LastActivity,
                OrderCount = 0,
                City = user.UserSpecification.City,
                Wallet = false,
                IsActive = user.UserSpecification.IsActive,
                UserSpecification = new UserSpecificationDTO 
                {
                    Id = user.UserSpecification.Id,
                    UserType = user.UserType,
                    Roles = user.Roles.ToList(),
                    IsActive  = user.UserSpecification.IsActive,
                    ImageScr =  user.UserSpecification.Images != null ? _byteFileUtility.GetEncryptedFileActionUrl(user.UserSpecification.Images.Select(img => new EntityImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl ?? string.Empty,
                    Placeholder = img.Placeholder ?? string.Empty
                }).ToList(), nameof(User)).First() : null,
                IdCardImageSrc = user.UserSpecification.IdCardImages != null ? _byteFileUtility.GetEncryptedFileActionUrl(user.UserSpecification.IdCardImages.Select(img => new EntityImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl ?? string.Empty,
                    Placeholder = img.Placeholder ?? string.Empty
                }).ToList(), nameof(UserSpecification)).First() : null,
                MobileNumber = user.UserSpecification.MobileNumber,
                PassCode =
                },
                Created = user.Created,
                LastUpdated = user.LastUpdated
            })
            .ToListAsync();
    }
}
