using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Graph;
using System.Text;

namespace EctBlazorApp.Shared
{
    public class CalendarEvent
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        // Importance??
        public string Subject { get; set; }
        public string Organizer { get; set; }


        private List<String> _attendees;
        public List<String> Attendees
        {
            get { return _attendees; }
            set { _attendees = value; }
        }

        public string AttendeesAsString
        {
            get
            {
                return string.Join(" | ", _attendees);
            }
            set
            {
                _attendees = value.Split(" | ").ToList();
            }
        }

        public CalendarEvent()
        {

        }

        public CalendarEvent(Event graphEvent)
        {
            Attendees = new List<string>();
            foreach (Attendee attendee in graphEvent.Attendees)
            {
                Attendees.Add($"{attendee.EmailAddress.Name} <{attendee.EmailAddress.Address}>");
            }
            End = DateTime.Parse(graphEvent.End.DateTime);
            Organizer = graphEvent.Organizer.EmailAddress.Name;
            Start = DateTime.Parse(graphEvent.Start.DateTime);
            Subject = graphEvent.Subject;
        }
    }
}
