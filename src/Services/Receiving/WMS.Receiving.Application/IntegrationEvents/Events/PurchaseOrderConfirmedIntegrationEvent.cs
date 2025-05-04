using System;
using WMS.BuildingBlocks.EventBus.Events;

namespace WMS.Receiving.Application.IntegrationEvents.Events
{
    public class PurchaseOrderConfirmedIntegrationEvent : IntegrationEvent
    {
        public Guid PurchaseOrderId { get; private set; }
        public string PurchaseOrderNumber { get; private set; }
        public string VendorId { get; private set; }
        public string VendorName { get; private set; }

        public PurchaseOrderConfirmedIntegrationEvent(
            Guid purchaseOrderId,
            string purchaseOrderNumber,
            string vendorId,
            string vendorName)
        {
            PurchaseOrderId = purchaseOrderId;
            PurchaseOrderNumber = purchaseOrderNumber;
            VendorId = vendorId;
            VendorName = vendorName;
        }
    }
}