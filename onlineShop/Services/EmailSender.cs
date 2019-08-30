using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace onlineShop.Services
{
    public class EmailSender : IEmailSender
    {

        private readonly string _sendGridApiKey;
        private readonly string _senderAddress;
        private readonly string _senderName;

        public EmailSender(IConfiguration config)
        {
            var configSection = config.GetSection("SendGrid");

            _sendGridApiKey = configSection["APIKey"];
            _senderAddress = configSection["SenderAddress"];
            _senderName = configSection["SenderName"];
        }
        public Task SendEmailAsync(string recipient, string subject, string htmlMessage)
        {
            return Execute(recipient, subject, htmlMessage);
        }

        private Task Execute(string recipient, string subject, string htmlMessage)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var msg = new SendGridMessage
            {
                From = new EmailAddress(_senderAddress, _senderName),
                Subject = subject,
                HtmlContent = htmlMessage,
                PlainTextContent = htmlMessage,
            };

            msg.AddTo(new EmailAddress(recipient));
            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
    }
}
