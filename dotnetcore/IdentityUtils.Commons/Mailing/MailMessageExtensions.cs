using System.Net.Mail;

namespace IdentityUtils.Commons.Mailing
{
    public static class MailMessageExtensions
    {
        public static MailMessage ToMailMessage(this MailMessageSimple messageSimple)
        {
            MailMessage message = new MailMessage
            {
                From = new MailAddress(messageSimple.From),
                Subject = messageSimple.Subject,
                IsBodyHtml = messageSimple.EmailType == EmailTypes.HTML,
                Body = messageSimple.Content
            };

            messageSimple.To.ForEach(x => message.To.Add(x));

            return message;
        }
    }
}