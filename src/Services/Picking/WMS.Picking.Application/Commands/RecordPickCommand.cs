namespace WMS.Picking.Application.Commands
{
    public class RecordPickCommand
    {
        public Guid PickingListId { get; set; }
        public string Sku { get; set; }
        public string LocationCode { get; set; }
        public int QuantityPicked { get; set; }
    }
}