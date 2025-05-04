namespace WMS.Inventory.Application.DTOs
{
    public class StockOnHandDto
    {
        public Guid Id { get; set; }
        public Guid LocationId { get; set; }
        public string LocationCode { get; set; }
        public Guid InventoryItemId { get; set; }
        public string ItemSku { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}