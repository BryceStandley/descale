using WMS.Inventory.Application.Commands;
using WMS.Inventory.Application.DTOs;

namespace WMS.Inventory.Application.Services
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryItemDto>> GetInventoryItemsAsync(int pageSize, int pageNumber, string searchTerm);
        Task<InventoryItemDto> GetInventoryItemByIdAsync(Guid id);
        Task<InventoryItemDto> GetInventoryItemBySkuAsync(string sku);
        Task<InventoryItemDto> CreateInventoryItemAsync(CreateInventoryItemCommand command);
        Task UpdateInventoryItemAsync(UpdateInventoryItemCommand command);
        Task AddStockAsync(AddStockCommand command);
        Task RemoveStockAsync(RemoveStockCommand command);
        Task AllocateStockAsync(AllocateStockCommand command);
        Task DeallocateStockAsync(DeallocateStockCommand command);
    }
}