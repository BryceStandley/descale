using WMS.SharedKernel.Entities;

namespace WMS.Receiving.Domain.Entities
{
    public class PurchaseOrder : Entity
    {
        public string Number { get; private set; }
        public string VendorId { get; private set; }
        public string VendorName { get; private set; }
        public DateTime OrderDate { get; private set; }
        public DateTime? ExpectedDeliveryDate { get; private set; }
        public string Status { get; private set; } // Draft, Confirmed, Partially Received, Closed, Cancelled
        public string Notes { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ModifiedAt { get; private set; }
        
        private readonly List<PurchaseOrderLine> _orderLines = new();
        public IReadOnlyCollection<PurchaseOrderLine> OrderLines => _orderLines.AsReadOnly();
        
        private readonly List<Receipt> _receipts = new();
        public IReadOnlyCollection<Receipt> Receipts => _receipts.AsReadOnly();

        // For EF Core
        private PurchaseOrder() { }

        public PurchaseOrder(string number, string vendorId, string vendorName, DateTime orderDate, DateTime? expectedDeliveryDate, string notes)
        {
            Number = number;
            VendorId = vendorId;
            VendorName = vendorName;
            OrderDate = orderDate;
            ExpectedDeliveryDate = expectedDeliveryDate;
            Status = "Draft";
            Notes = notes;
            CreatedAt = DateTime.UtcNow;
            
            AddDomainEvent(new PurchaseOrderCreatedEvent(this));
        }

        public void AddOrderLine(string sku, string itemName, int quantity, decimal unitPrice)
        {
            if (Status != "Draft")
                throw new InvalidOperationException("Cannot modify a purchase order that is not in Draft status");
                
            if (_orderLines.Any(ol => ol.Sku == sku))
                throw new InvalidOperationException($"SKU {sku} already exists in this purchase order");
                
            _orderLines.Add(new PurchaseOrderLine(this.Id, sku, itemName, quantity, unitPrice));
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new PurchaseOrderLineAddedEvent(this, sku, quantity));
        }

        public void UpdateOrderLine(string sku, int quantity, decimal unitPrice)
        {
            if (Status != "Draft")
                throw new InvalidOperationException("Cannot modify a purchase order that is not in Draft status");
                
            var orderLine = _orderLines.FirstOrDefault(ol => ol.Sku == sku);
            if (orderLine == null)
                throw new InvalidOperationException($"SKU {sku} does not exist in this purchase order");
                
            orderLine.Update(quantity, unitPrice);
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new PurchaseOrderLineUpdatedEvent(this, sku, quantity));
        }

        public void RemoveOrderLine(string sku)
        {
            if (Status != "Draft")
                throw new InvalidOperationException("Cannot modify a purchase order that is not in Draft status");
                
            var orderLine = _orderLines.FirstOrDefault(ol => ol.Sku == sku);
            if (orderLine == null)
                throw new InvalidOperationException($"SKU {sku} does not exist in this purchase order");
                
            _orderLines.Remove(orderLine);
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new PurchaseOrderLineRemovedEvent(this, sku));
        }

        public void Confirm()
        {
            if (Status != "Draft")
                throw new InvalidOperationException("Only Draft purchase orders can be confirmed");
                
            if (!_orderLines.Any())
                throw new InvalidOperationException("Cannot confirm a purchase order with no order lines");
                
            Status = "Confirmed";
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new PurchaseOrderConfirmedEvent(this));
        }

        public void Cancel(string reason)
        {
            if (Status == "Closed" || Status == "Cancelled")
                throw new InvalidOperationException("Cannot cancel a purchase order that is already closed or cancelled");
                
            Status = "Cancelled";
            Notes = string.IsNullOrEmpty(Notes) ? reason : $"{Notes}\nCancelled: {reason}";
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new PurchaseOrderCancelledEvent(this, reason));
        }

        public Receipt AddReceipt(string receiptNumber, DateTime receivedDate, string receivedBy, string notes)
        {
            if (Status == "Closed" || Status == "Cancelled")
                throw new InvalidOperationException("Cannot add receipts to a purchase order that is closed or cancelled");
                
            var receipt = new Receipt(this.Id, receiptNumber, receivedDate, receivedBy, notes);
            _receipts.Add(receipt);
            
            UpdateStatusBasedOnReceipts();
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new ReceiptCreatedEvent(this, receipt));
            
            return receipt;
        }

        private void UpdateStatusBasedOnReceipts()
        {
            if (!_receipts.Any())
                return;
                
            // Check if all items have been fully received
            bool allLinesFullyReceived = true;
            
            foreach (var orderLine in _orderLines)
            {
                int totalReceived = _receipts
                    .SelectMany(r => r.ReceiptLines)
                    .Where(rl => rl.Sku == orderLine.Sku)
                    .Sum(rl => rl.QuantityReceived);
                    
                if (totalReceived < orderLine.Quantity)
                {
                    allLinesFullyReceived = false;
                    break;
                }
            }
            
            Status = allLinesFullyReceived ? "Closed" : "Partially Received";
        }

        public decimal TotalValue => _orderLines.Sum(ol => ol.LineTotal);
    }
}