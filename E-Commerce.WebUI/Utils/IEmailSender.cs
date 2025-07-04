namespace E_Commerce.WebUI.Utils
{
    public interface ICustomEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}
