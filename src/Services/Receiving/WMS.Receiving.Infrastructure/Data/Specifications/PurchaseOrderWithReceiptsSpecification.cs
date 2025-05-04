using System;
using WMS.Receiving.Domain.Entities;
using WMS.SharedKernel.Interfaces;

namespace WMS.Receiving.Infrastructure.Data.Specifications
{
    public class PurchaseOrderWithReceiptsSpecification : BaseSpecification<PurchaseOrder>
    {
        public PurchaseOrderWithReceiptsSpecification(Guid purchaseOrderId)
            : base(p => p.Id == purchaseOrderId)
        {
            AddInclude(p => p.Receipts);
        }
    }
}