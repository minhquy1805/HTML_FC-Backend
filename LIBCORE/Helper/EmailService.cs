using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using LIBCORE.Models;
using MimeKit.Text;

namespace LIBCORE.Helper
{
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }

        public async Task SendVerificationEmailAsync(string toEmail, string verificationCode)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "🔐 Xác nhận đăng ký tài khoản - HTML FC";

            // ✅ Body: HTML nội dung đầy đủ, có lời chào, hướng dẫn và chữ ký
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = $@"
            <p>Xin chào,</p>
            <p>Bạn vừa đăng ký tài khoản tại <strong>HTML FC</strong>.</p>
            <p>Đây là mã xác nhận tài khoản của bạn:</p>
            <h2 style='color:blue;'>{verificationCode}</h2>
            <p>Vui lòng nhập mã này vào màn hình xác thực trong vòng <strong>10 phút</strong>.</p>
            <br/>
            <p>Nếu bạn không yêu cầu, vui lòng bỏ qua email này.</p>
            <p>Trân trọng,<br/>Đội ngũ HTML FC</p>
        "
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.FromEmail, _emailSettings.AppPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendResetPasswordEmailAsync(string toEmail, string resetCode)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "🔐 Đặt lại mật khẩu - HTML FC";

            email.Body = new TextPart(TextFormat.Html)
            {
                Text = $@"
                    <p>Xin chào,</p>
                    <p>Bạn đã yêu cầu <strong>đặt lại mật khẩu</strong> cho tài khoản tại <strong>HTML FC</strong>.</p>
                    <p>Đây là mã xác nhận để đặt lại mật khẩu:</p>
                    <h2 style='color:red;'>{resetCode}</h2>
                    <p>Vui lòng nhập mã này vào màn hình đổi mật khẩu trong vòng <strong>10 phút</strong>.</p>
                    <br/>
                    <p>Nếu bạn không yêu cầu, vui lòng bỏ qua email này.</p>
                    <p>Trân trọng,<br/>Đội ngũ HTML FC</p>
                "
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.FromEmail, _emailSettings.AppPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
