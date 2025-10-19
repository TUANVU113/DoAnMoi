using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Threading.Tasks;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;

    public EmailService(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using (var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
        {
            client.Credentials = new System.Net.NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Username),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
        }
    }
}