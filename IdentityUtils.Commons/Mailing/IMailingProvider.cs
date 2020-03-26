using System.Net.Mail;
using System.Threading.Tasks;

namespace IdentityUtils.Commons.Mailing
{
    public interface IMailingProvider
    {
        Task SendEmailMessage(MailMessageSimple mailMessageSimple);

        Task SendEmailMessage(MailMessage mailMessage);
    }
}