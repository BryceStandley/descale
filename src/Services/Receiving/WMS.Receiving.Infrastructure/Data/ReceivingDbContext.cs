using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WMS.Receiving.Domain.Entities;
using WMS.SharedKernel.Entities;

namespace WMS.Receiving.Infrastructure.Data
{
    public class ReceivingDbContext : DbContext
    {
        public ReceivingDbContext(DbContextOptions<ReceivingDbContext> options) : base(options) { }
        
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderLine> PurchaseOrderLines { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<ReceiptLine> ReceiptLines { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReceivingDbContext).Assembly);
            
            // Configure entity relationships
            modelBuilder.Entity<PurchaseOrder>()
                .HasMany(p => p.OrderLines)
                .WithOne(l => l.PurchaseOrder)
                .HasForeignKey(l => l.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<PurchaseOrder>()
                .HasMany(p => p.Receipts)
                .WithOne(r => r.PurchaseOrder)
                .HasForeignKey(r => r.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<Receipt>()
                .HasMany(r => r.ReceiptLines)
                .WithOne(l => l.Receipt)
                .HasForeignKey(l => l.ReceiptId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Configure indexes
            modelBuilder.Entity<PurchaseOrder>()
                .HasIndex(p => p.Number)
                .IsUnique();
                
            modelBuilder.Entity<PurchaseOrderLine>()
                .HasIndex(l => new { l.PurchaseOrderId, l.Sku })
                .IsUnique();
                
            modelBuilder.Entity<Receipt>()
                .HasIndex(r => r.ReceiptNumber)
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