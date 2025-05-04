namespace WMS.Inventory.Application.Commands
{
    public class RemoveStockFromLocationCommand
    {
        public Guid LocationId { get; set; }
        public Guid InventoryItemId { get; set; }
        public int Quantity { get; set; }
    }
}