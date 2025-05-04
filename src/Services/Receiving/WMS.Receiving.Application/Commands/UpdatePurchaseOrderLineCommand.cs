using System;

namespace WMS.Receiving.Application.Commands
{
    public class UpdatePurchaseOrderLineCommand
    {
        public Guid PurchaseOrderId { get; set; }
        public string Sku { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}