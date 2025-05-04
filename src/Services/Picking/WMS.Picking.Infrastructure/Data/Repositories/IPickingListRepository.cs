using WMS.Picking.Domain.Entities;
using WMS.SharedKernel.Interfaces;

namespace WMS.Picking.Infrastructure.Data.Repositories
{
    public interface IPickingListRepository : IRepository<PickingList>
    {
        Task<string> GetNextPickingListNumberAsync();
    }
}