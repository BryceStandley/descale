using System;
using System.Collections.Generic;

namespace WMS.Receiving.Application.DTOs
{
    public class PurchaseOrderDto
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public decimal TotalValue { get; set; }
        public List<PurchaseOrderLineDto> OrderLines { get; set; } = new List<PurchaseOrderLineDto>();
    }
}