using WMS.EventBus.Events;

namespace WMS.Picking.Application.IntegrationEvents.Events
{
    public class PickingItemShortIntegrationEvent : IntegrationEvent
    {
        public Guid PickingListId { get; private set; }
        public string Sku { get; private set; }
        public int QuantityRequired { get; private set; }
        public int QuantityPicked { get; private set; }
        public string Reason { get; private set; }

        public PickingItemShortIntegrationEvent(
            Guid pickingListId,
            string sku,
            int quantityRequired,
            int quantityPicked,
            string reason)
        {
            PickingListId = pickingListId;
            Sku = sku;
            QuantityRequired = quantityRequired;
            QuantityPicked = quantityPicked;
            Reason = reason;
        }
    }
}