using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WMS.BuildingBlocks.EventBus;
using WMS.Inventory.Application.IntegrationEvents.Events;

namespace WMS.Receiving.API.IntegrationEventHandlers
{
    public class InventoryItemCreatedIntegrationEventHandler : IIntegrationEventHandler<InventoryItemCreatedIntegrationEvent>
    {
        private readonly ILogger<InventoryItemCreatedIntegrationEventHandler> _logger;

        public InventoryItemCreatedIntegrationEventHandler(
            ILogger<InventoryItemCreatedIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(InventoryItemCreatedIntegrationEvent @event)
        {
            _logger.LogInformation("Handling InventoryItemCreated event for SKU {Sku}", @event.Sku);
            
            // Here we might want to update local cache or reference data about inventory items
            // that could be used during receiving process
            
            await Task.CompletedTask;
        }
    }
}