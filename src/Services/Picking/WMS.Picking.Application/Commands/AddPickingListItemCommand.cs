namespace WMS.Picking.Application.Commands
{
    public class AddPickingListItemCommand
    {
        public Guid PickingListId { get; set; }
        public string Sku { get; set; }
        public string ItemName { get; set; }
        public int QuantityRequired { get; set; }
        public int QuantityAllocated { get; set; }
    }
}