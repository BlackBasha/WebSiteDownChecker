using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Core.EventBus
{
    public class EventBusRabbitMQ : IEventBus
    {
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly int _retryCount = 0;
        public EventBusRabbitMQ(bool isLogQueue = false)
        {
            _persistentConnection = new DefaultRabbitMQPersistentConnection(isLogQueue);
            _retryCount = 5;
        }

        public void Dispose()
        {
            _persistentConnection.Dispose();
        }

        public void Publish<T>(T @event)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
               .Or<SocketException>()
               .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
               {
                   // _logger.LogWarning(ex.ToString());
               });



            using (var channel = _persistentConnection.CreateModel())
            {
                var queueName = @event.GetType().Name;

                channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);


                var message = JsonConvert.SerializeObject(@event, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

                var body = Encoding.UTF8.GetBytes(message);

                policy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2; // persistent

                    channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);

                });
            }

        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            string queueName = typeof(T).Name;

            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();

            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);


            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);


                var handlerType = typeof(TH);

                var eventType = typeof(T);

                var integrationEvent = JsonConvert.DeserializeObject(message, eventType, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

                var task = Activator.CreateInstance(handlerType) as IIntegrationEventHandler<T>;

                await task.Handle((T)integrationEvent);

                channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: queueName,
                autoAck: false,
                consumer: consumer);
        }






        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }


    }
}
