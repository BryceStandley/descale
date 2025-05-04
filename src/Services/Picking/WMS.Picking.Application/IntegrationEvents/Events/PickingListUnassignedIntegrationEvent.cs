using WMS.EventBus.Events;

namespace WMS.Picking.Application.IntegrationEvents.Events
{
    public class PickingListUnassignedIntegrationEvent : IntegrationEvent
    {
        public Guid PickingListId { get; private set; }
        public string PickingListNumber { get; private set; }
        public string PreviousUserId { get; private set; }

        public PickingListUnassignedIntegrationEvent(
            Guid pickingListId,
            string pickingListNumber,
            string previousUserId)
        {
            PickingListId = pickingListId;
            PickingListNumber = pickingListNumber;
            PreviousUserId = previousUserId;
        }
    }
}