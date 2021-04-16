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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<EctTeamRequestDetails>> GetAll()
        {
            try
            {
                var allTeams = _dbContext.Teams.ToList().Select(t => new EctTeamRequestDetails(t)).ToList();
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
            if (teamDetails.AreDetailsValid() == false)                                                     // This check is done on the front end before submitting the form.
                return BadRequest("Invalid team details!");                                                 // Running it again here in case another client is connected.

            if (_dbContext.Teams.Any(t => t.Name.ToLower().Equals(teamDetails.Name.ToLower())))
                return Conflict($"A team with the name {teamDetails.Name} already exists.");

            string leaderEmail = GetEmailFromFormattedString(teamDetails.LeaderNameAndEmail);
            EctUser leader = _dbContext.Users.FirstOrDefault(u => u.Email.Equals(leaderEmail));
            List<EctUser> members = _dbContext.Users.Where(u => teamDetails.MemberEmails.Contains(u.Email)).ToList();
            members.Add(leader);                                                                            // In most cases the leader is filtered out of the member list. 
                                                                                                            // It may be possible to not add them as a member at all.
            EctTeam newTeam = new()
            {
                Name = teamDetails.Name,
                Leader = leader,
                Members = members,
                AdditionalUsersToNotify = new List<string> { teamDetails.LeaderNameAndEmail },
                PointsThreshold =0 ,
                MarginForNotification = 100
            };
            try
            {
                _dbContext.Teams.Add(newTeam);
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

            EctTeam team = _dbContext.GetTeamForTeamId(teamDetails.TeamId);
            if (team == null)
                return null;

            team.Members = _dbContext.Users.Where(u => teamDetails.MemberEmails.Contains(u.Email)).ToList();
            team.Leader = _dbContext.Users.FirstOrDefault(u => u.Email.Equals(teamDetails.LeaderEmail));
            team.Members.Add(team.Leader);                                                                  // It may be possible to not add the leader as a member at all.      
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
            EctTeam teamToDelete = _dbContext.GetTeamForTeamId(hashedTeamId);

            if(teamToDelete == null) 
                return NotFound();
            
            try
            {
                _dbContext.Teams.Remove(teamToDelete);
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
            var originalTeams = _dbContext.Teams.AsEnumerable().Where(t =>                                  // Get references to the original teams
                teamsToUpdate.Any(tu => tu.Name.Equals(t.Name))).ToList();             

            if (originalTeams.Count < 1) 
                return BadRequest("No matching teams were found.");

            foreach (var originalTeam in originalTeams)                                                     // This ( n^2 * m ) loop should be ok as the number of teams is pretty low and each shouldn't have a lot of members
            {   foreach (var newMembersTeam in teamsToUpdate)
                {
                    if (newMembersTeam.Name.Equals(originalTeam.Name))
                        originalTeam.Members = _dbContext.GetMembersFromStringListAndLeader(
                                                    newMembersTeam.MemberNamesAndEmails, newMembersTeam.LeaderNameAndEmail);
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
            EctTeam assignedTeam = teamLead.LeaderOf.FirstOrDefault();

            var response = GenerateTeamDashboardResponse(assignedTeam, fromDate, toDate);

            return Ok(response);
        }

        [Route("team-id")]
        [HttpGet]
        [AuthorizeLeader]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<string>> GetHashedTeamId([FromQuery] string teamName = "")                                                           // The rest of the endopoints can be defined in a similar 
        {                                                                                                                                                   // manner to enable multiple teams per team lead.
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
            EctUser user = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            EctTeam assignedTeam = user.LeaderOf.FirstOrDefault();

            NotificationOptionsResponse notificationOptions = new()
            {
                PointsThreshold = assignedTeam.PointsThreshold,
                MarginForNotification = assignedTeam.MarginForNotification,
                UsersToNotify = assignedTeam.AdditionalUsersToNotify
            };

            return Ok(notificationOptions);
        }

        private static TeamDashboardResponse GenerateTeamDashboardResponse(EctTeam forTeam, string fromDate, string toDate)
        {
            TeamDashboardResponse response = new()
            {
                TeamName = forTeam.Name,
                TeamMembers = new(),
                LeaderNameAndEmail = FormatFullNameAndEmail(forTeam.Leader.FullName, forTeam.Leader.Email)
            };

            DateTime formattedFromDate = NewDateTimeFromString(fromDate);
            DateTime formattedToDate = NewDateTimeFromString(toDate);

            foreach (var teamMember in forTeam.Members)
                response.TeamMembers.Add(GetCommunicationDataAsNewUserInstance(teamMember, formattedFromDate, formattedToDate));

            return response;
        }

        private static EctUser GetCommunicationDataAsNewUserInstance(EctUser forUser, DateTime fromDate, DateTime toDate)
        {
            EctUser tempUser = new(forUser);

            tempUser.CalendarEvents = forUser.GetCalendarEventsInDateRange(fromDate, toDate);
            tempUser.ReceivedEmails = forUser.GetReceivedMailInDateRange(fromDate, toDate);
            tempUser.SentEmails = forUser.GetSentMailInDateRange(fromDate, toDate);

            return tempUser;
        }
    }
}
