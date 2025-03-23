using DSEMachshipETL.Models;
using Microsoft.Extensions.Options;

namespace DSEMachshipETL.Services;

public class Logger(IOptions<EmailSettings> emailSettings, EmailService emailService)
{
    private readonly string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");

    public void LogError(string message)
    {
        LogText(message);
        EmailInfo(message, "system@dsetrucks.com.au", "ETL", emailSettings.Value.ErrorEmail, "ETL Error");
    }

    private void LogText(string message)
    {
        try
        {
            // Ensure the log directory exists
            Directory.CreateDirectory(logDirectory);

            // Create the log file name with the current date
            var logFileName = $"{DateTime.Now:yyyy-MM-dd}_error.log";
            var logFilePath = Path.Combine(logDirectory, logFileName);

            using var writer = new StreamWriter(logFilePath, true);
            writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to log error: {ex.Message}");
        }
    }

    private void EmailInfo(string message, string fromEmail, string fromName, string toEmail, string subject)
    {
        try
        {
            emailService.SendEmail(fromEmail, fromName, toEmail, subject, message);
        }
        catch (Exception ex)
        {
            LogText($"Failed to Email Error: {ex.Message}");
        }
    }

}