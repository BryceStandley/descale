using Microsoft.EntityFrameworkCore;
using WMS.Picking.Domain.Entities;

namespace WMS.Picking.Infrastructure.Data.Repositories
{
    public class PickingListRepository : EfRepository<PickingList>, IPickingListRepository
    {
        private readonly PickingDbContext _dbContext;
        
        public PickingListRepository(PickingDbContext dbContext) 
            : base(dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<string> GetNextPickingListNumberAsync()
        {
            var today = DateTime.UtcNow.Date;
            var prefix = $"PL-{today:yyyyMMdd}-";
            
            var lastPickingList = await _dbContext.PickingLists
                .Where(p => p.Number.StartsWith(prefix))
                .OrderByDescending(p => p.Number)
                .FirstOrDefaultAsync();
                
            if (lastPickingList == null)
            {
                return $"{prefix}0001";
            }
            
            var lastNumber = int.Parse(lastPickingList.Number.Substring(prefix.Length));
            return $"{prefix}{(lastNumber + 1):D4}";
        }
    }
}