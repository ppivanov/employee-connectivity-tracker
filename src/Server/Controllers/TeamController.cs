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
    }
}
