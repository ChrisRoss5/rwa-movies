using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace RwaMovies.Services
{
    public interface IMailService
    {
        Task Send(string receiverMail, string? subject, string body);
    }

    public class MailSettings
    {
        public string DisplayName { get; set; } = null!;
        public string From { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Host { get; set; } = null!;
        public int Port { get; set; }
        public bool UseSSL { get; set; }
        public bool UseStartTls { get; set; }
    }

    public class MailService : IMailService
    {
        private readonly MailSettings _settings;

        public MailService(IOptions<MailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task Send(string receiverMail, string? subject, string body)
        {
            var mail = new MimeMessage();
            var _body = new BodyBuilder { HtmlBody = body };
            mail.From.Add(new MailboxAddress(_settings.DisplayName, _settings.From));
            mail.To.Add(MailboxAddress.Parse(receiverMail));
            mail.Subject = subject ?? "RwaMovies";
            mail.Body = _body.ToMessageBody();
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
            await smtp.SendAsync(mail);
            await smtp.DisconnectAsync(true);
        }
    }
}
