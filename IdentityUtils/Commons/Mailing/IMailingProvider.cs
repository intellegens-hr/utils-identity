using System.Net.Mail;
using System.Threading.Tasks;

namespace Commons.Mailing
{
    public interface IMailingProvider
    {
        Task SendEmailMessage(MailMessageSimple mailMessageSimple);

        Task SendEmailMessage(MailMessage mailMessage);
    }
}