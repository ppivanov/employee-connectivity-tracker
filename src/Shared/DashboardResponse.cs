using System;
using System.Collections.Generic;
using System.Text;

namespace EctBlazorApp.Shared
{
    public class DashboardResponse
    {
        public List<CalendarEvent> CalendarEvents { get; set; }
        public List<ReceivedMail> ReceivedMail { get; set; }
        public List<SentMail> SentMail { get; set; }
    }
}
