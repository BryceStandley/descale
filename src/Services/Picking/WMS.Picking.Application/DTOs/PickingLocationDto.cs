namespace WMS.Picking.Application.DTOs
{
    public class PickingLocationDto
    {
        public Guid Id { get; set; }
        public string LocationCode { get; set; }
        public string Zone { get; set; }
        public int QuantityToPick { get; set; }
        public int QuantityPicked { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}