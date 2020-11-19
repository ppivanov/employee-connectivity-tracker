using EctBlazorApp.Shared.GraphModels;
using System;
using System.Collections.Generic;

namespace EctBlazorApp.Shared
{
    public class ReceivedMail : Mail
    {
        public string From { get; set; }
        public DateTime ReceivedAt { get; set; }

        public ReceivedMail() : base()
        {
        }

        public ReceivedMail(MicrosoftGraphReceivedMail graphMail)
        {
            From = graphMail.Sender.ToString();
            ReceivedAt = graphMail.ReceivedDateTime;
            Subject = graphMail.Subject;
        }

        public static List<ReceivedMail> CastGraphReceivedMailToReceivedMail(MicrosoftGraphReceivedMail[] graphReceivedMail)
        {
            List<ReceivedMail> receivedMailList = new List<ReceivedMail>();
            foreach (var graphMail in graphReceivedMail)
            {
                ReceivedMail receivedMail = new ReceivedMail(graphMail);
                receivedMailList.Add(receivedMail);
            }

            return receivedMailList;
        }
    }
}
