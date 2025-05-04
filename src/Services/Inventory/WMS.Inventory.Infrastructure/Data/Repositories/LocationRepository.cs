using WMS.Inventory.Domain.Entities;

namespace WMS.Inventory.Infrastructure.Data.Repositories
{
    public class LocationRepository : EfRepository<Location>, ILocationRepository
    {
        public LocationRepository(InventoryDbContext dbContext) : base(dbContext)
        {
        }
        
        // Additional methods specific to locations
    }
}