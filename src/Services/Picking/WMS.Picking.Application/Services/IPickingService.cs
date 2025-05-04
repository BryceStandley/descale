namespace WMS.Picking.Application.Services
{
    public interface IPickingService
    {
        Task<IEnumerable<PickingListDto>> GetPickingListsAsync(int pageSize, int pageNumber, string status);
        Task<PickingListDto> GetPickingListByIdAsync(Guid id);
        Task<PickingListDto> CreatePickingListAsync(CreatePickingListCommand command);
        Task AddPickingListItemAsync(AddPickingListItemCommand command);
        Task UpdatePickingListItemAsync(UpdatePickingListItemCommand command);
        Task RemovePickingListItemAsync(Guid pickingListId, string sku);
        Task ReleasePickingListAsync(Guid pickingListId);
        Task AssignPickingListAsync(Guid pickingListId, string userId);
        Task UnassignPickingListAsync(Guid pickingListId);
        Task CompletePickingListAsync(Guid pickingListId);
        Task CancelPickingListAsync(Guid pickingListId, string reason);
        Task UpdatePickingListPriorityAsync(Guid pickingListId, string priority);
        Task RecordPickAsync(RecordPickCommand command);
        Task MarkItemAsShortAsync(Guid pickingListId, string sku, string reason);
    }
}