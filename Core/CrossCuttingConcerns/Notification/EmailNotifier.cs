using Core.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Notification
{
    public class EmailNotifier : INotifier<EmailMessage>
    {
        public void Notify(EmailMessage data)
        {
            using (var eventBus = new EventBusRabbitMQ())
            {
                eventBus.Publish(data);
            }
        }
    }
}
