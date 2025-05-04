namespace WMS.Inventory.Application.Commands
{
    public class AllocateStockCommand
    {
        public Guid InventoryItemId { get; set; }
        public int Quantity { get; set; }
        public string Reference { get; set; } // Order number, picking list, etc.
    }
}