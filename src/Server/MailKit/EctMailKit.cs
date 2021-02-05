using EctBlazorApp.Shared.Entities;
using MailKit.Net.Smtp;
using MimeKit;
using System;

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
            EmailMessage message = new EmailMessage();
            message.Sender = new MailboxAddress("Employee Connectivity Tracker", Sender);
            message.Reciever = new MailboxAddress(user.FullName, user.Email);
            message.Subject = "Welcome to ECT!";
            message.Content = "Welcome to Employee Connectivity Tracker!";
            var mimeMessage = message.CreateMimeMessage();
            try
            {
                using SmtpClient smtpClient = new SmtpClient();
                smtpClient.Connect(SmtpServer, Port, true);
                smtpClient.Authenticate(UserName, Password);
                smtpClient.Send(mimeMessage);
                smtpClient.Disconnect(true);
            }
            catch (Exception ex)
            {
                return false;                                                                       // todo: log the exception
            }

            return true;
        }

    }
}
