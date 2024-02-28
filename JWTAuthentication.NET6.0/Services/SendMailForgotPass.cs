using JWTAuthentication.NET6._0.Helpter;
using JWTAuthentication.NET6._0.Services.Contracts;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace JWTAuthentication.NET6._0.Services
{
    public class SendMailForgotPass : IEmailService
    {
        private readonly EmailSettings emailSettings;
        public SendMailForgotPass(IOptions<EmailSettings> options)
        {
            this.emailSettings = options.Value;
        }

        public string GetHtmlcontent(string clientUrl)
        {
            string Response = "<html lang=\"en\">";
            Response += "<head>";
            Response += "<meta charset=\"UTF-8\" />";
            Response += "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />";
            Response += "<title>Email Verification</title>";
            Response += "<style>";
            Response += "body { margin: 0 auto; }";
            Response += ".wrapper { border: 1px solid #dfdfdf; margin: 0 auto; max-width: 520px; border-radius: 8px; padding: 40px 91px; margin-top: 36px; }";
            Response += "@media only screen and (max-width: 520px) { .wrapper { padding: 21px; } }";
            Response += ".container { max-width: 520px; margin: 0 auto; }";
            Response += ".logo { margin: 0 auto; width: 177px; height: 43px; }";
            Response += ".thumb { overflow: hidden; }";
            Response += ".thumb img { width: 100%; height: 100%; object-fit: fill; }";
            Response += "</style>";
            Response += "</head>";
            Response += "<body>";
            Response += "<div class=\"logo thumb\">";
            Response += "<img src=\"https://sales-homepage.s3.ap-northeast-2.amazonaws.com/logo-photoism.jpg\" alt=\"logo\" />";
            Response += "</div>";
            Response += "<div class=\"wrapper\">";
            Response += "<div class=\"container\">";
            Response += "<div class=\"title\" style=\"color: #343434; margin-top: 3rem; text-align: center\">[Verify email to change password]</div>";
            Response += "<div class=\"content\" style=\"color: #343434; text-align: center\">";
            Response += "C# will send you a email for authentication. <br />";
            Response += "Please click the confirm password change button to change your password.";
            Response += "</div>";
            Response += "<div class=\"content\" style=\"color: #343434; text-align: center\">";
            Response += "Hello. This is Photoism. <br />";
            Response += "Please click the verification button below.";
            Response += "</div>";
            Response += "<div style=\"text-align: center\">";
            Response += "<a href=" + clientUrl + "> ";
            Response += "<button class=\"btn-submit\" style=\"cursor: pointer; background: while; border-radius: 8px\">";
            Response += "Please click here for change password";
            Response += "</button>";
            Response += "</a>";
            Response += "</div>";
            Response += "<div class=\"content\" style=\"color: #343434; text-align: center\">";
            Response += "Thank you.";
            Response += "</div>";
            Response += "</div>";
            Response += "</div>";
            Response += "</body>";
            Response += "</html>";
            return Response;
        }

        public async Task SendEmailAsync(Mailrequest mailrequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emailSettings.Email);
            email.To.Add(MailboxAddress.Parse(mailrequest.ToEmail));
            email.Subject = mailrequest.Subject;
            var builder = new BodyBuilder();


            builder.HtmlBody = mailrequest.Body;
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(emailSettings.Email, emailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
