using Core.EventBus;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Notification
{
    public class EmailNotifierHandler :  IIntegrationEventHandler<Message>
    {
       
        public EmailNotifierHandler()
        {
           
        }
        public async Task Handle(Message @event)
        {
            try
            {
                // create email message
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(@event.FromAddress));
                foreach (var item in @event.ToAdresses)
                {
                    email.To.Add(MailboxAddress.Parse(item));
                }
                
                email.Subject = @event.Subject;
                email.Body = new TextPart(TextFormat.Text) { Text = @event.Body };

                // send email
                using var smtp = new SmtpClient();
                smtp.Connect("SMTPddress", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("UserName", "Password");
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {
               // _logger.LogError("Error in sending Mail", ex);
            }
        }

      
    }
}
