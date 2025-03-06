using System.Net;
using DSEConETL.Models;
using Mandrill;
using Mandrill.Model;
using Microsoft.Extensions.Options;

namespace DSEConETL.Services;

public class EmailService(IOptions<EmailSettings> emailSettings)
{
    private readonly string _apiKey = "md-w2D7YBVXFOn653Wp02bAOw";

    public void SendEmail(string fromEmail, string fromName, string toEmail, string subject, string htmlContent)
    {
        try
        {
            var api = new MandrillApi(_apiKey);

            var recipients = new List<MandrillMailAddress>
            {
                new MandrillMailAddress { Email = toEmail }
            };

            var emailMessage = new MandrillMessage
            {
                FromEmail = fromEmail,
                FromName = fromName,
                Subject = subject,
                Html = WebUtility.HtmlEncode(htmlContent),
                To = recipients
            };

            var result = Task.Run(async () => await api.Messages.SendAsync(emailMessage)).Result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while sending the email: {ex.Message}");
            throw;
        }
    }

    public void SendEmailWithAttachment(string fromEmail, string fromName, string? toEmail, string subject, string htmlContent, string attachmentFilePath)
    {
        try
        {
            var api = new MandrillApi(_apiKey);

            toEmail ??= emailSettings.Value.DefaultToEmail;

            var recipients = new List<MandrillMailAddress>
            {
                new() { Email = toEmail }
            };

            var emailMessage = new MandrillMessage
            {
                FromEmail = fromEmail,
                FromName = fromName,
                Subject = subject,
                Html = htmlContent,
                To = recipients,
                Attachments = new List<MandrillAttachment>()
            };

            if (File.Exists(attachmentFilePath))
            {
                var content = File.ReadAllBytes(attachmentFilePath);
                var name = Path.GetFileName(attachmentFilePath);
                var type = GetMimeType(attachmentFilePath);

                var attachment = new MandrillAttachment { Content = content, Name = name, Type = type };

                emailMessage.Attachments.Add(attachment);
            }
            else
            {
                Console.WriteLine("Attachment file does not exist.");
            }

            var result = Task.Run(async () => await api.Messages.SendAsync(emailMessage)).Result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while sending the email with attachment: {ex.Message}");
            throw;
        }
    }

    private string GetMimeType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLower();
        return extension switch
        {
            ".csv" => "text/csv",
            ".xml" => "application/xml",
            _ => "application/octet-stream"
        };
    }
}