using EctBlazorApp.Server.AuthorizationAttributes;
using EctBlazorApp.Server.Extensions;
using EctBlazorApp.Shared;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EctBlazorApp.Shared.SharedMethods;

namespace EctBlazorApp.Server.Controllers
{
    [Produces("application/json")]
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

            string leaderEmail = GetEmailFromFormattedString(teamDetails.LeaderNameAndEmail);
            EctUser leader = _dbContext.Users.Include(u => u.LeaderOf).FirstOrDefault(u => u.Email.Equals(leaderEmail));
            List<EctUser> members = _dbContext.Users.Where(u => teamDetails.MemberEmails.Contains(u.Email)).ToList();
            members.Add(leader);
            EctTeam newTeam = new()
            {
                Name = teamDetails.Name,
                Leader = leader,
                Members = members,
                AdditionalUsersToNotify = new List<string> { teamDetails.LeaderNameAndEmail }
            };
            try
            {
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
        public async Task<ActionResult<TeamDashboardResponse>> GetStatsForDashboard([FromQuery] string fromDate, [FromQuery] string toDate)
        {
            string userEmail = await HttpContext.GetPreferredUsername();
            EctUser teamLead = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            EctTeam assignedTeam = _dbContext.Teams.Include(t => t.Members).FirstOrDefault(t => t.LeaderId == teamLead.Id);
            TeamDashboardResponse response = new()
            {
                TeamName = assignedTeam.Name,
                TeamMembers = new List<EctUser>(),
                LeaderNameAndEmail = FormatFullNameAndEmail(teamLead.FullName, teamLead.Email)
            };

            DateTime formattedFromDate = NewDateTimeFromString(fromDate);
            DateTime formattedToDate = NewDateTimeFromString(toDate);

            foreach (var teamMember in assignedTeam.Members)
            {
                response.TeamMembers.Add(GetCommunicationDataAsNewUserInstance(teamMember, formattedFromDate, formattedToDate));
            }

            return response;
        }

        [Route("get-notification-options")]
        [HttpGet]
        [AuthorizeLeader]
        public async Task<ActionResult<NotificationOptionsResponse>> GetCurrentPointsThreshold()
        {
            string userEmail = await HttpContext.GetPreferredUsername();
            int userId = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail).Id;
            EctTeam assignedTeam = _dbContext.Teams.FirstOrDefault(t => t.LeaderId == userId);

            var notificationOptions = new NotificationOptionsResponse
            {
                PointsThreshold = assignedTeam.PointsThreshold,
                MarginForNotification = assignedTeam.MarginForNotification,
                UsersToNotify = assignedTeam.AdditionalUsersToNotify
            };
            return notificationOptions;
        }

        [Route("set-notification-options")]
        [HttpPut]
        [AuthorizeLeader]
        public async Task<ActionResult<string>> SetNotificationOptions([FromBody] NotificationOptionsResponse notificationOptions)
        {
            string userEmail = await HttpContext.GetPreferredUsername();
            int userId = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail).Id;
            EctTeam assignedTeam = _dbContext.Teams.FirstOrDefault(t => t.LeaderId == userId);

            assignedTeam.PointsThreshold = notificationOptions.PointsThreshold;
            assignedTeam.MarginForNotification = notificationOptions.MarginForNotification;
            assignedTeam.AdditionalUsersToNotify = notificationOptions.UsersToNotify;
            await _dbContext.SaveChangesAsync();

            return Ok("Notification options saved.");
        }

        private EctUser GetCommunicationDataAsNewUserInstance(EctUser forUser, DateTime fromDate, DateTime toDate)
        {
            EctUser tempUser = new EctUser(forUser);
            tempUser.CalendarEvents = _dbContext.GetCalendarEventsInDateRangeForUserId(tempUser.Id, fromDate, toDate);
            tempUser.ReceivedEmails = _dbContext.GetReceivedMailInDateRangeForUserId(tempUser.Id, fromDate, toDate);
            tempUser.SentEmails = _dbContext.GetSentMailInDateRangeForUserId(tempUser.Id, fromDate, toDate);

            return tempUser;
        }
    }
}
