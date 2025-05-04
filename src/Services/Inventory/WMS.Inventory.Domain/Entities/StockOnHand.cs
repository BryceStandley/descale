using WMS.SharedKernel.Entities;

namespace WMS.Inventory.Domain.Entities
{
    public class StockOnHand : Entity
    {
        public Guid LocationId { get; private set; }
        public Guid InventoryItemId { get; private set; }
        public int Quantity { get; private set; }
        public DateTime LastUpdated { get; private set; }
        
        // Navigation properties for EF Core
        public InventoryItem InventoryItem { get; private set; }
        public Location Location { get; private set; }

        // For EF Core
        private StockOnHand() { }

        public StockOnHand(Guid locationId, Guid inventoryItemId, int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException("Quantity cannot be negative", nameof(quantity));
                
            LocationId = locationId;
            InventoryItemId = inventoryItemId;
            Quantity = quantity;
            LastUpdated = DateTime.UtcNow;
        }

        public void AddQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));
                
            Quantity += quantity;
            LastUpdated = DateTime.UtcNow;
        }

        public void RemoveQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));
                
            if (quantity > Quantity)
                throw new InvalidOperationException("Cannot remove more than available quantity");
                
            Quantity -= quantity;
            LastUpdated = DateTime.UtcNow;
        }
    }
}