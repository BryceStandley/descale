namespace WMS.Inventory.Application.Commands
{
    public class DeallocateStockCommand
    {
        public Guid InventoryItemId { get; set; }
        public int Quantity { get; set; }
        public string Reference { get; set; } 
    }
}