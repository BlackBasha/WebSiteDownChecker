using Core.EventBus;
using Core.Notification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.HostServices
{
    public class ConsumeRabbitMqEmailHostService : IHostedService
    {
        private IConfiguration _configuration;
        private ILogger _logger;
        public IIntegrationEventHandler<Message> emailMessageEvent { get; set; }

        public ConsumeRabbitMqEmailHostService(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            IEventBus eventBus = new EventBusRabbitMQ();
            emailMessageEvent = new EmailNotifierHandler();
            eventBus.Subscribe<Message, EmailNotifierHandler>();
            return Task.FromResult(emailMessageEvent);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
       
    }
}
