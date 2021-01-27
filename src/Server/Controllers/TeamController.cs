using EctBlazorApp.Server.AuthorizationAttributes;
using EctBlazorApp.Server.Extensions;
using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EctBlazorApp.Shared.SharedCommonMethods;

namespace EctBlazorApp.Server.Controllers
{
    [Authorize]
    [Route("api/team")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly EctDbContext _dbContext;

        public TeamController(EctDbContext context)
        {
            _dbContext = context;
        }

        [Route("create-team")]
        [HttpPost]
        [AuthorizeAdmin]
        public async Task<ActionResult> CreateNewTeam(EctTeamRequestDetails teamDetails)
        {

            if (teamDetails.AreDetailsValid() == false)
                return BadRequest("Invalid team details!");

            try
            {
                EctUser leader = _dbContext.Users.Include(u => u.LeaderOf).First(u => u.Email.Equals(teamDetails.LeaderEmail));
                List<EctUser> members = _dbContext.Users.Include(u => u.MemberOf).Where(u => teamDetails.MemberEmails.Contains(u.Email)).ToList();
                members.Add(leader);

                EctTeam newTeam = new EctTeam
                {
                    Name = teamDetails.Name,
                    Leader = leader,
                    Members = members
                };

                leader.MakeLeader(newTeam);
                foreach (var member in members)
                    member.MemberOf = newTeam;

                await _dbContext.SaveChangesAsync();

                return Ok("Team created successfully.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error. Please, try again later.");
            }
        }

        [Route("get-team-stats")]
        [HttpGet]
        [AuthorizeLeader]
        public async Task<ActionResult<DashboardResponse>> StatsForDashboard([FromQuery] string fromDate, [FromQuery] string toDate, [FromQuery] string teamId = "")
        {
            string userEmail = await HttpContext.GetPreferredUsername();
            int userId = _dbContext.Users.First(u => u.Email == userEmail).Id;

            DateTime formattedFromDate = NewDateTimeFromString(fromDate);
            DateTime formattedToDate = NewDateTimeFromString(toDate);
            List<ReceivedMail> receivedMail = _dbContext.GetReceivedMailInDateRangeForUserId(userId, formattedFromDate, formattedToDate);
            List<SentMail> sentMail = _dbContext.GetSentMailInDateRangeForUserId(userId, formattedFromDate, formattedToDate);
            List<CalendarEvent> calendarEvents = _dbContext.GetCalendarEventsInDateRangeForUserId(userId, formattedFromDate, formattedToDate);

            double secondsInMeeting = CalendarEvent.GetTotalSecondsForEvents(calendarEvents);

            return new DashboardResponse
            {
                CalendarEvents = calendarEvents,
                ReceivedMail = receivedMail,
                SentMail = sentMail,
                SecondsInMeeting = secondsInMeeting
            };
        }
    }
}
