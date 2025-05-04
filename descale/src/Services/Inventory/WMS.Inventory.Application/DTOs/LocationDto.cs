namespace WMS.Inventory.Application.DTOs
{
    public class LocationDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Zone { get; set; }
        public string Aisle { get; set; }
        public string Bay { get; set; }
        public string Level { get; set; }
        public string Position { get; set; }
        public string Type { get; set; }
        public int MaxWeight { get; set; }
        public bool IsActive { get; set; }
    }
}