using AdminTemplate.Models.Email;

namespace AdminTemplate.Services.Email
{
    public interface IEmailService
    {
        Task SendMailAsync(MailModel model);
    }
}
