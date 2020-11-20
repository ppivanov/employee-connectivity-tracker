using System;
using System.Collections.Generic;
using System.Text;

namespace EctBlazorApp.Shared.GraphModels
{
    public class GraphReceivedMailResponse
    {
        public MicrosoftGraphReceivedMail[] Value { get; set; }
    }

    public class MicrosoftGraphReceivedMail
    {
        public DateTime ReceivedDateTime { get; set; }
        public string Subject { get; set; }
        public MicrosoftGraphPerson Sender { get; set; }
    }

    public class GraphSentMailResponse
    {
        public MicrosoftGraphSentMail[] Value { get; set; }
    }

    public class MicrosoftGraphSentMail
    {
        public DateTime SentDateTime { get; set; }
        public string Subject { get; set; }
        public MicrosoftGraphPerson[] ToRecipients { get; set; }
    }
}
