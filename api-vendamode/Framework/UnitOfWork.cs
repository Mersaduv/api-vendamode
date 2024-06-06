using api_vendace.Data;
using api_vendace.Interfaces;

namespace api_vendace.Framework
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext applicationDbContext;

        public UnitOfWork(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public void Dispose()
        {
            applicationDbContext.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await applicationDbContext.SaveChangesAsync();
        }
    }

}