using ComChienMaDui.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ComChienMaDui.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    // Gửi email bất đồng bộ
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var email = new MimeMessage();

        // Thiết lập địa chỉ người gửi
        email.From.Add(
            new MailboxAddress(
                _settings.DisplayName,
                _settings.From
            )
        );

        // Thiết lập địa chỉ người nhận
        email.To.Add(MailboxAddress.Parse(toEmail));

        // Thiết lập tiêu đề email
        email.Subject = subject;

        // Thiết lập nội dung email (dạng HTML)
        email.Body = new TextPart("html")
        {
            Text = body
        };

        // Sử dụng SmtpClient để gửi email
        using var smtp = new SmtpClient();

        // Kết nối đến máy chủ SMTP với tùy chọn bảo mật StartTLS
        await smtp.ConnectAsync(
            _settings.Host,
            _settings.Port,
            SecureSocketOptions.StartTls
        );

        // Xác thực với máy chủ SMTP
        await smtp.AuthenticateAsync(
            _settings.Username,
            _settings.Password
        );

        // Gửi email
        await smtp.SendAsync(email);

        // Ngắt kết nối sau khi gửi
        await smtp.DisconnectAsync(true);
    }
}