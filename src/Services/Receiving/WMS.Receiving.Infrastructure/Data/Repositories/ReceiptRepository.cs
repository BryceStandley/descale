using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WMS.Receiving.Domain.Entities;
using WMS.Receiving.Domain.Repositories;
using WMS.Receiving.Infrastructure.Data;
using WMS.SharedKernel.Interfaces;

namespace WMS.Receiving.Infrastructure.Data.Repositories
{
    public class ReceiptRepository : EfRepository<Receipt>, IReceiptRepository
    {
        private readonly ReceivingDbContext _dbContext;
        
        public ReceiptRepository(ReceivingDbContext dbContext) 
            : base(dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<string> GetNextReceiptNumberAsync()
        {
            var today = DateTime.UtcNow.Date;
            var prefix = $"GRN-{today:yyyyMMdd}-";
            
            var lastReceipt = await _dbContext.Receipts
                .Where(r => r.ReceiptNumber.StartsWith(prefix))
                .OrderByDescending(r => r.ReceiptNumber)
                .FirstOrDefaultAsync();
                
            if (lastReceipt == null)
            {
                return $"{prefix}0001";
            }
            
            var lastNumber = int.Parse(lastReceipt.ReceiptNumber.Substring(prefix.Length));
            return $"{prefix}{(lastNumber + 1):D4}";
        }
    }
}