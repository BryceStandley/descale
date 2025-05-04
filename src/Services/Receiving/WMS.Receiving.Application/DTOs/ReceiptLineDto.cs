using System;

namespace WMS.Receiving.Application.DTOs
{
    public class ReceiptLineDto
    {
        public Guid Id { get; set; }
        public string Sku { get; set; }
        public string ItemName { get; set; }
        public int QuantityExpected { get; set; }
        public int QuantityReceived { get; set; }
        public string LocationCode { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}