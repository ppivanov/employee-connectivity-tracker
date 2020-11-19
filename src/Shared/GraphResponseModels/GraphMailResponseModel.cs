using System;
using System.Collections.Generic;
using System.Text;

namespace EctBlazorApp.Shared.GraphModels
{
    public class GraphMailResponse
    {
        public MicrosoftGraphReceivedMail[] Value { get; set; }
    }

    public class MicrosoftGraphReceivedMail
    {
        public DateTime ReceivedDateTime { get; set; }
        public string Subject { get; set; }
        public MicrosoftGraphPerson Sender { get; set; }
    }
}
