using System;

namespace WMS.Receiving.Application.Commands
{
    public class CreatePurchaseOrderCommand
    {
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string Notes { get; set; }
    }
}