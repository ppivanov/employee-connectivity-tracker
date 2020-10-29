// Code adapted from https://github.com/microsoftgraph/msgraph-training-aspnet-core/blob/master/demo/GraphTutorial/Controllers/CalendarController.cs

using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static EctWebApp.CommonMethods.CommonStaticMethods;

namespace EctWebApp.Models
{
    public class CalendarEvent
    {
        public List<string> Attendees { get; private set; }
        public DateTime End { get; private set; }
        public Importance EventImportance { get; private set; }
        public string Organizer { get; private set; }
        public DateTime Start { get; private set; }
        public string Subject { get; private set; }

        public CalendarEvent(Event graphEvent)
        {
            Attendees = new List<string>();
            foreach (Attendee attendee in graphEvent.Attendees)
            {
                Attendees.Add(FormatNameAndEmailAddressParms(attendee.EmailAddress.Name, attendee.EmailAddress.Address));
            }
            End = DateTime.Parse(graphEvent.End.DateTime);
            EventImportance = graphEvent.Importance.Value;
            Organizer = graphEvent.Organizer.EmailAddress.Name;
            Start = DateTime.Parse(graphEvent.Start.DateTime);
            Subject = graphEvent.Subject;
        }

        public static async Task<List<CalendarEvent>> GetCalendarEventsForQueryOptionsParmAsync(List<QueryOption> queryOptions,
                                                                                                string timeZone,
                                                                                                GraphServiceClient graphClient)
        {
            var graphEvents = await GetGraphEventsForQueryOptionsParm(queryOptions, timeZone, graphClient);
            List<CalendarEvent> calendarEventsList = new List<CalendarEvent>();
            if (graphEvents != null)
            {
                foreach (Event calendarEvent in graphEvents)
                {
                    calendarEventsList.Add(new CalendarEvent(calendarEvent));
                }
            }
            return calendarEventsList;
        }

        private static async Task<IList<Event>> GetGraphEventsForQueryOptionsParm(List<QueryOption> queryOptions,
                                                                                  string timeZone,
                                                                                  GraphServiceClient graphClient)
        {
            IUserCalendarViewCollectionPage calendarPages = await GetCalendarCollectionPageForQueryParms(queryOptions, 
                                                                                                         timeZone, 
                                                                                                         graphClient);
            IList<Event> calendarEvents = await GetGraphEventsFromCalendarPagesParm(calendarPages, graphClient);

            return calendarEvents;
        }

        private static async Task<IUserCalendarViewCollectionPage> GetCalendarCollectionPageForQueryParms(List<QueryOption> queryOptions,
                                                                                                          string timeZone,
                                                                                                          GraphServiceClient graphClient)
        {
            IUserCalendarViewCollectionPage calendarPages = await graphClient.Me
                .CalendarView
                .Request(queryOptions)
                .Header("Prefer", $"outlook.timezone=\"{timeZone}\"")                               // time zone used so date/time in
                .Select(e => new                                                                    // response will be in preferred time zone
                {
                    e.Attendees,
                    e.End,
                    e.Importance,
                    e.Organizer,
                    e.Start,
                    e.Subject
                })
                .GetAsync();
            return calendarPages;
        }

        private static async Task<IList<Event>> GetGraphEventsFromCalendarPagesParm(IUserCalendarViewCollectionPage calendarPages, GraphServiceClient graphClient)
        {
            IList<Event> calendarEvents;
            if (calendarPages.NextPageRequest != null)                                              // Handle case where there are more than 50
            {
                calendarEvents = new List<Event>();
                PageIterator<Event> pageIterator = PageIterator<Event>.CreatePageIterator(          // Create a page iterator to iterate over subsequent pages
                    graphClient, calendarPages,                                                     // of results. Build a list from the results
                    (e) =>
                    {
                        calendarEvents.Add(e);
                        return true;
                    }
                );
                await pageIterator.IterateAsync();
            }
            else
                calendarEvents = calendarPages.CurrentPage;                                         // If only one page - use the result

            return calendarEvents;
        }
        
    }
}
