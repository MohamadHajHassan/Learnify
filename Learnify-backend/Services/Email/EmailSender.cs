using System.Net;
using System.Net.Mail;

namespace Learnify_backend.Services.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(
            string fromAddress,
            string toAddress,
            string subject,
            string message,
            List<Attachment> attachments = null)
        {
            var MailMessage = new MailMessage(fromAddress, toAddress, subject, message);
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    MailMessage.Attachments.Add(attachment);
                }
            }
            using (var client = new SmtpClient(_configuration["SMTP:Host"], int.Parse(_configuration["SMTP:Port"])))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_configuration["SMTP:Username"], _configuration["SMTP:Password"]);
                client.EnableSsl = true;

                await client.SendMailAsync(MailMessage);
            }
        }
    }
}
