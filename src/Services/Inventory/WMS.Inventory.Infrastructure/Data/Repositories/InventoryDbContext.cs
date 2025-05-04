using Microsoft.EntityFrameworkCore;
using WMS.Inventory.Domain.Entities;
using WMS.SharedKernel.Entities;

namespace WMS.Inventory.Infrastructure.Data.Repositories
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }
        
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<StockOnHand> StockOnHands { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);
            
            // Example configuration
            modelBuilder.Entity<InventoryItem>()
                .HasIndex(i => i.Sku)
                .IsUnique();
                
            modelBuilder.Entity<Location>()
                .HasIndex(l => l.Code)
                .IsUnique();
                
            modelBuilder.Entity<StockOnHand>()
                .HasIndex(s => new { s.LocationId, s.InventoryItemId })
                .IsUnique();
        }
        
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Dispatch domain events before saving changes
            await DispatchDomainEventsAsync();
            
            return await base.SaveChangesAsync(cancellationToken);
        }
        
        private async Task DispatchDomainEventsAsync()
        {
            var domainEventEntities = ChangeTracker.Entries<Entity>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents != null && e.DomainEvents.Any())
                .ToArray();
                
            foreach (var entity in domainEventEntities)
            {
                var events = entity.DomainEvents.ToArray();
                entity.ClearDomainEvents();
                
                foreach (var domainEvent in events)
                {
                    // Here you would typically publish the event using MediatR or another event bus
                    // await _mediator.Publish(domainEvent);
                }
            }
        }
    }
}