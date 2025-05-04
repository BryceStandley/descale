using WMS.SharedKernel.Entities;

namespace WMS.Shipping.Domain.Entities
{
    public class ShipmentItem : Entity
    {
        public Guid ShipmentId { get; private set; }
        public string Sku { get; private set; }
        public string ItemName { get; private set; }
        public int Quantity { get; private set; }
        
        // Navigation property for EF Core
        public Shipment Shipment { get; private set; }
        
        private readonly List<ShipmentItemPackage> _itemPackages = new();
        public IReadOnlyCollection<ShipmentItemPackage> ItemPackages => _itemPackages.AsReadOnly();

        // For EF Core
        private ShipmentItem() { }

        public ShipmentItem(Guid shipmentId, string sku, string itemName, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));
                
            ShipmentId = shipmentId;
            Sku = sku;
            ItemName = itemName;
            Quantity = quantity;
        }

        public void UpdateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));
                
            Quantity = quantity;
        }

        public void AddToPackage(Guid packageId, string packageNumber, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));
                
            // Check if this item is already in the package
            var existingItemPackage = _itemPackages.FirstOrDefault(ip => ip.PackageId == packageId);
            
            if (existingItemPackage != null)
            {
                // Update the quantity
                existingItemPackage.UpdateQuantity(existingItemPackage.Quantity + quantity);
            }
            else
            {
                // Add a new item package
                _itemPackages.Add(new ShipmentItemPackage(this.Id, packageId, packageNumber, quantity));
            }
        }

        public void RemoveFromPackage(Guid packageId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));
                
            // Find the item package
            var itemPackage = _itemPackages.FirstOrDefault(ip => ip.PackageId == packageId);
            
            if (itemPackage == null)
                throw new InvalidOperationException($"Item is not in package {packageId}");
                
            if (quantity >= itemPackage.Quantity)
            {
                // Remove the entire item package
                _itemPackages.Remove(itemPackage);
            }
            else
            {
                // Update the quantity
                itemPackage.UpdateQuantity(itemPackage.Quantity - quantity);
            }
        }

        public int GetPackagedQuantity()
        {
            return _itemPackages.Sum(ip => ip.Quantity);
        }

        public int GetRemainingQuantity()
        {
            return Quantity - GetPackagedQuantity();
        }
    }
}