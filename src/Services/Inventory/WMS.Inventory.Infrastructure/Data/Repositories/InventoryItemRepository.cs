using WMS.Inventory.Domain.Entities;

namespace WMS.Inventory.Infrastructure.Data.Repositories
{
    public class InventoryItemRepository : EfRepository<InventoryItem>, IInventoryItemRepository
    {
        public InventoryItemRepository(InventoryDbContext dbContext) : base(dbContext)
        {
        }
        
        // Additional methods specific to inventory items
    }
}