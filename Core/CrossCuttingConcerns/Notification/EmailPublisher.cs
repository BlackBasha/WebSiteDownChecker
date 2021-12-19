using Core.EventBus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Notification
{
   public class EmailPublisher
    {
        private ILogger _logger;
        public EmailPublisher( ILogger logger)
        {
            _logger = logger;
        }

        public void SendEmail(Message emailMessage)
        {
            try
            {
                using (var eventBus = new EventBusRabbitMQ())
                {
                    eventBus.Publish(emailMessage);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in sending Mail to queue", ex);
            }
        }
    }
}
