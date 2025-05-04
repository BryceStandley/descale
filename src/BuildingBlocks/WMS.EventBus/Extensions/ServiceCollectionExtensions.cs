using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using WMS.EventBus.Implementations;

namespace WMS.EventBus.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventBus(
            this IServiceCollection services, 
            string connectionString, 
            string exchangeName,
            string queueName, 
            int retryCount = 5)
        {
            // Add the event bus subscriptions manager
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            // Add the RabbitMQ connection
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    Uri = new Uri(connectionString),
                    DispatchConsumersAsync = true
                };

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });

            // Add the RabbitMQ event bus
            services.AddSingleton<IEventBus, RabbitMQEventBus>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var logger = sp.GetRequiredService<ILogger<RabbitMQEventBus>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                return new RabbitMQEventBus(
                    rabbitMQPersistentConnection,
                    logger,
                    sp,
                    eventBusSubcriptionsManager,
                    exchangeName,
                    queueName,
                    retryCount);
            });

            return services;
        }
    }
}