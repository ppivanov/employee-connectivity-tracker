using EctBlazorApp.Shared.Entities;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using static EctBlazorApp.Shared.SharedMethods;

namespace EctBlazorApp.Server.MailKit
{
    public class EctMailKit
    {
        public string Sender { get; set; }
        public string Reciever { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        
        public bool SendWelcome(EctUser user)
        {
            string subject = "Welcome to ECT!";
            string messageContent = "Welcome to Employee Connectivity Tracker!";
            bool sentSuccessfully = SendMessage(user, subject, messageContent);

            return sentSuccessfully;
        }

        public bool SendNotificationEmail(string teamName, string recipient, string messageContent)
        {
            string subject = $"ECT: Isolated teammates ({teamName})";
            bool sentSuccessfully = SendMessage(recipient, subject, messageContent);

            return sentSuccessfully;
        }

        private bool SendMessage(EctUser recipient, string subject, string messageContent)
        {
            return SendMessage(recipient.FullName, recipient.Email, subject, messageContent);
        }

        private bool SendMessage(string recipient, string subject, string messageContent)                   // Expects the recipient string to be formatted - firstname lastname <email@example.com>
        {
            var name = GetFullNameFromFormattedString(recipient);
            var email = GetEmailFromFormattedString(recipient);
            return SendMessage(name, email, subject, messageContent);
        }

        private bool SendMessage(string recipientName, string recipientEmail, string subject, string messageContent)
        {
            EmailMessage message = new EmailMessage
            {
                Sender = new MailboxAddress("Employee Connectivity Tracker", Sender),
                Reciever = new MailboxAddress(recipientName, recipientEmail),
                Subject = subject,
                Content = messageContent
            };
            var mimeMessage = message.CreateMimeMessage();
            try
            {
                using SmtpClient smtpClient = new SmtpClient();
                smtpClient.Connect(SmtpServer, Port, true);
                smtpClient.Authenticate(UserName, Password);
                smtpClient.Send(mimeMessage);
                smtpClient.Disconnect(true);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
