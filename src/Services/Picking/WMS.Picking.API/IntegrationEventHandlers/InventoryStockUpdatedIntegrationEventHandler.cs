using WMS.EventBus;
using WMS.Picking.Infrastructure.Data.Repositories;

namespace WMS.Picking.API.IntegrationEventHandlers
{
    public class InventoryStockUpdatedIntegrationEventHandler : IIntegrationEventHandler<InventoryStockUpdatedIntegrationEvent>
    {
        private readonly IPickingListRepository _pickingListRepository;
        private readonly ILogger<InventoryStockUpdatedIntegrationEventHandler> _logger;

        public InventoryStockUpdatedIntegrationEventHandler(
            IPickingListRepository pickingListRepository,
            ILogger<InventoryStockUpdatedIntegrationEventHandler> logger)
        {
            _pickingListRepository = pickingListRepository;
            _logger = logger;
        }

        public async Task HandleAsync(InventoryStockUpdatedIntegrationEvent @event)
        {
            _logger.LogInformation("Handling InventoryStockUpdated event for SKU {Sku}", @event.Sku);
            
            // Find all picking lists that have this item and are in Draft status
            var spec = new PickingListsWithItemSkuSpecification(@event.Sku, "Draft");
            var pickingLists = await _pickingListRepository.ListAsync(spec);
            
            foreach (var pickingList in pickingLists)
            {
                // Update the allocated quantity for this item
                var item = pickingList.Items.FirstOrDefault(i => i.Sku == @event.Sku);
                if (item != null)
                {
                    // Update the allocation based on new available quantity
                    int previousAllocation = item.QuantityAllocated;
                    int newAllocation = Math.Min(item.QuantityRequired, @event.QuantityAvailable);
                    
                    if (newAllocation != previousAllocation)
                    {
                        try
                        {
                            // For Draft picking lists, we can update allocations
                            if (pickingList.Status == "Draft")
                            {
                                if (newAllocation > previousAllocation)
                                {
                                    _logger.LogInformation("Increasing allocation for SKU {Sku} in picking list {Id} from {Previous} to {New}",
                                        @event.Sku, pickingList.Id, previousAllocation, newAllocation);
                                    
                                    // Simulate updating the allocation
                                    item.AllocateQuantity(newAllocation - previousAllocation);
                                }
                                else
                                {
                                    _logger.LogWarning("Allocation for SKU {Sku} in picking list {Id} would be reduced from {Previous} to {New}, but we don't deallocate automatically",
                                        @event.Sku, pickingList.Id, previousAllocation, newAllocation);
                                }
                                
                                await _pickingListRepository.UpdateAsync(pickingList);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error updating allocation for SKU {Sku} in picking list {Id}", 
                                @event.Sku, pickingList.Id);
                        }
                    }
                }
            }
        }
    }
}