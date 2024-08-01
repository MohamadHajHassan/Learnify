using System.Net.Mail;

namespace Learnify_backend.Services.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(
            string fromAddress,
            string toAddress,
            string subject,
            string message,
            List<Attachment> attachments = null);
    }
}
