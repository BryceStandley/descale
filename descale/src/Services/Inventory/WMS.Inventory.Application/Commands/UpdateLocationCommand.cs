namespace WMS.Inventory.Application.Commands
{
    public class UpdateLocationCommand
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int MaxWeight { get; set; }
    }
}