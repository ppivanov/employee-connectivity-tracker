using EctBlazorApp.Server.AuthorizationAttributes;
using EctBlazorApp.Server.Extensions;
using EctBlazorApp.Shared;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        [HttpGet]
        [AuthorizeAdmin]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<EctTeamRequestDetails>> GetAll()
        {
            try
            {
                var allTeams = _dbContext.Teams.Include(t => t.Members).Include(t => t.Leader)
                    .Select(t => new EctTeamRequestDetails(t)).ToList();
                return Ok(allTeams);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error. Please, try again later.");
            }
        }


        [HttpPost]
        [AuthorizeAdmin]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateNewTeam([FromBody] EctTeamRequestDetails teamDetails)
        {

            if (teamDetails.AreDetailsValid() == false)
                return BadRequest("Invalid team details!");

            if (_dbContext.Teams.Any(t => t.Name.ToLower().Equals(teamDetails.Name.ToLower())))
                return Conflict($"A team with the name {teamDetails.Name} already exists.");

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

        [HttpPut]
        [AuthorizeLeader]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateTeam([FromBody] EctTeamRequestDetails teamDetails)
        {
            if (teamDetails.AreDetailsValid() == false)
                return BadRequest("Invalid team details!");

            EctTeam team = _dbContext.Teams.Include(t => t.Members).AsEnumerable()
                .FirstOrDefault(t => ComputeSha256Hash(t.Id.ToString()).Equals(teamDetails.TeamId));
            if (team == null)
                return null;

            team.Members = _dbContext.Users.Where(u => teamDetails.MemberEmails.Contains(u.Email)).ToList();
            team.Leader = _dbContext.Users.FirstOrDefault(u => u.Email.Equals(teamDetails.LeaderEmail));
            team.Members.Add(team.Leader);
            team.AdditionalUsersToNotify = teamDetails.NewNotificationOptions.UsersToNotify;
            team.PointsThreshold = teamDetails.NewNotificationOptions.PointsThreshold;
            team.MarginForNotification = teamDetails.NewNotificationOptions.MarginForNotification;
            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok("Updated successfully");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error. Please, try again later.");
            }
        }

        [HttpDelete("{hashedTeamId}")]
        [AuthorizeAdmin]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteTeam([FromRoute] string hashedTeamId)
        {
            EctTeam teamToDelete = _dbContext.Teams.Include(t => t.Members).Include(t => t.Leader).ToList()
                .FirstOrDefault(t => ComputeSha256Hash(t.Id.ToString()).Equals(hashedTeamId));

            if(teamToDelete == null) 
                return NotFound();
            
            _dbContext.Teams.Remove(teamToDelete);
            try
            {
                await _dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch(Exception)
            {
                return StatusCode(500);
            }
        }

        [Route("move-members")]
        [HttpPut]
        [AuthorizeAdmin]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> MoveMembersBetweenTeams([FromBody] IEnumerable<EctTeamRequestDetails> teamsToUpdate)
        {
            var originalTeams = _dbContext.Teams.Include(t => t.Members).ToList().Where(t =>                                    // Get references to the original teams
                teamsToUpdate.Any(tu => tu.Name.Equals(t.Name))).ToList();             

            if (originalTeams.Count < 1) 
                return BadRequest("No matching teams were found.");

            foreach (var originalTeam in originalTeams)                                                 // This n^2 * m loop should be ok as the number of teams is pretty low and each shouldn't have a lot of members
            {   foreach (var newMembersTeam in teamsToUpdate)
                {
                    if (newMembersTeam.Name.Equals(originalTeam.Name))
                    {
                        originalTeam.Members = _dbContext.Users.ToList()
                            .Where(u => newMembersTeam.MemberNamesAndEmails
                                .Contains(FormatFullNameAndEmail(u.FullName, u.Email)) 
                                || newMembersTeam.LeaderEmail.Equals(u.Email)).ToList();
                    }
                }
            }
            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok("Members moved successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Route("team-stats")]
        [HttpGet]
        [AuthorizeLeader]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

            return Ok(response);
        }

        [Route("team-id")]
        [HttpGet]
        [AuthorizeLeader]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<string>> GetHashedTeamId([FromQuery] string teamName = "")
        {
            string userEmail = await HttpContext.GetPreferredUsername();
            EctUser teamLead = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            //if (string.IsNullOrEmpty(teamName) == false)                                                                                                  // If the team lead has more than 1 team associated with them
            //{
            //    var team = _dbContext.Teams.FirstOrDefault(t => t.Name.ToLower().Equals(teamName.ToLower()));
            //    if (team == null) return NotFound();

            //    return ComputeSha256Hash(team.Id.ToString());
            //}

            var team = _dbContext.Teams.FirstOrDefault(t => t.LeaderId == teamLead.Id);
            return Ok(ComputeSha256Hash(team.Id.ToString()));
        }

        [Route("notification-options")]
        [HttpGet]
        [AuthorizeLeader]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
            return Ok(notificationOptions);
        }

        [Route("notification-options")]
        [HttpPut]
        [AuthorizeLeader]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> SetNotificationOptions([FromBody] NotificationOptionsResponse notificationOptions)
        {
            string userEmail = await HttpContext.GetPreferredUsername();
            int userId = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail).Id;
            EctTeam assignedTeam = _dbContext.Teams.FirstOrDefault(t => t.LeaderId == userId);

            assignedTeam.PointsThreshold = notificationOptions.PointsThreshold;
            assignedTeam.MarginForNotification = notificationOptions.MarginForNotification;
            assignedTeam.AdditionalUsersToNotify = notificationOptions.UsersToNotify;
            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok("Notification options saved.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error. Please, try again later.");
            }
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
