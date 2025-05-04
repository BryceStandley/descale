using WMS.Picking.Domain.Entities;

namespace WMS.Picking.Infrastructure.Data.Specifications
{
    public class PickingListWithItemsSpecification : BaseSpecification<PickingList>
    {
        public PickingListWithItemsSpecification(Guid pickingListId)
            : base(p => p.Id == pickingListId)
        {
            AddInclude(p => p.Items);
            AddInclude("Items.PickingLocations");
        }
    }
}