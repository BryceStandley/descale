using WMS.SharedKernel.Entities;

namespace WMS.Receiving.Domain.Entities
{
    public class ReceiptLine : Entity
    {
        public Guid ReceiptId { get; private set; }
        public string Sku { get; private set; }
        public string ItemName { get; private set; }
        public int QuantityExpected { get; private set; }
        public int QuantityReceived { get; private set; }
        public string LocationCode { get; private set; } // Where the item was put away
        public string Status { get; private set; } // Received, Partial, Over, Damaged
        public string Notes { get; private set; }
        
        // Navigation property for EF Core
        public Receipt Receipt { get; private set; }

        // For EF Core
        private ReceiptLine() { }

        public ReceiptLine(Guid receiptId, string sku, string itemName, int quantityExpected, int quantityReceived, string locationCode)
        {
            if (quantityExpected <= 0)
                throw new ArgumentException("Expected quantity must be positive", nameof(quantityExpected));
                
            if (quantityReceived < 0)
                throw new ArgumentException("Received quantity cannot be negative", nameof(quantityReceived));
                
            ReceiptId = receiptId;
            Sku = sku;
            ItemName = itemName;
            QuantityExpected = quantityExpected;
            QuantityReceived = quantityReceived;
            LocationCode = locationCode;
            
            Status = DetermineStatus(quantityExpected, quantityReceived);
        }

        private string DetermineStatus(int expected, int received)
        {
            if (received == 0)
                return "Pending";
            else if (received < expected)
                return "Partial";
            else if (received == expected)
                return "Complete";
            else
                return "Excess";
        }

        public void UpdateQuantity(int quantityReceived, string notes = null)
        {
            if (quantityReceived < 0)
                throw new ArgumentException("Received quantity cannot be negative", nameof(quantityReceived));
                
            QuantityReceived = quantityReceived;
            Status = DetermineStatus(QuantityExpected, quantityReceived);
            
            if (!string.IsNullOrEmpty(notes))
            {
                Notes = string.IsNullOrEmpty(Notes) ? notes : $"{Notes}\n{notes}";
            }
        }

        public void MarkAsDamaged(string notes)
        {
            Status = "Damaged";
            Notes = string.IsNullOrEmpty(Notes) ? notes : $"{Notes}\nDamaged: {notes}";
        }

        public void UpdateLocation(string locationCode)
        {
            LocationCode = locationCode;
        }
    }
}