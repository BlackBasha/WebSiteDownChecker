using Core.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Notification
{
   public class EmailMessage: IntegrationEvent
    {
        public string FromAddress { get; set; }
        public List<string> ToAdresses { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public EmailMessage()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.Now;
        }
    }
}
