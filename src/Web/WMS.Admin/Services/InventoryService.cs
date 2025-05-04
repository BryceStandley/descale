namespace descale.Web.WMS.Admin.Services
{
    public class InventoryService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(IHttpClientFactory clientFactory, ILogger<InventoryService> logger)
        {
            _httpClient = clientFactory.CreateClient("InventoryAPI");
            _logger = logger;
        }

        public async Task<IEnumerable<InventoryItemDto>> GetInventoryItemsAsync(int pageSize = 10, int pageNumber = 1, string searchTerm = "")
        {
            var response = await _httpClient.GetAsync($"api/inventoryitems?pageSize={pageSize}&pageNumber={pageNumber}&searchTerm={Uri.EscapeDataString(searchTerm)}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<IEnumerable<InventoryItemDto>>();
        }

        public async Task<InventoryItemDto> GetInventoryItemByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/inventoryitems/{id}");
            
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
                
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<InventoryItemDto>();
        }

        public async Task<InventoryItemDto> CreateInventoryItemAsync(CreateInventoryItemCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync("api/inventoryitems", command);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<InventoryItemDto>();
        }

        public async Task UpdateInventoryItemAsync(UpdateInventoryItemCommand command)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/inventoryitems/{command.Id}", command);
            response.EnsureSuccessStatusCode();
        }

        public async Task AddStockAsync(AddStockCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/inventoryitems/{command.InventoryItemId}/add-stock", command);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveStockAsync(RemoveStockCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/inventoryitems/{command.InventoryItemId}/remove-stock", command);
            response.EnsureSuccessStatusCode();
        }

        public async Task AllocateStockAsync(AllocateStockCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/inventoryitems/{command.InventoryItemId}/allocate-stock", command);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeallocateStockAsync(DeallocateStockCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/inventoryitems/{command.InventoryItemId}/deallocate-stock", command);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<LocationDto>> GetLocationsAsync(int pageSize = 10, int pageNumber = 1, string searchTerm = "")
        {
            var response = await _httpClient.GetAsync($"api/locations?pageSize={pageSize}&pageNumber={pageNumber}&searchTerm={Uri.EscapeDataString(searchTerm)}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<IEnumerable<LocationDto>>();
        }

        public async Task<LocationDto> GetLocationByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/locations/{id}");
            
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
                
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<LocationDto>();
        }

        public async Task<LocationDto> CreateLocationAsync(CreateLocationCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync("api/locations", command);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<LocationDto>();
        }

        public async Task UpdateLocationAsync(UpdateLocationCommand command)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/locations/{command.Id}", command);
            response.EnsureSuccessStatusCode();
        }

        public async Task ActivateLocationAsync(Guid id)
        {
            var response = await _httpClient.PostAsync($"api/locations/{id}/activate", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeactivateLocationAsync(Guid id)
        {
            var response = await _httpClient.PostAsync($"api/locations/{id}/deactivate", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task AddStockToLocationAsync(AddStockToLocationCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/locations/{command.LocationId}/add-stock", command);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveStockFromLocationAsync(RemoveStockFromLocationCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/locations/{command.LocationId}/remove-stock", command);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<StockOnHandDto>> GetLocationStockAsync(Guid locationId)
        {
            var response = await _httpClient.GetAsync($"api/locations/{locationId}/stock");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<IEnumerable<StockOnHandDto>>();
        }
    }
}