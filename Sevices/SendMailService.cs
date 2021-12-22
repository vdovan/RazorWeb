using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
namespace RazorWeb.Services
{

    public class SendMailService : IEmailSender
    {
        private ILogger _logger;
        private MailConfig _config;
        public SendMailService(IOptions<MailConfig> config, ILogger<SendMailService> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string mail, string subject, string htmlMessage)
        {
             MimeMessage email = new MimeMessage();
            // Email from
            email.From.Add(new MailboxAddress(_config.DisplayName, _config.Mail));

            // Mail to
            email.To.Add(new MailboxAddress(mail, mail));
            //Subject
            email.Subject = subject;

            //Body
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = htmlMessage;

            email.Body = bodyBuilder.ToMessageBody();

            //SMTP
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                await smtp.ConnectAsync(_config.SmtpHost, _config.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_config.Mail, _config.Pwd);

                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                System.IO.Directory.CreateDirectory ("mailssave");
                var emailsavefile = string.Format (@"mailssave/{0}.eml", Guid.NewGuid ());
                await email.WriteToAsync (emailsavefile);

                _logger.LogInformation ("Lỗi gửi mail, lưu tại - " + emailsavefile);
                _logger.LogError (ex.Message);
            }

        }

        public async Task<string> SendEmailAsync(MailContent content)
        {

            MimeMessage email = new MimeMessage();
            // Email from
            email.From.Add(new MailboxAddress(_config.DisplayName, _config.Mail));

            // Mail to
            email.To.Add(new MailboxAddress(content.To, content.To));
            //Subject
            email.Subject = content.Subject;

            //Body
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = content.Body;
            if (content.Attachments != null)
            {
                 for (var i = 0; i < content.Attachments.Length; i++)
                 {
                    byte[] data = File.ReadAllBytes(content.Attachments[i]);
                    bodyBuilder.Attachments.Add(Path.GetFileName(content.Attachments[i]), data);
                 }
                 
                // var multipart = new Multipart("mixed");
                // multipart.Add(bodyContent);

                // foreach (MimePart item in content.Attachments)
                // {
                //     using FileStream fs = File.OpenRead(item.FileName);
                //     item.Content = new MimeContent(fs, ContentEncoding.Default);
                //     item.ContentDisposition = new ContentDisposition(ContentDisposition.Attachment);
                //     item.ContentTransferEncoding = ContentEncoding.Base64;
                //     item.FileName = Path.GetFileName(item.FileName);
                //     multipart.Add(item);
                // }

                // email.Body = multipart;
            }
            email.Body = bodyBuilder.ToMessageBody();

            //SMTP
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                await smtp.ConnectAsync(_config.SmtpHost, _config.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_config.Mail, _config.Pwd);

                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                return "Send email false: " + ex.Message;
            }

            return "SEND MAIL SUCCESSFUL" + DateTime.Now;
        }
    }
}