using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace IdentityUtils.Commons.Mailing.Google
{
    public class GoogleMailingProvider : IMailingProvider
    {
        private readonly IGoogleMailingProviderConfig mailingProviderConfig;

        private const string GOOGLE_MAIL_HOST = "smtp.gmail.com";
        private const short GOOGLE_MAIL_PORT = 587;

        public GoogleMailingProvider(IGoogleMailingProviderConfig mailingProviderConfig)
        {
            this.mailingProviderConfig = mailingProviderConfig;
        }

        public Task SendEmailMessage(MailMessageSimple mailMessageSimple)
            => SendEmailMessage(mailMessageSimple.ToMailMessage());

        public async Task SendEmailMessage(MailMessage mailMessage)
        {
            using var smtpClient = new SmtpClient(GOOGLE_MAIL_HOST, GOOGLE_MAIL_PORT)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential()
                {
                    UserName = mailingProviderConfig.Username,
                    Password = mailingProviderConfig.Password,
                },
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}