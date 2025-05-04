namespace WMS.Picking.Application.Commands
{
    public class CreatePickingListCommand
    {
        public string OrderNumber { get; set; }
        public string Priority { get; set; } = "Normal";
        public string Notes { get; set; }
    }
}