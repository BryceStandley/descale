using WMS.EventBus.Events;

namespace WMS.Picking.Application.IntegrationEvents.Events
{
    public class PickingListCancelledIntegrationEvent : IntegrationEvent
    {
        public Guid PickingListId { get; private set; }
        public string PickingListNumber { get; private set; }
        public string Reason { get; private set; }

        public PickingListCancelledIntegrationEvent(
            Guid pickingListId,
            string pickingListNumber,
            string reason)
        {
            PickingListId = pickingListId;
            PickingListNumber = pickingListNumber;
            Reason = reason;
        }
    }
}