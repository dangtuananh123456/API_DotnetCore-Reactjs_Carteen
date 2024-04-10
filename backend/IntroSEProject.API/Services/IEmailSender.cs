using SendGrid;

namespace IntroSEProject.API.Services
{
    public interface IEmailSender
    {
        Task<Response> SendEmailAsync(string email, string subject, string message);
    }
}
