namespace WMS.Inventory.Application.Commands
{
    public class AddStockToLocationCommand
    {
        public Guid LocationId { get; set; }
        public Guid InventoryItemId { get; set; }
        public int Quantity { get; set; }
    }
}