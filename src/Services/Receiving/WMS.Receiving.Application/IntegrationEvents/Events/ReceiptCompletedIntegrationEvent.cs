using System;
using System.Collections.Generic;
using WMS.BuildingBlocks.EventBus.Events;

namespace WMS.Receiving.Application.IntegrationEvents.Events
{
    public class ReceiptCompletedIntegrationEvent : IntegrationEvent
    {
        public Guid ReceiptId { get; private set; }
        public string ReceiptNumber { get; private set; }
        public Guid PurchaseOrderId { get; private set; }
        public List<ReceiptLineInfo> ReceiptLines { get; private set; }

        public ReceiptCompletedIntegrationEvent(
            Guid receiptId,
            string receiptNumber,
            Guid purchaseOrderId,
            List<ReceiptLineInfo> receiptLines)
        {
            ReceiptId = receiptId;
            ReceiptNumber = receiptNumber;
            PurchaseOrderId = purchaseOrderId;
            ReceiptLines = receiptLines;
        }

        public class ReceiptLineInfo
        {
            public string Sku { get; private set; }
            public int QuantityReceived { get; private set; }
            public string LocationCode { get; private set; }

            public ReceiptLineInfo(
                string sku,
                int quantityReceived,
                string locationCode)
            {
                Sku = sku;
                QuantityReceived = quantityReceived;
                LocationCode = locationCode;
            }
        }
    }
}