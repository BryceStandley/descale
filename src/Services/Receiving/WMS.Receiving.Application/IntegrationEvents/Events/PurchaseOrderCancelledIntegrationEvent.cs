using System;
using WMS.BuildingBlocks.EventBus.Events;

namespace WMS.Receiving.Application.IntegrationEvents.Events
{
    public class PurchaseOrderCancelledIntegrationEvent : IntegrationEvent
    {
        public Guid PurchaseOrderId { get; private set; }
        public string PurchaseOrderNumber { get; private set; }
        public string Reason { get; private set; }

        public PurchaseOrderCancelledIntegrationEvent(
            Guid purchaseOrderId,
            string purchaseOrderNumber,
            string reason)
        {
            PurchaseOrderId = purchaseOrderId;
            PurchaseOrderNumber = purchaseOrderNumber;
            Reason = reason;
        }
    }
}