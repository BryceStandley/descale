using System;
using WMS.BuildingBlocks.EventBus.Events;

namespace WMS.Receiving.Application.IntegrationEvents.Events
{
    public class ReceiptCancelledIntegrationEvent : IntegrationEvent
    {
        public Guid ReceiptId { get; private set; }
        public string ReceiptNumber { get; private set; }
        public Guid PurchaseOrderId { get; private set; }
        public string Reason { get; private set; }

        public ReceiptCancelledIntegrationEvent(
            Guid receiptId,
            string receiptNumber,
            Guid purchaseOrderId,
            string reason)
        {
            ReceiptId = receiptId;
            ReceiptNumber = receiptNumber;
            PurchaseOrderId = purchaseOrderId;
            Reason = reason;
        }
    }
}