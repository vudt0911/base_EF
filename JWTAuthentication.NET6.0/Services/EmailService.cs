using JWTAuthentication.NET6._0.Helpter;
using JWTAuthentication.NET6._0.Services.Contracts;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace JWTAuthentication.NET6._0.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings emailSettings;
        public EmailService(IOptions<EmailSettings> options)
        {
            this.emailSettings = options.Value;
        }

        public string GetHtmlcontent(string emailUser)
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
            Response += "<div class=\"title\" style=\"color: #343434; margin-top: 3rem\">[Authentication number information]</div>";
            Response += "<div class=\"content\" style=\"color: #343434; text-align: center\">";
            Response += "Photoism will send you a number for authentication. <br />";
            Response += "Please check the verification number and complete email verification.";
            Response += "</div>";
            Response += "<div class=\"content\" style=\"color: #343434; text-align: center\">";
            Response += "Hello. This is Photoism. <br />";
            Response += "Please enter the verification number below and proceed with membership registration.";
            Response += "</div>";
            Response += "<a href=\"#\">";
            Response += "<button class=\"btn-submit\" style=\"cursor: pointer; background: blue; border-radius: 8px\">";
            Response += "Email : " + emailUser;
            Response += "</button>";
            Response += "</a>";
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


            byte[] fileBytes;
            if (System.IO.File.Exists("Attachment/dummy.pdf"))
            {
                FileStream file = new FileStream("Attachment/dummy.pdf", FileMode.Open, FileAccess.Read);
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }
                builder.Attachments.Add("attachment.pdf", fileBytes, ContentType.Parse("application/octet-stream"));
                builder.Attachments.Add("attachment2.pdf", fileBytes, ContentType.Parse("application/octet-stream"));
            }

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
