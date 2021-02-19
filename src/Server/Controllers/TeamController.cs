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
        public async Task<ActionResult<List<EctUser>>> GetStatsForDashboard([FromQuery] string fromDate, [FromQuery] string toDate, [FromQuery] string teamId = "")
        {
            string userEmail = await HttpContext.GetPreferredUsername();
            int userId = _dbContext.Users.First(u => u.Email == userEmail).Id;
            EctTeam assignedTeam = _dbContext.Teams.Include(t => t.Members).First(t => t.LeaderId == userId);
            List<EctUser> membersAndCommsData = new List<EctUser>();

            DateTime formattedFromDate = NewDateTimeFromString(fromDate);
            DateTime formattedToDate = NewDateTimeFromString(toDate);

            foreach (var teamMember in assignedTeam.Members)
            {
                membersAndCommsData.Add(GetCommunicationDataAsNewUserInstance(teamMember, formattedFromDate, formattedToDate));
            }

            return membersAndCommsData;
        }

        [Route("get-points-threshold")]
        [HttpGet]
        [AuthorizeLeader]
        public async Task<ActionResult<int>> GetCurrentPointsThreshold([FromQuery] string teamId = "")
        {
            string userEmail = await HttpContext.GetPreferredUsername();
            int userId = _dbContext.Users.First(u => u.Email == userEmail).Id;
            EctTeam assignedTeam = _dbContext.Teams.First(t => t.LeaderId == userId);
            
            return assignedTeam.PointsThreshold;
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
