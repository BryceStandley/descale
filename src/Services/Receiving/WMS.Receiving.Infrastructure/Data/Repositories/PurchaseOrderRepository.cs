using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WMS.Receiving.Domain.Entities;
using WMS.Receiving.Domain.Repositories;
using WMS.Receiving.Infrastructure.Data;
using WMS.SharedKernel.Interfaces;

namespace WMS.Receiving.Infrastructure.Data.Repositories
{
    public class PurchaseOrderRepository : EfRepository<PurchaseOrder>, IPurchaseOrderRepository
    {
        private readonly ReceivingDbContext _dbContext;
        
        public PurchaseOrderRepository(ReceivingDbContext dbContext) 
            : base(dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<string> GetNextPurchaseOrderNumberAsync()
        {
            var today = DateTime.UtcNow.Date;
            var prefix = $"PO-{today:yyyyMMdd}-";
            
            var lastPurchaseOrder = await _dbContext.PurchaseOrders
                .Where(p => p.Number.StartsWith(prefix))
                .OrderByDescending(p => p.Number)
                .FirstOrDefaultAsync();
                
            if (lastPurchaseOrder == null)
            {
                return $"{prefix}0001";
            }
            
            var lastNumber = int.Parse(lastPurchaseOrder.Number.Substring(prefix.Length));
            return $"{prefix}{(lastNumber + 1):D4}";
        }
    }
}