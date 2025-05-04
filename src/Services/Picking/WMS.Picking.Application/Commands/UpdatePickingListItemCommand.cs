namespace WMS.Picking.Application.Commands
{
    public class UpdatePickingListItemCommand
    {
        public Guid PickingListId { get; set; }
        public string Sku { get; set; }
        public int QuantityRequired { get; set; }
    }
}