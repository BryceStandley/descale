using WMS.SharedKernel.Entities;

namespace WMS.Shipping.Domain.Entities
{
    public class ShipmentPackage : Entity
    {
        public Guid ShipmentId { get; private set; }
        public string PackageNumber { get; private set; }
        public string PackageType { get; private set; } // Box, Pallet, Envelope, etc.
        public decimal Weight { get; private set; }
        public decimal Length { get; private set; }
        public decimal Width { get; private set; }
        public decimal Height { get; private set; }
        public string TrackingNumber { get; private set; }
        public string Status { get; private set; } // Created, Packed, Sealed, Labeled, Shipped
        
        // Navigation property for EF Core
        public Shipment Shipment { get; private set; }
        
        private readonly List<ShipmentItemPackage> _itemPackages = new();
        public IReadOnlyCollection<ShipmentItemPackage> ItemPackages => _itemPackages.AsReadOnly();

        // For EF Core
        private ShipmentPackage() { }

        public ShipmentPackage(Guid shipmentId, string packageNumber, string packageType, decimal weight, decimal length, decimal width, decimal height)
        {
            if (weight <= 0)
                throw new ArgumentException("Weight must be positive", nameof(weight));
                
            if (length <= 0)
                throw new ArgumentException("Length must be positive", nameof(length));
                
            if (width <= 0)
                throw new ArgumentException("Width must be positive", nameof(width));
                
            if (height <= 0)
                throw new ArgumentException("Height must be positive", nameof(height));
                
            ShipmentId = shipmentId;
            PackageNumber = packageNumber;
            PackageType = packageType;
            Weight = weight;
            Length = length;
            Width = width;
            Height = height;
            Status = "Created";
        }

        public void UpdateDimensions(decimal weight, decimal length, decimal width, decimal height)
        {
            if (Status != "Created")
                throw new InvalidOperationException("Cannot update dimensions when package is not in Created status");
                
            if (weight <= 0)
                throw new ArgumentException("Weight must be positive", nameof(weight));
                
            if (length <= 0)
                throw new ArgumentException("Length must be positive", nameof(length));
                
            if (width <= 0)
                throw new ArgumentException("Width must be positive", nameof(width));
                
            if (height <= 0)
                throw new ArgumentException("Height must be positive", nameof(height));
                
            Weight = weight;
            Length = length;
            Width = width;
            Height = height;
        }

        public void AddItem(ShipmentItem item, int quantity)
        {
            if (Status != "Created" && Status != "Packed")
                throw new InvalidOperationException("Cannot add items when package is not in Created or Packed status");
                
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));
                
            item.AddToPackage(this.Id, this.PackageNumber, quantity);
            
            if (Status == "Created")
                Status = "Packed";
        }

        public void RemoveItem(ShipmentItem item, int quantity)
        {
            if (Status != "Created" && Status != "Packed")
                throw new InvalidOperationException("Cannot remove items when package is not in Created or Packed status");
                
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));
                
            item.RemoveFromPackage(this.Id, quantity);
            
            // If no items remain in this package, set status back to Created
            if (!_itemPackages.Any())
                Status = "Created";
        }

        public void Seal()
        {
            if (Status != "Packed")
                throw new InvalidOperationException("Cannot seal a package that is not in Packed status");
                
            Status = "Sealed";
        }

        public void Label(string trackingNumber)
        {
            if (Status != "Sealed")
                throw new InvalidOperationException("Cannot label a package that is not in Sealed status");
                
            TrackingNumber = trackingNumber;
            Status = "Labeled";
        }

        public void MarkAsShipped()
        {
            if (Status != "Labeled")
                throw new InvalidOperationException("Cannot mark a package as shipped that is not in Labeled status");
                
            Status = "Shipped";
        }
    }
}