using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Commons.Mailing
{
    public class GoogleMailingProvider : IMailingProvider
    {
        public Task SendEmailMessage(MailMessageSimple mailMessageSimple)
            => SendEmailMessage(mailMessageSimple.ToMailMessage());

        public async Task SendEmailMessage(MailMessage mailMessage)
        {
            using var smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential()
                {
                    UserName = "intellegensdemo@gmail.com",
                    Password = "sysgegmqfyvxldyl",
                },
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}