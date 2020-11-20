using EctBlazorApp.Shared.GraphModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EctBlazorApp.Shared
{
    public class SentMail : Mail
    {
        public DateTime SentAt { get; set; }

        private List<string> _recipients;

        public List<string> Recipients
        {
            get { return _recipients; }
            set { _recipients = value; }
        }

        public string RecipientsAsString 
        { 
            get
            {
                return string.Join("|", _recipients);
            }
            set
            {
                _recipients = value.Split("|").ToList();
            }
        }

        public SentMail()
        {
        }

        public SentMail(MicrosoftGraphSentMail graphMail)
        {
            SentAt = graphMail.SentDateTime;
            Subject = graphMail.Subject;
            Recipients = new List<string>();

            foreach (var recipient in graphMail.ToRecipients)
            {
                Recipients.Add(recipient.ToString());
            }
        }

        public static List<SentMail> CastGraphSentMailToSentMail(MicrosoftGraphSentMail[] graphSentMail)
        {
            List<SentMail> sentMail = new List<SentMail>();
            foreach (var graphMail in graphSentMail)
            {
                sentMail.Add(new SentMail(graphMail));
            }

            return sentMail;
        }
    }
}
