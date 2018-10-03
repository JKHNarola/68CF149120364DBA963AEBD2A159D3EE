﻿using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace App.BL.Services
{
    public class EmailService
    {
        private EmailSettings _settings;
        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }
        public EmailService(EmailSettings settings)
        {
            _settings = settings;
        }

        public async Task SendMailAsync(List<(string email, string displayName)> toAddrs, List<(string email, string displayName)> ccAddrs, List<(string email, string displayName)> bccAddrs, string subject, string bodyText, List<string> attachmentFilepaths)
        {
            var mailServerIp = _settings.MailServerIp;
            var systemEmailSenderEmail = _settings.SystemEmailSenderEmail;
            var systemEmailSenderUsername = _settings.SystemEmailSenderUsername;
            var systemEmailSenderPassword = _settings.SystemEmailSenderPassword;
            var systemEmailSenderName = _settings.SystemEmailSenderName;
            var port = _settings.OutPortNo;

            var objMail = new MailMessage();

            if (toAddrs == null || toAddrs.Count == 0)
                throw new Exception("No to email found to send email");

            foreach (var (email, displayName) in toAddrs)
                objMail.To.Add(new MailAddress(email, displayName));

            if (ccAddrs != null && ccAddrs.Count > 0)
                foreach (var (email, displayName) in ccAddrs)
                    objMail.CC.Add(new MailAddress(email, displayName));

            if (bccAddrs != null && bccAddrs.Count > 0)
                foreach (var (email, displayName) in bccAddrs)
                    objMail.Bcc.Add(new MailAddress(email, displayName));

            objMail.From = new MailAddress(systemEmailSenderEmail, systemEmailSenderName);

            objMail.IsBodyHtml = true;

            objMail.Priority = MailPriority.Normal;

            if (attachmentFilepaths != null && attachmentFilepaths.Count > 0)
                foreach (var path in attachmentFilepaths)
                    objMail.Attachments.Add(new Attachment(path));

            objMail.Subject = subject;

            objMail.Body = bodyText;

            var objSmtpClient = new SmtpClient(mailServerIp, port)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(systemEmailSenderUsername,
                    systemEmailSenderPassword)
            };

            objMail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;

            await objSmtpClient.SendMailAsync(objMail);
        }

        public void SendMails(List<string> emailids, string subject, string bodyText)
        {
            Task.Run(() =>
            {
                Parallel.ForEach(emailids, async emailid =>
                {
                    await SendMailAsync(new List<(string email, string displayName)>() { (emailid, "") }, null, null, subject, bodyText, null);
                });
            });
        }
    }

    public class EmailSettings
    {
        public string MailServerIp { get; set; }
        public string SystemEmailSenderEmail { get; set; }
        public string SystemEmailSenderUsername { get; set; }
        public string SystemEmailSenderPassword { get; set; }
        public string SystemEmailSenderName { get; set; }
        public int OutPortNo { get; set; }
    }
}
