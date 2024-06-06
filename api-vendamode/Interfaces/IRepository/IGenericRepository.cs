using api_vendace.Models;

namespace api_vendace.Interfaces.IRepository;

public interface IGenericRepository<T> where T : BaseClass<int>
{
    Task<T?> GetAsyncBy(int id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<int> Add(T entity);
    Task<int> Update(T entity);
    Task<int> Delete(int id);
    Task<int> GetTotalCountAsync();
}