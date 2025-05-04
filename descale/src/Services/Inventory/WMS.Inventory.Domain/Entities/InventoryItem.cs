using WMS.SharedKernel.Entities;

namespace WMS.Inventory.Domain.Entities
{
    public class InventoryItem : Entity
    {
        public string Sku { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Category { get; private set; }
        public string UnitOfMeasure { get; private set; }
        public int QuantityOnHand { get; private set; }
        public int QuantityAllocated { get; private set; }
        public int QuantityAvailable => QuantityOnHand - QuantityAllocated;
        public decimal Cost { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ModifiedAt { get; private set; }

        // For EF Core
        private InventoryItem() { }

        public InventoryItem(string sku, string name, string description, string category, string unitOfMeasure, int initialQuantity, decimal cost)
        {
            Sku = sku;
            Name = name;
            Description = description;
            Category = category;
            UnitOfMeasure = unitOfMeasure;
            QuantityOnHand = initialQuantity;
            QuantityAllocated = 0;
            Cost = cost;
            CreatedAt = DateTime.UtcNow;
            
            AddDomainEvent(new InventoryItemCreatedEvent(this));
        }

        public void AddStock(int quantity, string reason)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            QuantityOnHand += quantity;
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new InventoryStockAddedEvent(this, quantity, reason));
        }

        public void RemoveStock(int quantity, string reason)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));
                
            if (quantity > QuantityAvailable)
                throw new InvalidOperationException("Cannot remove more than available quantity");

            QuantityOnHand -= quantity;
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new InventoryStockRemovedEvent(this, quantity, reason));
        }

        public void AllocateStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));
                
            if (quantity > QuantityAvailable)
                throw new InvalidOperationException("Cannot allocate more than available quantity");

            QuantityAllocated += quantity;
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new InventoryStockAllocatedEvent(this, quantity));
        }

        public void DeallocateStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));
                
            if (quantity > QuantityAllocated)
                throw new InvalidOperationException("Cannot deallocate more than allocated quantity");

            QuantityAllocated -= quantity;
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new InventoryStockDeallocatedEvent(this, quantity));
        }
    }
}