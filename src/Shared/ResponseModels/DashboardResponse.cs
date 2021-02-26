using EctBlazorApp.Shared.Entities;
using System.Collections.Generic;

namespace EctBlazorApp.Shared
{
    public class DashboardResponse
    {
        public List<CalendarEvent> CalendarEvents { get; set; }
        public List<ReceivedMail> ReceivedMail { get; set; }
        public List<SentMail> SentMail { get; set; }
        public double SecondsInMeeting { get; set; }
        public string UserFullName { get; set; }
    }
}
