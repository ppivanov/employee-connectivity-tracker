using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.GraphModels
{

    public class GraphEventsResponse
    {
        public MicrosoftGraphEvent[] Value { get; set; }
    }

    public class MicrosoftGraphEvent
    {
        public string Subject { get; set; }
        public DateTimeZone Start { get; set; }
        public DateTimeZone End { get; set; }
        public EventAttendee Organizer { get; set; }
        public EventAttendee[] Attendees { get; set; }
    }

    public class EventAttendee
    {
        public EventEmailAddress emailAddress { get; set; }

        public override string ToString()
        {
            return $"{emailAddress.Name} <{emailAddress.Address}>";
        }
    }

    public class EventEmailAddress
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class DateTimeZone
    {
        public string DateTime { get; set; }
        public string TimeZone { get; set; }

        public DateTime ConvertToLocalDateTime()
        {
            var dateTime = System.DateTime.Parse(DateTime);

            TimeZoneInfo timeZone = null;
            if (TimeZone == "UTC")
                timeZone = TimeZoneInfo.Utc;
            else
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZone);

            return new DateTimeOffset(dateTime, timeZone.BaseUtcOffset).LocalDateTime;
        }
    }
}
