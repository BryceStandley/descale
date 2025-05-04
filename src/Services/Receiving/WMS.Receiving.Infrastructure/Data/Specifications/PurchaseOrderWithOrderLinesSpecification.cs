using System;
using WMS.Receiving.Domain.Entities;
using WMS.SharedKernel.Interfaces;

namespace WMS.Receiving.Infrastructure.Data.Specifications
{
    public class PurchaseOrderWithOrderLinesSpecification : BaseSpecification<PurchaseOrder>
    {
        public PurchaseOrderWithOrderLinesSpecification(Guid purchaseOrderId)
            : base(p => p.Id == purchaseOrderId)
        {
            AddInclude(p => p.OrderLines);
        }
    }
}