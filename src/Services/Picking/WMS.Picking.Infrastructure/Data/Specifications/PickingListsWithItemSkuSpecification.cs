using WMS.Picking.Domain.Entities;

namespace WMS.Picking.Infrastructure.Data.Specifications
{
    public class PickingListsWithItemSkuSpecification : BaseSpecification<PickingList>
    {
        public PickingListsWithItemSkuSpecification(string sku, string pickingListStatus = null)
            : base(p => p.Items.Any(i => i.Sku == sku) && 
                        (string.IsNullOrEmpty(pickingListStatus) || p.Status == pickingListStatus))
        {
            AddInclude(p => p.Items);
        }
    }
}