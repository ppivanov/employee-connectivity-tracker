using EctBlazorApp.Server.AuthorizationAttributes;
using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;

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
                EctUser leader = _dbContext.Users.First(u => u.Email.Equals(teamDetails.LeaderEmail));
                List<EctUser> members = _dbContext.Users.Where(u => teamDetails.MemberEmails.Contains(u.Email)).ToList();
                members.Add(leader);

                EctTeam newTeam = new EctTeam
                {
                    Name = teamDetails.Name,
                    Leader = leader,
                    Members = members
                };
                _dbContext.Teams.Add(newTeam);
                await _dbContext.SaveChangesAsync();

                return Ok("Create team endpoint");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error. Please, try again later.");
            }
        }
    }
}
