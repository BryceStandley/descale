using System;
using WMS.Receiving.Domain.Entities;
using WMS.SharedKernel.Interfaces;

namespace WMS.Receiving.Infrastructure.Data.Specifications
{
    public class ReceiptWithLinesSpecification : BaseSpecification<Receipt>
    {
        public ReceiptWithLinesSpecification(Guid receiptId)
            : base(r => r.Id == receiptId)
        {
            AddInclude(r => r.ReceiptLines);
        }
    }
}