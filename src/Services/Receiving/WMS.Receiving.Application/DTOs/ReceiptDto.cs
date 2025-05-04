using System;
using System.Collections.Generic;

namespace WMS.Receiving.Application.DTOs
{
    public class ReceiptDto
    {
        public Guid Id { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public string ReceiptNumber { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string ReceivedBy { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public List<ReceiptLineDto> ReceiptLines { get; set; } = new List<ReceiptLineDto>();
    }
}