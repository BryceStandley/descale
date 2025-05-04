using System;

namespace WMS.Receiving.Application.DTOs
{
    public class PurchaseOrderLineDto
    {
        public Guid Id { get; set; }
        public string Sku { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}