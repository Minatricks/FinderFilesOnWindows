using FileFinder.Managers.Interface;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace FileFinder
{
    public class EmailManager : IEmailManager
    {
        public string SenderMail { get; }
        public string SenderPassword { get; }
        public MailMessage CurrentMessage { get; private set; }
        public EmailManager(string senderEmail, string senderPassword)
        {
            this.SenderMail = senderEmail;
            this.SenderPassword = senderPassword;
        }

        public void SendEmail(MailMessage message)
        {
            try
            {
                TrySendEmail(message, _smptCodes.Pop());
            }
            catch (Exception ex)
            {
                if(_smptCodes.Count == 0)
                {
                    throw;
                }

                TrySendEmail(message, _smptCodes.Pop());
            }
        }
        public MailMessage GenerateMailMessage(
            IEnumerable<string> sendersEmail,
            string messageBody,
            string subject = null,
            bool isHtmlBody = false,
            string[] attachments = null)
        {
            var message = new MailMessage()
            {
                From = new MailAddress(this.SenderMail),
                Body = messageBody
            };

            if (subject != null) message.Subject = subject;
            if (isHtmlBody) message.IsBodyHtml = isHtmlBody;
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    message.Attachments.Add(new Attachment(attachment));
                }
            }

            foreach (var senderEmail in sendersEmail)
            {
                if (string.IsNullOrEmpty(senderEmail) || !senderEmail.Contains("@") || !senderEmail.Contains("."))
                {
                    throw new ArgumentException("Email should be really");
                }
            }

            this.CurrentMessage = message;
            return message;
        }

        private Stack<int> _smptCodes = new Stack<int>(new[] { 25, 587, 465 });
        private void TrySendEmail(MailMessage message, int codeForConfigureSmtpServer)
        {
            var smtpServer = ConfigureSmptServer(codeForConfigureSmtpServer);
            smtpServer.Send(message);
        }

        private SmtpClient ConfigureSmptServer(int codeForConfigureSmtpServer)
        {
            return new SmtpClient("smtp.gmail.com")
            {
                Port = codeForConfigureSmtpServer,
                Credentials = new NetworkCredential(this.SenderMail, this.SenderPassword),
                EnableSsl = true
            };
        }
    }
}
