using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Graph;
using System.Text;
using EctBlazorApp.Shared.GraphModels;

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

        public CalendarEvent(MicrosoftGraphEvent graphEvent)
        {
            const int attendeesLimit = 20;

            Subject = graphEvent.Subject;
            Start = graphEvent.Start.ConvertToLocalDateTime();
            End = graphEvent.End.ConvertToLocalDateTime();
            Organizer = graphEvent.Organizer.ToString();
            Attendees = new List<string>();
            // TODO -> find a solution or remove the limit???
            Attendees.AddRange(
                graphEvent.Attendees.Take(attendeesLimit)
                    .Select(a => a.ToString()));
        }

        public static List<CalendarEvent> CastGraphEventsToCalendarEvents(MicrosoftGraphEvent[] graphEvents)
        {
            List<CalendarEvent> calendarEvents = new List<CalendarEvent>();
            foreach (var graphEvent in graphEvents)
            {
                CalendarEvent calendarEvent = new CalendarEvent(graphEvent);
                calendarEvents.Add(calendarEvent);
            }

            return calendarEvents;
        }
    }
}
