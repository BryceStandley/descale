using WMS.SharedKernel.Entities;

namespace WMS.Receiving.Domain.Entities
{
    public class Receipt : Entity
    {
        public Guid PurchaseOrderId { get; private set; }
        public string ReceiptNumber { get; private set; }
        public DateTime ReceivedDate { get; private set; }
        public string ReceivedBy { get; private set; }
        public string Status { get; private set; } // Draft, Completed, Cancelled
        public string Notes { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ModifiedAt { get; private set; }
        
        private readonly List<ReceiptLine> _receiptLines = new();
        public IReadOnlyCollection<ReceiptLine> ReceiptLines => _receiptLines.AsReadOnly();
        
        // Navigation property for EF Core
        public PurchaseOrder PurchaseOrder { get; private set; }

        // For EF Core
        private Receipt() { }

        public Receipt(Guid purchaseOrderId, string receiptNumber, DateTime receivedDate, string receivedBy, string notes)
        {
            PurchaseOrderId = purchaseOrderId;
            ReceiptNumber = receiptNumber;
            ReceivedDate = receivedDate;
            ReceivedBy = receivedBy;
            Status = "Draft";
            Notes = notes;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddReceiptLine(string sku, string itemName, int quantityExpected, int quantityReceived, string locationCode)
        {
            if (Status != "Draft")
                throw new InvalidOperationException("Cannot modify a receipt that is not in Draft status");
                
            if (_receiptLines.Any(rl => rl.Sku == sku))
                throw new InvalidOperationException($"SKU {sku} already exists in this receipt");
                
            _receiptLines.Add(new ReceiptLine(this.Id, sku, itemName, quantityExpected, quantityReceived, locationCode));
            ModifiedAt = DateTime.UtcNow;
        }

        public void Complete()
        {
            if (Status != "Draft")
                throw new InvalidOperationException("Only Draft receipts can be completed");
                
            if (!_receiptLines.Any())
                throw new InvalidOperationException("Cannot complete a receipt with no receipt lines");
                
            Status = "Completed";
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new ReceiptCompletedEvent(this));
        }

        public void Cancel(string reason)
        {
            if (Status == "Cancelled")
                throw new InvalidOperationException("Receipt is already cancelled");
                
            if (Status == "Completed")
                throw new InvalidOperationException("Cannot cancel a completed receipt");
                
            Status = "Cancelled";
            Notes = string.IsNullOrEmpty(Notes) ? reason : $"{Notes}\nCancelled: {reason}";
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new ReceiptCancelledEvent(this, reason));
        }
    }
}