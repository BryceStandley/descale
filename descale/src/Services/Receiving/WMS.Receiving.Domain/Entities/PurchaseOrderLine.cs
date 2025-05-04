using WMS.SharedKernel.Entities;

namespace WMS.Receiving.Domain.Entities
{
    public class PurchaseOrderLine : Entity
    {
        public Guid PurchaseOrderId { get; private set; }
        public string Sku { get; private set; }
        public string ItemName { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal LineTotal => Quantity * UnitPrice;
        
        // Navigation property for EF Core
        public PurchaseOrder PurchaseOrder { get; private set; }

        // For EF Core
        private PurchaseOrderLine() { }

        public PurchaseOrderLine(Guid purchaseOrderId, string sku, string itemName, int quantity, decimal unitPrice)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));
                
            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));
                
            PurchaseOrderId = purchaseOrderId;
            Sku = sku;
            ItemName = itemName;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public void Update(int quantity, decimal unitPrice)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));
                
            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));
                
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}