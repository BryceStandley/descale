using WMS.EventBus.Events;

namespace WMS.Picking.Application.IntegrationEvents.Events
{
    public class PickingListCompletedIntegrationEvent : IntegrationEvent
    {
        public Guid PickingListId { get; private set; }
        public string PickingListNumber { get; private set; }
        public string OrderNumber { get; private set; }

        public PickingListCompletedIntegrationEvent(
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