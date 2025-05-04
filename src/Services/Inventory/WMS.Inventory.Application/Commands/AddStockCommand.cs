namespace WMS.Inventory.Application.Commands
{
    public class AddStockCommand
    {
        public Guid InventoryItemId { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; }
    }
}