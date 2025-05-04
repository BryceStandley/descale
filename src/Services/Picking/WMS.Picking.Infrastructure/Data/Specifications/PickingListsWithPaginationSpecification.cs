using System.Linq.Expressions;
using WMS.Picking.Domain.Entities;

namespace WMS.Picking.Infrastructure.Data.Specifications
{
    public class PickingListsWithPaginationSpecification : BaseSpecification<PickingList>
    {
        public PickingListsWithPaginationSpecification(int pageSize, int pageNumber, string status)
            : base(BuildCriteria(status))
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
            ApplyOrderByDescending(p => p.CreatedAt);
        }
        
        private static Expression<Func<PickingList, bool>> BuildCriteria(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return p => true;
            }
            
            return p => p.Status == status;
        }
    }
}