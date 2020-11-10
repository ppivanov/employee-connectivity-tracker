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

        private bool CalendarEventExists(int id)
        {
            return _dbContext.CalendarEvents.Any(e => e.Id == id);
        }
    }
}
