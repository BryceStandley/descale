using System;
using System.Linq.Expressions;
using WMS.Receiving.Domain.Entities;
using WMS.SharedKernel.Interfaces;

namespace WMS.Receiving.Infrastructure.Data.Specifications
{
    public class PurchaseOrdersWithPaginationSpecification : BaseSpecification<PurchaseOrder>
    {
        public PurchaseOrdersWithPaginationSpecification(int pageSize, int pageNumber, string status)
            : base(BuildCriteria(status))
        {
            AddInclude(p => p.OrderLines);
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
            ApplyOrderByDescending(p => p.CreatedAt);
        }
        
        private static Expression<Func<PurchaseOrder, bool>> BuildCriteria(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return p => true;
            }
            
            return p => p.Status == status;
        }
    }
}