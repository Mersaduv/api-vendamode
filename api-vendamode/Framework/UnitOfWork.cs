using api_vendamode.Data;
using api_vendamode.Interfaces;

namespace api_vendamode.Framework
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