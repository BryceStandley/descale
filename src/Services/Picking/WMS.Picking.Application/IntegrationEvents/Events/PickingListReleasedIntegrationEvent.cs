using WMS.EventBus.Events;

namespace WMS.Picking.Application.IntegrationEvents.Events
{
    public class PickingListReleasedIntegrationEvent : IntegrationEvent
    {
        public Guid PickingListId { get; private set; }
        public string PickingListNumber { get; private set; }
        public string OrderNumber { get; private set; }

        public PickingListReleasedIntegrationEvent(
            Guid pickingListId,
            string pickingListNumber,
            string orderNumber)
        {
            PickingListId = pickingListId;
            PickingListNumber = pickingListNumber;
            OrderNumber = orderNumber;
        }
    }
}