using MimeKit;

namespace EctBlazorApp.Server.MailKit
{
    public class EmailMessage
    {
        public MailboxAddress Sender { get; set; }
        public MailboxAddress Reciever { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public MimeMessage CreateMimeMessage()
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(Sender);
            mimeMessage.To.Add(Reciever);
            mimeMessage.Subject = Subject;
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text){ Text = Content };
            return mimeMessage;
        }
    }
}
