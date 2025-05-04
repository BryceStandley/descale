using WMS.SharedKernel.Interfaces;

namespace WMS.SharedKernel.Entities
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }
            
        protected Entity()
        {
            Id = Guid.NewGuid();
        }
            
        // Domain event handling
        private List<IDomainEvent> _domainEvents;
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();
            
        public void AddDomainEvent(IDomainEvent eventItem)
        {
            _domainEvents ??= new List<IDomainEvent>();
            _domainEvents.Add(eventItem);
        }
            
        public void RemoveDomainEvent(IDomainEvent eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }
            
        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }
    }
}