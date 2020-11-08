using System;
using System.Collections.Generic;
using System.Text;

namespace EctBlazorApp.Shared
{
    public class EctUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime LastSignIn { get; set; }

        public ICollection<CalendarEvent> CalendarEvents { get; set; }
        public ICollection<EctTeam> LeaderOf { get; set; }
        public EctTeam MemberOf { get; set; }
        public ICollection<ReceivedMail> ReceivedEmails { get; set; }
        public ICollection<SentMail> SentEmails { get; set; }
        
    }
}
