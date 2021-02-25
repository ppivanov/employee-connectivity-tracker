using EctBlazorApp.Shared.GraphModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EctBlazorApp.Shared.Entities
{
    public class SentMail : Mail
    {
        [Required]
        public DateTime SentAt { get; set; }
        public List<string> Recipients { get; set; }

        public string RecipientsAsString 
        { 
            get
            {
                if (Recipients.Count < 1)
                    return "";

                return string.Join("|", Recipients);
            }
            set
            {
                if (value.Length < 1)
                    Recipients = new List<string>();
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
