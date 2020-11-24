using EctBlazorApp.Shared.GraphModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EctBlazorApp.Shared
{
    public class CalendarEvent
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Subject { get; set; }
        public string Organizer { get; set; }

        public int EctUserId { get; set; }

        private List<string> _attendees;
        public List<string> Attendees
        {
            get { return _attendees; }
            set { _attendees = value; }
        }

        public string AttendeesAsString
        {
            get
            {
                if (Attendees.Count < 1) 
                    return "";

                return string.Join(" | ", Attendees);
            }
            set
            {
                if (value.Length < 1) 
                    Attendees = new List<string>();

                Attendees = value.Split(" | ").ToList();
            }
        }

        public CalendarEvent()
        {
            Attendees = new List<string>();
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

        public static int GetTotalSecondsForEvents(List<CalendarEvent> calendarEvents)
        {
            int seconds = 0;
            foreach (var singleEvent in calendarEvents)
                seconds += (singleEvent.End - singleEvent.Start).TotalSeconds;

            return seconds;
        }
    }
}
