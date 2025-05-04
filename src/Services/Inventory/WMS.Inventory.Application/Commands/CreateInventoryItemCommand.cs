namespace WMS.Inventory.Application.Commands
{
    public class CreateInventoryItemCommand
    {
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string UnitOfMeasure { get; set; }
        public int InitialQuantity { get; set; }
        public decimal Cost { get; set; }
    }
}