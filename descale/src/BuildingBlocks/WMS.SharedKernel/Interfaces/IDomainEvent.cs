namespace WMS.SharedKernel.Interfaces
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}