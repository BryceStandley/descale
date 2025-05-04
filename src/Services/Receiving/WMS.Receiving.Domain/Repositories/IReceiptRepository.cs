using System.Threading.Tasks;
using WMS.Receiving.Domain.Entities;
using WMS.SharedKernel.Interfaces;

namespace WMS.Receiving.Domain.Repositories
{
    public interface IReceiptRepository : IRepository<Receipt>
    {
        Task<string> GetNextReceiptNumberAsync();
    }
}