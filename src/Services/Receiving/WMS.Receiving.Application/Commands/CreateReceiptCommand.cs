using System;
using System.Collections.Generic;

namespace WMS.Receiving.Application.Commands
{
    public class CreateReceiptCommand
    {
        public Guid PurchaseOrderId { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string ReceivedBy { get; set; }
        public string Notes { get; set; }
        public List<ReceiptLineInfo> Lines { get; set; } = new List<ReceiptLineInfo>();

        public class ReceiptLineInfo
        {
            public string Sku { get; set; }
            public int QuantityReceived { get; set; }
            public string LocationCode { get; set; }
        }
    }
}