namespace WMS.Picking.Application.DTOs
{
    public class PickingListDto
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public string OrderNumber { get; set; }
        public string Status { get; set; }
        public string AssignedTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Notes { get; set; }
        public string Priority { get; set; }
        public List<PickingListItemDto> Items { get; set; } = new List<PickingListItemDto>();
    }
}