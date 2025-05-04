using WMS.Inventory.Application.Commands;
using WMS.Inventory.Application.DTOs;

namespace WMS.Inventory.Application.Services
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationDto>> GetLocationsAsync(int pageSize, int pageNumber, string searchTerm);
        Task<LocationDto> GetLocationByIdAsync(Guid id);
        Task<LocationDto> GetLocationByCodeAsync(string code);
        Task<LocationDto> CreateLocationAsync(CreateLocationCommand command);
        Task UpdateLocationAsync(UpdateLocationCommand command);
        Task ActivateLocationAsync(Guid id);
        Task DeactivateLocationAsync(Guid id);
        Task AddStockToLocationAsync(AddStockToLocationCommand command);
        Task RemoveStockFromLocationAsync(RemoveStockFromLocationCommand command);
        Task<IEnumerable<StockOnHandDto>> GetLocationStockAsync(Guid locationId);
    }
}