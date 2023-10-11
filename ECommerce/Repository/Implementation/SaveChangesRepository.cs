using ECommerce.DbContext;
using ECommerce.Repository.Interface;

namespace ECommerce.Repository.Implementation
{
    public class SaveChangesRepository : ISaveChangesRepository
    {
        private readonly EcommerceContext dbContext;

        public SaveChangesRepository(EcommerceContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task AsynchronousSaveChanges()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
