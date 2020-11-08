using System;
using System.Collections.Generic;
using System.Text;

namespace EctBlazorApp.Shared
{
    public class ReceivedMail : Mail
    {
        public string From { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}
