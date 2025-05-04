namespace WMS.Picking.Domain.Entities
{
    public class PickingList : Entity
    {
        public string Number { get; private set; }
        public string OrderNumber { get; private set; } // Reference to external order system
        public string Status { get; private set; } // Draft, Released, In Progress, Completed, Cancelled
        public string AssignedTo { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ModifiedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public string Notes { get; private set; }
        public string Priority { get; private set; } // Low, Normal, High, Urgent
        
        private readonly List<PickingListItem> _items = new();
        public IReadOnlyCollection<PickingListItem> Items => _items.AsReadOnly();

        // For EF Core
        private PickingList() { }

        public PickingList(string number, string orderNumber, string priority = "Normal", string notes = null)
        {
            Number = number;
            OrderNumber = orderNumber;
            Status = "Draft";
            Priority = priority;
            Notes = notes;
            CreatedAt = DateTime.UtcNow;
            
            AddDomainEvent(new PickingListCreatedEvent(this));
        }

        public void AddItem(string sku, string itemName, int quantityRequired, int quantityAllocated)
        {
            if (Status != "Draft")
                throw new InvalidOperationException("Cannot modify a picking list that is not in Draft status");
                
            if (_items.Any(i => i.Sku == sku))
                throw new InvalidOperationException($"SKU {sku} already exists in this picking list");
                
            _items.Add(new PickingListItem(this.Id, sku, itemName, quantityRequired, quantityAllocated));
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new PickingListItemAddedEvent(this, sku, quantityRequired));
        }

        public void UpdateItem(string sku, int quantityRequired)
        {
            if (Status != "Draft")
                throw new InvalidOperationException("Cannot modify a picking list that is not in Draft status");
                
            var item = _items.FirstOrDefault(i => i.Sku == sku);
            if (item == null)
                throw new InvalidOperationException($"SKU {sku} does not exist in this picking list");
                
            item.UpdateQuantityRequired(quantityRequired);
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new PickingListItemUpdatedEvent(this, sku, quantityRequired));
        }

        public void RemoveItem(string sku)
        {
            if (Status != "Draft")
                throw new InvalidOperationException("Cannot modify a picking list that is not in Draft status");
                
            var item = _items.FirstOrDefault(i => i.Sku == sku);
            if (item == null)
                throw new InvalidOperationException($"SKU {sku} does not exist in this picking list");
                
            _items.Remove(item);
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new PickingListItemRemovedEvent(this, sku));
        }

        public void Release()
        {
            if (Status != "Draft")
                throw new InvalidOperationException("Only Draft picking lists can be released");
                
            if (!_items.Any())
                throw new InvalidOperationException("Cannot release a picking list with no items");
                
            if (_items.Any(i => i.QuantityAllocated < i.QuantityRequired))
                throw new InvalidOperationException("Cannot release a picking list with insufficient allocated quantities");
                
            Status = "Released";
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new PickingListReleasedEvent(this));
        }

        public void Assign(string userId)
        {
            if (Status != "Released")
                throw new InvalidOperationException("Only Released picking lists can be assigned");
                
            AssignedTo = userId;
            Status = "In Progress";
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new PickingListAssignedEvent(this, userId));
        }

        public void Unassign()
        {
            if (Status != "In Progress")
                throw new InvalidOperationException("Only In Progress picking lists can be unassigned");
                
            string previousUser = AssignedTo;
            AssignedTo = null;
            Status = "Released";
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new PickingListUnassignedEvent(this, previousUser));
        }

        public void Complete()
        {
            if (Status != "In Progress")
                throw new InvalidOperationException("Only In Progress picking lists can be completed");
                
            if (_items.Any(i => i.Status != "Picked"))
                throw new InvalidOperationException("Cannot complete a picking list with unpicked items");
                
            Status = "Completed";
            CompletedAt = DateTime.UtcNow;
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new PickingListCompletedEvent(this));
        }

        public void Cancel(string reason)
        {
            if (Status == "Completed" || Status == "Cancelled")
                throw new InvalidOperationException("Cannot cancel a picking list that is already completed or cancelled");
                
            Status = "Cancelled";
            Notes = string.IsNullOrEmpty(Notes) ? reason : $"{Notes}\nCancelled: {reason}";
            ModifiedAt = DateTime.UtcNow;
            
            // Deallocate all items
            foreach (var item in _items)
            {
                item.CancelPicking();
            }
            
            AddDomainEvent(new PickingListCancelledEvent(this, reason));
        }

        public void UpdatePriority(string priority)
        {
            if (!new[] { "Low", "Normal", "High", "Urgent" }.Contains(priority))
                throw new ArgumentException("Invalid priority value", nameof(priority));
                
            Priority = priority;
            ModifiedAt = DateTime.UtcNow;
            
            AddDomainEvent(new PickingListPriorityUpdatedEvent(this, priority));
        }
    }
}