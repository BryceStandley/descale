namespace WMS.Picking.Application.Services
{
    public class PickingService : IPickingService
    {
        private readonly IPickingListRepository _pickingListRepository;
        private readonly IEventBus _eventBus;
        private readonly ILogger<PickingService> _logger;

        public PickingService(
            IPickingListRepository pickingListRepository,
            IEventBus eventBus,
            ILogger<PickingService> logger)
        {
            _pickingListRepository = pickingListRepository;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task<IEnumerable<PickingListDto>> GetPickingListsAsync(int pageSize, int pageNumber, string status)
        {
            var spec = new PickingListsWithPaginationSpecification(pageSize, pageNumber, status);
            var pickingLists = await _pickingListRepository.ListAsync(spec);
            
            return pickingLists.Select(MapToDto);
        }

        public async Task<PickingListDto> GetPickingListByIdAsync(Guid id)
        {
            var spec = new PickingListWithItemsSpecification(id);
            var pickingList = await _pickingListRepository.FirstOrDefaultAsync(spec);
            
            if (pickingList == null)
                return null;
                
            return MapToDto(pickingList);
        }

        public async Task<PickingListDto> CreatePickingListAsync(CreatePickingListCommand command)
        {
            var nextNumber = await _pickingListRepository.GetNextPickingListNumberAsync();
            
            var pickingList = new PickingList(
                nextNumber,
                command.OrderNumber,
                command.Priority,
                command.Notes);
                
            await _pickingListRepository.AddAsync(pickingList);
            
            _logger.LogInformation("Picking list {Number} created with ID {Id}", nextNumber, pickingList.Id);
            
            return MapToDto(pickingList);
        }

        public async Task AddPickingListItemAsync(AddPickingListItemCommand command)
        {
            var pickingList = await _pickingListRepository.GetByIdAsync(command.PickingListId);
            
            if (pickingList == null)
                throw new Exception($"Picking list with ID {command.PickingListId} not found.");
                
            pickingList.AddItem(
                command.Sku,
                command.ItemName,
                command.QuantityRequired,
                command.QuantityAllocated);
                
            await _pickingListRepository.UpdateAsync(pickingList);
            
            _logger.LogInformation("Item {Sku} added to picking list {Id}", command.Sku, command.PickingListId);
        }

        public async Task UpdatePickingListItemAsync(UpdatePickingListItemCommand command)
        {
            var pickingList = await _pickingListRepository.GetByIdAsync(command.PickingListId);
            
            if (pickingList == null)
                throw new Exception($"Picking list with ID {command.PickingListId} not found.");
                
            pickingList.UpdateItem(command.Sku, command.QuantityRequired);
            
            await _pickingListRepository.UpdateAsync(pickingList);
            
            _logger.LogInformation("Item {Sku} updated in picking list {Id}", command.Sku, command.PickingListId);
        }

        public async Task RemovePickingListItemAsync(Guid pickingListId, string sku)
        {
            var pickingList = await _pickingListRepository.GetByIdAsync(pickingListId);
            
            if (pickingList == null)
                throw new Exception($"Picking list with ID {pickingListId} not found.");
                
            pickingList.RemoveItem(sku);
            
            await _pickingListRepository.UpdateAsync(pickingList);
            
            _logger.LogInformation("Item {Sku} removed from picking list {Id}", sku, pickingListId);
        }

        public async Task ReleasePickingListAsync(Guid pickingListId)
        {
            var pickingList = await _pickingListRepository.GetByIdAsync(pickingListId);
            
            if (pickingList == null)
                throw new Exception($"Picking list with ID {pickingListId} not found.");
                
            pickingList.Release();
            
            await _pickingListRepository.UpdateAsync(pickingList);
            
            await _eventBus.PublishAsync(new PickingListReleasedIntegrationEvent(
                pickingListId,
                pickingList.Number,
                pickingList.OrderNumber));
            
            _logger.LogInformation("Picking list {Id} released", pickingListId);
        }

        public async Task AssignPickingListAsync(Guid pickingListId, string userId)
        {
            var pickingList = await _pickingListRepository.GetByIdAsync(pickingListId);
            
            if (pickingList == null)
                throw new Exception($"Picking list with ID {pickingListId} not found.");
                
            pickingList.Assign(userId);
            
            await _pickingListRepository.UpdateAsync(pickingList);
            
            await _eventBus.PublishAsync(new PickingListAssignedIntegrationEvent(
                pickingListId,
                pickingList.Number,
                userId));
            
            _logger.LogInformation("Picking list {Id} assigned to user {UserId}", pickingListId, userId);
        }

        public async Task UnassignPickingListAsync(Guid pickingListId)
        {
            var pickingList = await _pickingListRepository.GetByIdAsync(pickingListId);
            
            if (pickingList == null)
                throw new Exception($"Picking list with ID {pickingListId} not found.");
                
            string previousUser = pickingList.AssignedTo;
            pickingList.Unassign();
            
            await _pickingListRepository.UpdateAsync(pickingList);
            
            await _eventBus.PublishAsync(new PickingListUnassignedIntegrationEvent(
                pickingListId,
                pickingList.Number,
                previousUser));
            
            _logger.LogInformation("Picking list {Id} unassigned from user {UserId}", pickingListId, previousUser);
        }

        public async Task CompletePickingListAsync(Guid pickingListId)
        {
            var spec = new PickingListWithItemsSpecification(pickingListId);
            var pickingList = await _pickingListRepository.FirstOrDefaultAsync(spec);
            
            if (pickingList == null)
                throw new Exception($"Picking list with ID {pickingListId} not found.");
                
            pickingList.Complete();
            
            await _pickingListRepository.UpdateAsync(pickingList);
            
            await _eventBus.PublishAsync(new PickingListCompletedIntegrationEvent(
                pickingListId,
                pickingList.Number,
                pickingList.OrderNumber));
            
            _logger.LogInformation("Picking list {Id} completed", pickingListId);
        }

        public async Task CancelPickingListAsync(Guid pickingListId, string reason)
        {
            var pickingList = await _pickingListRepository.GetByIdAsync(pickingListId);
            
            if (pickingList == null)
                throw new Exception($"Picking list with ID {pickingListId} not found.");
                
            pickingList.Cancel(reason);
            
            await _pickingListRepository.UpdateAsync(pickingList);
            
            await _eventBus.PublishAsync(new PickingListCancelledIntegrationEvent(
                pickingListId,
                pickingList.Number,
                reason));
            
            _logger.LogInformation("Picking list {Id} cancelled. Reason: {Reason}", pickingListId, reason);
        }

        public async Task UpdatePickingListPriorityAsync(Guid pickingListId, string priority)
        {
            var pickingList = await _pickingListRepository.GetByIdAsync(pickingListId);
            
            if (pickingList == null)
                throw new Exception($"Picking list with ID {pickingListId} not found.");
                
            pickingList.UpdatePriority(priority);
            
            await _pickingListRepository.UpdateAsync(pickingList);
            
            _logger.LogInformation("Picking list {Id} priority updated to {Priority}", pickingListId, priority);
        }

        public async Task RecordPickAsync(RecordPickCommand command)
        {
            var spec = new PickingListWithItemsSpecification(command.PickingListId);
            var pickingList = await _pickingListRepository.FirstOrDefaultAsync(spec);
            
            if (pickingList == null)
                throw new Exception($"Picking list with ID {command.PickingListId} not found.");
                
            var item = pickingList.Items.FirstOrDefault(i => i.Sku == command.Sku);
            
            if (item == null)
                throw new Exception($"Item with SKU {command.Sku} not found in picking list {command.PickingListId}");
                
            // Find picking location
            var location = item.PickingLocations.FirstOrDefault(l => l.LocationCode == command.LocationCode);
            
            if (location == null)
                throw new Exception($"Location {command.LocationCode} not assigned to item {command.Sku}");
                
            // Record the pick
            item.StartPicking();
            item.RecordPick(command.LocationCode, command.QuantityPicked);
            
            await _pickingListRepository.UpdateAsync(pickingList);
            
            // Check if all required quantity has been picked
            bool isItemFullyPicked = item.QuantityPicked >= item.QuantityRequired;
            
            // Publish an integration event for inventory
            await _eventBus.PublishAsync(new InventoryPickedIntegrationEvent(
                command.PickingListId,
                command.Sku,
                command.LocationCode,
                command.QuantityPicked,
                isItemFullyPicked));
            
            _logger.LogInformation("Recorded pick of {Quantity} of {Sku} from location {Location} for picking list {Id}", 
                command.QuantityPicked, command.Sku, command.LocationCode, command.PickingListId);
        }

        public async Task MarkItemAsShortAsync(Guid pickingListId, string sku, string reason)
        {
            var spec = new PickingListWithItemsSpecification(pickingListId);
            var pickingList = await _pickingListRepository.FirstOrDefaultAsync(spec);
            
            if (pickingList == null)
                throw new Exception($"Picking list with ID {pickingListId} not found.");
                
            var item = pickingList.Items.FirstOrDefault(i => i.Sku == sku);
            
            if (item == null)
                throw new Exception($"Item with SKU {sku} not found in picking list {pickingListId}");
                
            // Mark the item as short
            item.MarkAsShort(reason);
            
            await _pickingListRepository.UpdateAsync(pickingList);
            
            // Publish an integration event
            await _eventBus.PublishAsync(new PickingItemShortIntegrationEvent(
                pickingListId,
                sku,
                item.QuantityRequired,
                item.QuantityPicked,
                reason));
            
            _logger.LogInformation("Item {Sku} marked as short for picking list {Id}. Reason: {Reason}", 
                sku, pickingListId, reason);
        }

        private PickingListDto MapToDto(PickingList entity)
        {
            return new PickingListDto
            {
                Id = entity.Id,
                Number = entity.Number,
                OrderNumber = entity.OrderNumber,
                Status = entity.Status,
                AssignedTo = entity.AssignedTo,
                CreatedAt = entity.CreatedAt,
                ModifiedAt = entity.ModifiedAt,
                CompletedAt = entity.CompletedAt,
                Notes = entity.Notes,
                Priority = entity.Priority,
                Items = entity.Items.Select(item => new PickingListItemDto
                {
                    Id = item.Id,
                    Sku = item.Sku,
                    ItemName = item.ItemName,
                    QuantityRequired = item.QuantityRequired,
                    QuantityAllocated = item.QuantityAllocated,
                    QuantityPicked = item.QuantityPicked,
                    Status = item.Status,
                    PickingLocations = item.PickingLocations.Select(loc => new PickingLocationDto
                    {
                        Id = loc.Id,
                        LocationCode = loc.LocationCode,
                        Zone = loc.Zone,
                        QuantityToPick = loc.QuantityToPick,
                        QuantityPicked = loc.QuantityPicked,
                        Status = loc.Status,
                        Notes = loc.Notes
                    }).ToList()
                }).ToList()
            };
        }
    }
}