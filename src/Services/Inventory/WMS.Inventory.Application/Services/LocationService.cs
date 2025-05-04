using Microsoft.Extensions.Logging;
using WMS.Inventory.Application.Commands;
using WMS.Inventory.Application.DTOs;
using WMS.Inventory.Domain.Entities;

namespace WMS.Inventory.Application.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IInventoryItemRepository _inventoryItemRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LocationService> _logger;

        public LocationService(
            ILocationRepository locationRepository,
            IInventoryItemRepository inventoryItemRepository,
            IMapper mapper,
            ILogger<LocationService> logger)
        {
            _locationRepository = locationRepository;
            _inventoryItemRepository = inventoryItemRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<LocationDto>> GetLocationsAsync(int pageSize, int pageNumber, string searchTerm)
        {
            var spec = new LocationsWithPaginationSpecification(pageSize, pageNumber, searchTerm);
            var locations = await _locationRepository.ListAsync(spec);
            
            return _mapper.Map<IEnumerable<LocationDto>>(locations);
        }

        public async Task<LocationDto> GetLocationByIdAsync(Guid id)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            
            if (location == null)
                return null;
                
            return _mapper.Map<LocationDto>(location);
        }

        public async Task<LocationDto> GetLocationByCodeAsync(string code)
        {
            var spec = new LocationByCodeSpecification(code);
            var location = await _locationRepository.FirstOrDefaultAsync(spec);
            
            if (location == null)
                return null;
                
            return _mapper.Map<LocationDto>(location);
        }

        public async Task<LocationDto> CreateLocationAsync(CreateLocationCommand command)
        {
            // Check if code already exists
            var existingLocation = await GetLocationByCodeAsync(command.Code);
            if (existingLocation != null)
                throw new ApplicationException($"Location with code {command.Code} already exists.");
                
            var location = new Location(
                command.Code,
                command.Name,
                command.Zone,
                command.Aisle,
                command.Bay,
                command.Level,
                command.Position,
                command.Type,
                command.MaxWeight);
                
            await _locationRepository.AddAsync(location);
            
            _logger.LogInformation("Location {Code} created with ID {Id}", command.Code, location.Id);
            
            return _mapper.Map<LocationDto>(location);
        }

        public async Task UpdateLocationAsync(UpdateLocationCommand command)
        {
            var location = await _locationRepository.GetByIdAsync(command.Id);
            
            if (location == null)
                throw new NotFoundException($"Location with ID {command.Id} not found.");
                
            // Update properties (using reflection or a custom method on the entity)
            // This is a simplified example - in a real application you would need to handle this more carefully
            
            await _locationRepository.UpdateAsync(location);
            
            _logger.LogInformation("Location {Id} updated", command.Id);
        }

        public async Task ActivateLocationAsync(Guid id)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            
            if (location == null)
                throw new NotFoundException($"Location with ID {id} not found.");
                
            location.Activate();
            
            await _locationRepository.UpdateAsync(location);
            
            _logger.LogInformation("Location {Id} activated", id);
        }

        public async Task DeactivateLocationAsync(Guid id)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            
            if (location == null)
                throw new NotFoundException($"Location with ID {id} not found.");
                
            location.Deactivate();
            
            await _locationRepository.UpdateAsync(location);
            
            _logger.LogInformation("Location {Id} deactivated", id);
        }

        public async Task AddStockToLocationAsync(AddStockToLocationCommand command)
        {
            var location = await _locationRepository.GetByIdAsync(command.LocationId);
            
            if (location == null)
                throw new NotFoundException($"Location with ID {command.LocationId} not found.");
                
            var item = await _inventoryItemRepository.GetByIdAsync(command.InventoryItemId);
            
            if (item == null)
                throw new NotFoundException($"Inventory item with ID {command.InventoryItemId} not found.");
                
            location.AddStock(item, command.Quantity);
            
            await _locationRepository.UpdateAsync(location);
            
            _logger.LogInformation("Added {Quantity} of item {ItemId} to location {LocationId}", 
                command.Quantity, command.InventoryItemId, command.LocationId);
        }

        public async Task RemoveStockFromLocationAsync(RemoveStockFromLocationCommand command)
        {
            var location = await _locationRepository.GetByIdAsync(command.LocationId);
            
            if (location == null)
                throw new NotFoundException($"Location with ID {command.LocationId} not found.");
                
            var item = await _inventoryItemRepository.GetByIdAsync(command.InventoryItemId);
            
            if (item == null)
                throw new NotFoundException($"Inventory item with ID {command.InventoryItemId} not found.");
                
            location.RemoveStock(item, command.Quantity);
            
            await _locationRepository.UpdateAsync(location);
            
            _logger.LogInformation("Removed {Quantity} of item {ItemId} from location {LocationId}", 
                command.Quantity, command.InventoryItemId, command.LocationId);
        }

        public async Task<IEnumerable<StockOnHandDto>> GetLocationStockAsync(Guid locationId)
        {
            var spec = new LocationWithStockSpecification(locationId);
            var location = await _locationRepository.FirstOrDefaultAsync(spec);
            
            if (location == null)
                throw new NotFoundException($"Location with ID {locationId} not found.");
                
            return _mapper.Map<IEnumerable<StockOnHandDto>>(location.StockItems);
        }
    }
}