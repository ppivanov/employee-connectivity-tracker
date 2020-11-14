using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EctBlazorApp.Client.Models;
using EctBlazorApp.Shared;

namespace EctBlazorApp.Client.Graph
{
    public interface ICalendarEventsProvider
    {
        Task<IEnumerable<CalendarEvent>> GetEventsInDateRangeAsync(DateTime fromDate, DateTime toDate);

        Task<string> GetCalendarEventsForEmail(string userEmail);
    }
}
