using System.Threading.Tasks;
using WMS.Receiving.Domain.Entities;
using WMS.SharedKernel.Interfaces;

namespace WMS.Receiving.Domain.Repositories
{
    public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
    {
        Task<string> GetNextPurchaseOrderNumberAsync();
    }
}