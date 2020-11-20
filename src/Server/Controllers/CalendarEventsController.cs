using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using static EctBlazorApp.Shared.SharedCommonMethods;
using Newtonsoft.Json;
using System.Net.Http;
using EctBlazorApp.Shared.GraphModels;
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
            var calendarEvents = 
                await _dbContext.CalendarEvents.Where(c => 
                    c.Start >= NewDateTimeFromString(fromDate) 
                    && c.End < NewDateTimeFromString(toDate)).ToListAsync();
            return calendarEvents;
        }
    }
}
