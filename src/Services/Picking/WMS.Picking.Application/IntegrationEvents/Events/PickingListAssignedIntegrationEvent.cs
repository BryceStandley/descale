using WMS.EventBus.Events;

namespace WMS.Picking.Application.IntegrationEvents.Events
{
    public class PickingListAssignedIntegrationEvent : IntegrationEvent
    {
        public Guid PickingListId { get; private set; }
        public string PickingListNumber { get; private set; }
        public string UserId { get; private set; }

        public PickingListAssignedIntegrationEvent(
            Guid pickingListId,
            string pickingListNumber,
            string userId)
        {
            PickingListId = pickingListId;
            PickingListNumber = pickingListNumber;
            UserId = userId;
        }
    }
}