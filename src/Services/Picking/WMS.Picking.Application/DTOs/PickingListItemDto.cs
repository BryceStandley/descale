namespace WMS.Picking.Application.DTOs
{
    public class PickingListItemDto
    {
        public Guid Id { get; set; }
        public string Sku { get; set; }
        public string ItemName { get; set; }
        public int QuantityRequired { get; set; }
        public int QuantityAllocated { get; set; }
        public int QuantityPicked { get; set; }
        public string Status { get; set; }
        public List<PickingLocationDto> PickingLocations { get; set; } = new List<PickingLocationDto>();
    }
}