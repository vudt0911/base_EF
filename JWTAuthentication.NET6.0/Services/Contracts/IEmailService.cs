using JWTAuthentication.NET6._0.Helpter;

namespace JWTAuthentication.NET6._0.Services.Contracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(Mailrequest mailrequest);
        public string GetHtmlcontent(string emailUser);
    }
}
