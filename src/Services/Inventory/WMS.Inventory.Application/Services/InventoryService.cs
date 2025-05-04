using Microsoft.Extensions.Logging;
using WMS.Inventory.Application.Commands;
using WMS.Inventory.Application.DTOs;
using WMS.Inventory.Domain.Entities;

namespace WMS.Inventory.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryItemRepository _inventoryItemRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(
            IInventoryItemRepository inventoryItemRepository,
            IMapper mapper,
            ILogger<InventoryService> logger)
        {
            _inventoryItemRepository = inventoryItemRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<InventoryItemDto>> GetInventoryItemsAsync(int pageSize, int pageNumber, string searchTerm)
        {
            var spec = new InventoryItemsWithPaginationSpecification(pageSize, pageNumber, searchTerm);
            var items = await _inventoryItemRepository.ListAsync(spec);
            
            return _mapper.Map<IEnumerable<InventoryItemDto>>(items);
        }

        public async Task<InventoryItemDto> GetInventoryItemByIdAsync(Guid id)
        {
            var item = await _inventoryItemRepository.GetByIdAsync(id);
            
            if (item == null)
                return null;
                
            return _mapper.Map<InventoryItemDto>(item);
        }

        public async Task<InventoryItemDto> GetInventoryItemBySkuAsync(string sku)
        {
            var spec = new InventoryItemBySkuSpecification(sku);
            var item = await _inventoryItemRepository.FirstOrDefaultAsync(spec);
            
            if (item == null)
                return null;
                
            return _mapper.Map<InventoryItemDto>(item);
        }

        public async Task<InventoryItemDto> CreateInventoryItemAsync(CreateInventoryItemCommand command)
        {
            // Check if SKU already exists
            var existingItem = await GetInventoryItemBySkuAsync(command.Sku);
            if (existingItem != null)
                throw new ApplicationException($"Inventory item with SKU {command.Sku} already exists.");
                
            var item = new InventoryItem(
                command.Sku,
                command.Name,
                command.Description,
                command.Category,
                command.UnitOfMeasure,
                command.InitialQuantity,
                command.Cost);
                
            await _inventoryItemRepository.AddAsync(item);
            
            _logger.LogInformation("Inventory item {Sku} created with ID {Id}", command.Sku, item.Id);
            
            return _mapper.Map<InventoryItemDto>(item);
        }

        public async Task UpdateInventoryItemAsync(UpdateInventoryItemCommand command)
        {
            var item = await _inventoryItemRepository.GetByIdAsync(command.Id);
            
            if (item == null)
                throw new NotFoundException($"Inventory item with ID {command.Id} not found.");
                
            // Update properties (using reflection or a custom method on the entity)
            // This is a simplified example - in a real application you would need to handle this more carefully
            
            await _inventoryItemRepository.UpdateAsync(item);
            
            _logger.LogInformation("Inventory item {Id} updated", command.Id);
        }

        public async Task AddStockAsync(AddStockCommand command)
        {
            var item = await _inventoryItemRepository.GetByIdAsync(command.InventoryItemId);
            
            if (item == null)
                throw new NotFoundException($"Inventory item with ID {command.InventoryItemId} not found.");
                
            item.AddStock(command.Quantity, command.Reason);
            
            await _inventoryItemRepository.UpdateAsync(item);
            
            _logger.LogInformation("Added {Quantity} stock to inventory item {Id}. Reason: {Reason}", 
                command.Quantity, command.InventoryItemId, command.Reason);
        }

        public async Task RemoveStockAsync(RemoveStockCommand command)
        {
            var item = await _inventoryItemRepository.GetByIdAsync(command.InventoryItemId);
            
            if (item == null)
                throw new NotFoundException($"Inventory item with ID {command.InventoryItemId} not found.");
                
            item.RemoveStock(command.Quantity, command.Reason);
            
            await _inventoryItemRepository.UpdateAsync(item);
            
            _logger.LogInformation("Removed {Quantity} stock from inventory item {Id}. Reason: {Reason}", 
                command.Quantity, command.InventoryItemId, command.Reason);
        }

        public async Task AllocateStockAsync(AllocateStockCommand command)
        {
            var item = await _inventoryItemRepository.GetByIdAsync(command.InventoryItemId);
            
            if (item == null)
                throw new NotFoundException($"Inventory item with ID {command.InventoryItemId} not found.");
                
            item.AllocateStock(command.Quantity);
            
            await _inventoryItemRepository.UpdateAsync(item);
            
            _logger.LogInformation("Allocated {Quantity} stock for inventory item {Id}. Reference: {Reference}", 
                command.Quantity, command.InventoryItemId, command.Reference);
        }

        public async Task DeallocateStockAsync(DeallocateStockCommand command)
        {
            var item = await _inventoryItemRepository.GetByIdAsync(command.InventoryItemId);
            
            if (item == null)
                throw new NotFoundException($"Inventory item with ID {command.InventoryItemId} not found.");
                
            item.DeallocateStock(command.Quantity);
            
            await _inventoryItemRepository.UpdateAsync(item);
            
            _logger.LogInformation("Deallocated {Quantity} stock for inventory item {Id}. Reference: {Reference}", 
                command.Quantity, command.InventoryItemId, command.Reference);
        }
    }
}