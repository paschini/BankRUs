using BankRUs.Application.Services;

namespace BankRUs.Intrastructure.Services;

public class EmailSender : IEmailSender
{
    public async Task SendEmailAsync(string to, string from, string subject, string body)
    {
        // Skicka riktiga email...
    }
}
