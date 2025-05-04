namespace WMS.Inventory.Application.DTOs
{
    public class InventoryItemDto
    {
        public Guid Id { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string UnitOfMeasure { get; set; }
        public int QuantityOnHand { get; set; }
        public int QuantityAllocated { get; set; }
        public int QuantityAvailable { get; set; }
        public decimal Cost { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}