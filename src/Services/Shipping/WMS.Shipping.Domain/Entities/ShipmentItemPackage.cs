using descale.BuildingBlocks.WMS.SharedKernel.Entities;

namespace descale.Services.Shipping.WMS.Shipping.Domain.Entities
{
    public class ShipmentItemPackage : Entity
    {
        public Guid ShipmentItemId { get; private set; }
        public Guid PackageId { get; private set; }
        public string PackageNumber { get; private set; }
        public int Quantity { get; private set; }
        
        // Navigation properties for EF Core
        public ShipmentItem ShipmentItem { get; private set; }
        public ShipmentPackage Package { get; private set; }

        // For EF Core
        private ShipmentItemPackage() { }

        public ShipmentItemPackage(Guid shipmentItemId, Guid packageId, string packageNumber, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));
                
            ShipmentItemId = shipmentItemId;
            PackageId = packageId;
            PackageNumber = packageNumber;
            Quantity = quantity;
        }

        public void UpdateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));
                
            Quantity = quantity;
        }
    }
}