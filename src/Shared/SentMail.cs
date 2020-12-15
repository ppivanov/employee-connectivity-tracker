using EctBlazorApp.Shared.GraphModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EctBlazorApp.Shared
{
    public class SentMail : Mail
    {
        public DateTime SentAt { get; set; }
        public List<string> Recipients { get; set; }

        public string RecipientsAsString 
        { 
            get
            {
                return string.Join("|", Recipients);
            }
            set
            {
                Recipients = value.Split("|").ToList();
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
