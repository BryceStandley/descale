using Microsoft.EntityFrameworkCore;
using WMS.Picking.Domain.Entities;
using WMS.SharedKernel.Entities;

namespace WMS.Picking.Infrastructure.Data
{
    public class PickingDbContext : DbContext
    {
        public PickingDbContext(DbContextOptions<PickingDbContext> options) : base(options) { }
        
        public DbSet<PickingList> PickingLists { get; set; }
        public DbSet<PickingListItem> PickingListItems { get; set; }
        public DbSet<PickingLocation> PickingLocations { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PickingDbContext).Assembly);
            
            // Configure entity relationships
            modelBuilder.Entity<PickingList>()
                .HasMany(p => p.Items)
                .WithOne(i => i.PickingList)
                .HasForeignKey(i => i.PickingListId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<PickingListItem>()
                .HasMany(i => i.PickingLocations)
                .WithOne(l => l.PickingListItem)
                .HasForeignKey(l => l.PickingListItemId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Configure indexes
            modelBuilder.Entity<PickingList>()
                .HasIndex(p => p.Number)
                .IsUnique();
                
            modelBuilder.Entity<PickingListItem>()
                .HasIndex(i => new { i.PickingListId, i.Sku })
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
                    // Publish the event using MediatR
                    // await _mediator.Publish(domainEvent);
                }
            }
        }
    }
}