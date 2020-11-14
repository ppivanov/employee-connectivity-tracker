using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.Graph;
using static EctBlazorApp.Server.CommonMethods.CommonDateMethods;
using Newtonsoft.Json;
using System.Net.Http;
using EctBlazorApp.Client.Models;
using System.Net.Http.Headers;

namespace EctBlazorApp.Server.Controllers
{
    [Authorize]
    [Route("api/calendar")]
    [ApiController]
    public class CalendarEventsController : ControllerBase
    {
        private readonly EctDbContext _dbContext;

        public CalendarEventsController(EctDbContext context)
        {
            _dbContext = context;
        }

        [Route("get-calendar-events")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CalendarEvent>>> GetCalendarEvents([FromQuery] string fromDate, [FromQuery] string toDate)
        {
            var calendarEvents = await _dbContext.CalendarEvents.Where(c => c.Start >= NewDateTimeFromString(fromDate) && c.End < NewDateTimeFromString(toDate)).ToListAsync();
            return calendarEvents;
        }

        [Route("post-calendar-events")]
        [HttpPost]
        public async Task<ActionResult<CalendarEvent>> PostCalendarEvents(CalendarEvent[] calendarEvents)
        {
            try
            {
                _dbContext.CalendarEvents.AddRange(calendarEvents);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // TODO - log
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [Route("get-calendar-events-api")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CalendarEvent>>> GetCalendarEventsOnBehalfOfUser([FromQuery] string graphToken, [FromQuery] string userId)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", graphToken);
                var response = await client.GetAsync("https://graph.microsoft.com/v1.0/users/" + userId + "/events");

                string contentAsString = await response.Content.ReadAsStringAsync();
                var graphEvents = JsonConvert.DeserializeObject<GraphEventsResponse>(contentAsString);

                int attendeesLimit = 20;
                var events = new List<CalendarEvent>();

                foreach (var graphEvent in graphEvents.Value)
                {
                    CalendarEvent calendarEvent = new CalendarEvent
                    {
                        Subject = graphEvent.Subject,
                        Start = graphEvent.Start.ConvertToLocalDateTime(),
                        End = graphEvent.End.ConvertToLocalDateTime(),
                        Organizer = graphEvent.Organizer.ToString(),
                        Attendees = new List<string>()
                    };
                    // TODO -> find a solution or remove the limit
                    for (int i = 0; i < Math.Min(attendeesLimit, graphEvent.Attendees.Length); i++)         // if the graphEvent is a large group meeting,
                        calendarEvent.Attendees.Add(graphEvent.Attendees[i].ToString());                    // then perhaps we don't want to waste the resourses 
                                                                                                            // transferring them over to the database
                    events.Add(calendarEvent);
                }
                return events;
            }
        }

        private bool CalendarEventExists(int id)
        {
            return _dbContext.CalendarEvents.Any(e => e.Id == id);
        }
    }
}
