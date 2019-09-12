using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace FileFinder.Managers.Interface
{
    public interface IEmailManager
    {
        void SendEmail(MailMessage message);
        MailMessage GenerateMailMessage(
           IEnumerable<string> sendersEmail,
           string messageBody,
           string subject = null,
           bool isHtmlBody = false,
           string[] attachments = null);
    }
}
