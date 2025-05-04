using System;

namespace WMS.Receiving.Application.Commands
{
    public class AddPurchaseOrderLineCommand
    {
        public Guid PurchaseOrderId { get; set; }
        public string Sku { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}