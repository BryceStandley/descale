using WMS.EventBus.Events;

namespace WMS.Picking.Application.IntegrationEvents.Events
{
    public class InventoryPickedIntegrationEvent : IntegrationEvent
    {
        public Guid PickingListId { get; private set; }
        public string Sku { get; private set; }
        public string LocationCode { get; private set; }
        public int QuantityPicked { get; private set; }
        public bool IsItemFullyPicked { get; private set; }

        public InventoryPickedIntegrationEvent(
            Guid pickingListId,
            string sku,
            string locationCode,
            int quantityPicked,
            bool isItemFullyPicked)
        {
            PickingListId = pickingListId;
            Sku = sku;
            LocationCode = locationCode;
            QuantityPicked = quantityPicked;
            IsItemFullyPicked = isItemFullyPicked;
        }
    }
}