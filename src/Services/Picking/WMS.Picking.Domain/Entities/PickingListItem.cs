using WMS.SharedKernel.Entities;

namespace WMS.Picking.Domain.Entities
{
    public class PickingListItem : Entity
    {
        public Guid PickingListId { get; private set; }
        public string Sku { get; private set; }
        public string ItemName { get; private set; }
        public int QuantityRequired { get; private set; }
        public int QuantityAllocated { get; private set; }
        public int QuantityPicked { get; private set; }
        public string Status { get; private set; } // Pending, Allocated, Picking, Picked, Short
        
        private readonly List<PickingLocation> _pickingLocations = new();
        public IReadOnlyCollection<PickingLocation> PickingLocations => _pickingLocations.AsReadOnly();
        
        // Navigation property for EF Core
        public PickingList PickingList { get; private set; }

        // For EF Core
        private PickingListItem() { }

        public PickingListItem(Guid pickingListId, string sku, string itemName, int quantityRequired, int quantityAllocated)
        {
            if (quantityRequired <= 0)
                throw new ArgumentException("Quantity required must be positive", nameof(quantityRequired));
                
            PickingListId = pickingListId;
            Sku = sku;
            ItemName = itemName;
            QuantityRequired = quantityRequired;
            QuantityAllocated = quantityAllocated;
            QuantityPicked = 0;
            Status = quantityAllocated >= quantityRequired ? "Allocated" : "Pending";
        }

        public void UpdateQuantityRequired(int quantityRequired)
        {
            if (quantityRequired <= 0)
                throw new ArgumentException("Quantity required must be positive", nameof(quantityRequired));
                
            QuantityRequired = quantityRequired;
            
            // Update status based on new quantity required
            if (QuantityAllocated >= quantityRequired)
                Status = "Allocated";
            else
                Status = "Pending";
        }

        public void AllocateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));
                
            QuantityAllocated += quantity;
            
            if (QuantityAllocated >= QuantityRequired)
                Status = "Allocated";
        }

        public void AddPickingLocation(string locationCode, string zone, int quantityToPickFromLocation)
        {
            if (quantityToPickFromLocation <= 0)
                throw new ArgumentException("Quantity to pick must be positive", nameof(quantityToPickFromLocation));
                
            var location = new PickingLocation(this.Id, locationCode, zone, quantityToPickFromLocation);
            _pickingLocations.Add(location);
        }

        public void StartPicking()
        {
            if (Status != "Allocated")
                throw new InvalidOperationException("Cannot start picking an item that is not allocated");
                
            Status = "Picking";
        }

        public void RecordPick(string locationCode, int quantityPicked)
        {
            if (Status != "Picking")
                throw new InvalidOperationException("Cannot record picks for an item that is not in Picking status");
                
            var location = _pickingLocations.FirstOrDefault(pl => pl.LocationCode == locationCode);
            if (location == null)
                throw new InvalidOperationException($"Location {locationCode} is not assigned to this item");
                
            location.RecordPick(quantityPicked);
            QuantityPicked += quantityPicked;
            
            // Check if all required quantity has been picked
            if (QuantityPicked >= QuantityRequired)
            {
                Status = "Picked";
            }
        }

        public void MarkAsShort(string reason)
        {
            if (Status != "Picking")
                throw new InvalidOperationException("Can only mark an item as short when it is in Picking status");
                
            Status = "Short";
            
            // Add reason to the picking locations
            foreach (var location in _pickingLocations)
            {
                if (location.QuantityPicked < location.QuantityToPick)
                {
                    location.AddNotes($"Short: {reason}");
                }
            }
        }

        public void CancelPicking()
        {
            // Reset the picking process
            QuantityPicked = 0;
            Status = "Allocated";
            
            // Reset all picking locations
            foreach (var location in _pickingLocations)
            {
                location.Reset();
            }
        }
    }
}