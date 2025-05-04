using WMS.EventBus.Events;

namespace WMS.EventBus
{
    public interface IEventBus
    {
        Task PublishAsync<T>(T @event) where T : IntegrationEvent;
        
        void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
            
        void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
    }

    public interface IIntegrationEventHandler<in TIntegrationEvent> 
        where TIntegrationEvent : IntegrationEvent
    {
        Task HandleAsync(TIntegrationEvent @event);
    }
}