using api_vendace.Data;
using api_vendace.Interfaces;
using api_vendace.Models;
using api_vendace.Models.Query;
using api_vendamode.Entities;
using api_vendamode.Interfaces.IServices;
using api_vendamode.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace api_vendamode.Services.Auth;

public class CanceledServices : ICanceledServices
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public CanceledServices(ApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResponse<bool>> AddCanceled(CanceledDTO canceledDto)
    {
        if (await _context.Canceleds.AnyAsync(b => b.Title == canceledDto.Title))
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "عنوان وارد شده قبلا ایجاد شده است."
            };
        }
        var canceled = new Canceled
        {
            Id = Guid.NewGuid(),
            Title = canceledDto.Title,
            IsActive = canceledDto.IsActive,
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow
        };
        await _context.Canceleds.AddAsync(canceled);
        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool> { Data = true };
    }

    public async Task<ServiceResponse<bool>> DeleteCanceled(Guid id)
    {
        var canceled = await _context.Canceleds.FindAsync(id);
        if (canceled == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "موردی پیدا نشد"
            };
        }
        _context.Canceleds.Remove(canceled);
        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool> { Data = true };
    }

    public async Task<ServiceResponse<Pagination<Canceled>>> GetCanceleds(RequestQuery requestQuery)
    {
        var pageSize = requestQuery.PageSize ?? 15;
        var totalCount = await _context.Canceleds.CountAsync();
        var lastPage = (int)Math.Ceiling((double)totalCount / pageSize);
        var pageNumber = requestQuery.PageNumber ?? 1;
        var canceleds = await _context.Canceleds
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var hasPreviousPage = pageNumber > 1;
        var hasNextPage = pageNumber < lastPage;
        var previousPage = hasPreviousPage ? pageNumber - 1 : 0;
        var nextPage = hasNextPage ? pageNumber + 1 : 0;

        var pagination = new Pagination<Canceled>
        {
            CurrentPage = pageNumber,
            NextPage = nextPage,
            PreviousPage = previousPage,
            HasNextPage = hasNextPage,
            HasPreviousPage = hasPreviousPage,
            LastPage = lastPage,
            Data = canceleds,
            TotalCount = totalCount
        };

        return new ServiceResponse<Pagination<Canceled>> { Data = pagination };
    }

    public async Task<ServiceResponse<Canceled>> GetBy(Guid id)
    {
        var canceled = await _context.Canceleds.FindAsync(id);
        if (canceled == null)
        {
            return new ServiceResponse<Canceled>
            {
                Success = false,
                Message = "موردی پیدا نشد"
            };
        }
        return new ServiceResponse<Canceled> { Data = canceled };
    }

    public async Task<ServiceResponse<bool>> UpsertCanceled(CanceledDTO canceled)
    {
        var existingCanceled = await _context.Canceleds.FindAsync(canceled.Id);
        if (existingCanceled == null)
        {
            return await AddCanceled(canceled);
        }

        existingCanceled.Title = canceled.Title;
        existingCanceled.IsActive = canceled.IsActive;
        existingCanceled.Updated = DateTime.UtcNow;

        _context.Canceleds.Update(existingCanceled);
        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool> { Data = true };
    }

    public async Task<ServiceResponse<bool>> AddReturned(ReturnedDTO returned)
    {
        if (await _context.Returneds.AnyAsync(b => b.Title == returned.Title))
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "عنوان وارد شده قبلا ایجاد شده است."
            };
        }
        var returnedDb = new Returned
        {
            Id = Guid.NewGuid(),
            Title = returned.Title,
            IsActive = returned.IsActive,
            CanceledType = returned.CanceledType,
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow
        };
        await _context.Returneds.AddAsync(returnedDb);
        await _unitOfWork.SaveChangesAsync();
        return new ServiceResponse<bool> { Data = true };

    }

    public async Task<ServiceResponse<Pagination<Returned>>> GetReturned(RequestQuery requestQuery)
    {
        var pageSize = requestQuery.PageSize ?? 15;
        var totalCount = await _context.Returneds.CountAsync();
        var lastPage = (int)Math.Ceiling((double)totalCount / pageSize);
        var pageNumber = requestQuery.PageNumber ?? 1;
        var returneds = await _context.Returneds
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var hasPreviousPage = pageNumber > 1;
        var hasNextPage = pageNumber < lastPage;
        var previousPage = hasPreviousPage ? pageNumber - 1 : 0;
        var nextPage = hasNextPage ? pageNumber + 1 : 0;

        var pagination = new Pagination<Returned>
        {
            CurrentPage = pageNumber,
            NextPage = nextPage,
            PreviousPage = previousPage,
            HasNextPage = hasNextPage,
            HasPreviousPage = hasPreviousPage,
            LastPage = lastPage,
            Data = returneds,
            TotalCount = totalCount
        };

        return new ServiceResponse<Pagination<Returned>> { Data = pagination };
    }
}

