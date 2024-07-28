using api_vendace.Data;
using api_vendace.Entities.Users;
using api_vendace.Interfaces;
using api_vendace.Interfaces.IServices;
using api_vendace.Models;
using api_vendace.Models.Query;
using api_vendamode.Interfaces.IServices;
using Microsoft.EntityFrameworkCore;

namespace api_vendamode.Services.Auth;

public class AddressServices : IAddressServices
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserServices _userServices;

    public AddressServices(ApplicationDbContext context, IUnitOfWork unitOfWork, IUserServices userServices)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _userServices = userServices;
    }

    public async Task<ServiceResponse<bool>> AddAddress(Address address)
    {
        var userId = _userServices.GetUserId();
        if (await _context.Addresses.FirstOrDefaultAsync(b => b.PostalCode == address.PostalCode) != null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "آدرس وارد شده قبلا {مطابق با کد پستی} ایجاد شده."
            };
        }
        address.Id = Guid.NewGuid();
        address.UserId = userId;
        address.Created = DateTime.UtcNow;
        address.LastUpdated = DateTime.UtcNow;
        var result = await _context.Addresses.AddAsync(address);
        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool>
        {
            Data = true,
            Message = "آدرس با موفقیت ایجاد شد"
        };
    }

    public async Task<ServiceResponse<bool>> DeleteAddress(Guid id)
    {
        var address = await _context.Addresses.FirstOrDefaultAsync(b => b.Id == id);
        if (address == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "آدرس پیدا نشد"
            };
        }
        _context.Addresses.Remove(address);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true ,
            Message = "آدرس مد نظر با موفقیت حذف شد"
        };
    }

    public async Task<ServiceResponse<Pagination<Address>>> GetAddresses(RequestQuery requestQuery)
    {
        var pageSize = requestQuery.PageSize ?? 15;
        var totalCount = await _context.Addresses.CountAsync();
        var lastPage = (int)Math.Ceiling((double)totalCount / pageSize);
        var pageNumber = requestQuery.PageNumber ?? 1;
        var addresses = await _context.Addresses
        .Include(a => a.City)
        .Include(a => a.Province)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var hasPreviousPage = pageNumber > 1;
        var hasNextPage = pageNumber < lastPage;
        var previousPage = hasPreviousPage ? pageNumber - 1 : 0;
        var nextPage = hasNextPage ? pageNumber + 1 : 0;

        var pagination = new Pagination<Address>
        {
            CurrentPage = pageNumber,
            NextPage = nextPage,
            PreviousPage = previousPage,
            HasNextPage = hasNextPage,
            HasPreviousPage = hasPreviousPage,
            LastPage = lastPage,
            Data = addresses,
            TotalCount = totalCount
        };

        return new ServiceResponse<Pagination<Address>>
        {
            Data = pagination
        };
    }

    public async Task<ServiceResponse<IReadOnlyList<Address>>> GetAllAddresses()
    {
        var result = await _context.Addresses.ToListAsync();

        return new ServiceResponse<IReadOnlyList<Address>>
        {
            Data = result
        };
    }

    public async Task<ServiceResponse<Address>> GetBy(Guid id)
    {
        var address = await _context.Addresses.FirstOrDefaultAsync(b => b.Id == id);
        if (address == null)
        {
            return new ServiceResponse<Address>
            {
                Success = false,
                Message = "برندی پیدا نشد"
            };
        }

        return new ServiceResponse<Address>
        {
            Data = address
        };
    }

    public async Task<ServiceResponse<bool>> UpsertAddress(Address address)
    {
        var userId = _userServices.GetUserId();
        var addressDb = await _context.Addresses.FirstOrDefaultAsync(b => b.Id == address.Id);
        if (addressDb == null)
        {
            var result = await AddAddress(address);
            return result;
        }

        addressDb.Id = address.Id;
        addressDb.UserId = userId;
        addressDb.FullAddress = address.FullAddress;
        addressDb.FullName = address.FullName;
        addressDb.MobileNumber = address.MobileNumber;
        addressDb.PostalCode = address.PostalCode;
        addressDb.Province = address.Province;
        addressDb.City = address.City;
        addressDb.LastUpdated = DateTime.UtcNow;

        _context.Addresses.Update(addressDb);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Data = true
        };
    }
}