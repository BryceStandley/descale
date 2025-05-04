using WMS.SharedKernel.Entities;

namespace WMS.Picking.Domain.Entities
{
    public class PickingLocation : Entity
    {
        public Guid PickingListItemId { get; private set; }
        public string LocationCode { get; private set; }
        public string Zone { get; private set; }
        public int QuantityToPick { get; private set; }
        public int QuantityPicked { get; private set; }
        public string Status { get; private set; } // Pending, Picked, Partial
        public string Notes { get; private set; }
        
        // Navigation property for EF Core
        public PickingListItem PickingListItem { get; private set; }

        // For EF Core
        private PickingLocation() { }

        public PickingLocation(Guid pickingListItemId, string locationCode, string zone, int quantityToPick)
        {
            if (quantityToPick <= 0)
                throw new ArgumentException("Quantity to pick must be positive", nameof(quantityToPick));
                
            PickingListItemId = pickingListItemId;
            LocationCode = locationCode;
            Zone = zone;
            QuantityToPick = quantityToPick;
            QuantityPicked = 0;
            Status = "Pending";
        }

        public void RecordPick(int quantityPicked)
        {
            if (quantityPicked <= 0)
                throw new ArgumentException("Quantity picked must be positive", nameof(quantityPicked));
                
            if (quantityPicked > QuantityToPick - QuantityPicked)
                throw new InvalidOperationException("Cannot pick more than the remaining quantity");
                
            QuantityPicked += quantityPicked;
            
            if (QuantityPicked == QuantityToPick)
                Status = "Picked";
            else if (QuantityPicked > 0)
                Status = "Partial";
        }

        public void AddNotes(string notes)
        {
            Notes = string.IsNullOrEmpty(Notes) ? notes : $"{Notes}\n{notes}";
        }

        public void Reset()
        {
            QuantityPicked = 0;
            Status = "Pending";
        }
    }
}