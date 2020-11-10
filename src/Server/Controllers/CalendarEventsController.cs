using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // GET: api/CalendarEvents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CalendarEvent>>> GetCalendarEvents()
        {
            return await _dbContext.CalendarEvents.ToListAsync();
        }

        // POST: api/CalendarEvents
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Route("post-calendar-events")]
        [HttpPost]
        public async Task<ActionResult<CalendarEvent>> PostCalendarEvents(CalendarEvent[] calendarEvents)
        {
            try
            {
                _dbContext.CalendarEvents.AddRange(calendarEvents);
                _dbContext.SaveChanges();
            } catch (DbUpdateException ex)
            {
                // TODO - log
                return BadRequest();
            }

            return Ok("Success");
        }

        private bool CalendarEventExists(int id)
        {
            return _dbContext.CalendarEvents.Any(e => e.Id == id);
        }
    }
}
