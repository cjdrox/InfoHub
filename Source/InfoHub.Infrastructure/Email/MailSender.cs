using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace InfoHub.Infrastructure.Email
{
    public class MailSender
    {
        private readonly SmtpClient _client;

        public MailSender(SmtpClient client)
        {
            _client = client;
        }

        public MailSender(string host, int port, SmtpDeliveryMethod method, string userName, string password, bool useSSL = false)
        {
            _client = new SmtpClient(host, port)
                          {
                              DeliveryMethod = method,
                              UseDefaultCredentials = false,
                              Credentials = new NetworkCredential(userName, password),
                              EnableSsl = useSSL
                          };
        }

        public void Send(MailMessage message)
        {
            _client.Send(message);
        }

        public void Send(IList<MailMessage> messages)
        {
            foreach (var message in messages)
            {
                _client.Send(message);
            }
        }
    }
}
