using WMS.SharedKernel.Entities;

namespace WMS.Inventory.Domain.Entities
{
    public class Location : Entity
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string Zone { get; private set; }
        public string Aisle { get; private set; }
        public string Bay { get; private set; }
        public string Level { get; private set; }
        public string Position { get; private set; }
        public string Type { get; private set; }
        public int MaxWeight { get; private set; }
        public bool IsActive { get; private set; }
        
        private readonly List<StockOnHand> _stockItems = new();
        public IReadOnlyCollection<StockOnHand> StockItems => _stockItems.AsReadOnly();

        // For EF Core
        private Location() { }

        public Location(string code, string name, string zone, string aisle, string bay, string level, string position, string type, int maxWeight)
        {
            Code = code;
            Name = name;
            Zone = zone;
            Aisle = aisle;
            Bay = bay;
            Level = level;
            Position = position;
            Type = type;
            MaxWeight = maxWeight;
            IsActive = true;
            
            AddDomainEvent(new LocationCreatedEvent(this));
        }

        public void Deactivate()
        {
            if (_stockItems.Any())
                throw new InvalidOperationException("Cannot deactivate location with stock");
                
            IsActive = false;
            AddDomainEvent(new LocationDeactivatedEvent(this));
        }

        public void Activate()
        {
            IsActive = true;
            AddDomainEvent(new LocationActivatedEvent(this));
        }

        public void AddStock(InventoryItem item, int quantity)
        {
            if (!IsActive)
                throw new InvalidOperationException("Cannot add stock to inactive location");
                
            var existingStock = _stockItems.FirstOrDefault(s => s.InventoryItemId == item.Id);
            
            if (existingStock != null)
            {
                existingStock.AddQuantity(quantity);
            }
            else
            {
                _stockItems.Add(new StockOnHand(this.Id, item.Id, quantity));
            }
            
            AddDomainEvent(new StockAddedToLocationEvent(this, item, quantity));
        }

        public void RemoveStock(InventoryItem item, int quantity)
        {
            if (!IsActive)
                throw new InvalidOperationException("Cannot remove stock from inactive location");
                
            var existingStock = _stockItems.FirstOrDefault(s => s.InventoryItemId == item.Id);
            
            if (existingStock == null)
                throw new InvalidOperationException("Item not found in this location");
                
            existingStock.RemoveQuantity(quantity);
            
            if (existingStock.Quantity == 0)
            {
                _stockItems.Remove(existingStock);
            }
            
            AddDomainEvent(new StockRemovedFromLocationEvent(this, item, quantity));
        }
    }
}