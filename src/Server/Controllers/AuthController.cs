using EctBlazorApp.Server.Extensions;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.Controllers
{
    [Produces("application/json")]
    [Authorize]
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EctDbContext _dbContext;

        public AuthController(EctDbContext context)
        {
            _dbContext = context;
        }

        [Route("is-admin")]
        [HttpGet]
        public async Task<ActionResult<Boolean>> IsAdmin()
        {
            string userEmail = await HttpContext.GetPreferredUsername();
            bool userIsAdmin = _dbContext.IsEmailForAdmin(userEmail);

            return Ok(userIsAdmin);
        }

        [Route("is-leader/{teamId?}")]
        [HttpGet]
        public async Task<ActionResult<Boolean>> IsLeader(string teamId = "")
        {
            string userEmail = await HttpContext.GetPreferredUsername();
            bool userIsLeader = false;
            if (string.IsNullOrEmpty(teamId))
                userIsLeader = _dbContext.IsEmailForLeader(userEmail);
            //else
            //    userIsLeader = _dbContext.IsLeaderForTeam(userEmail, teamId);

            return Ok(userIsLeader);
        }

        [Route("get-app-users")]
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetUserEmails()
        {
            var appUsers = _dbContext.Users.Where(u => u.MemberOfId.HasValue == false).Select(u => u.Email).ToList();

            return Ok(appUsers);
        }

        [Route("get-administrators")]
        [HttpGet]
        public ActionResult<IEnumerable<EctUser>> GetAdminstrators()
        {
            List<EctUser> administrators = _dbContext.Administrators.Include(a => a.User).Select(a => a.User).ToList();
            if(administrators == null)
            {
                administrators = new List<EctUser>();
            }

            return Ok(administrators);
        }

        [Route("my-email")]
        [HttpGet]
        public async Task<ActionResult<string>> GetEmailForProcessingUser()
        {
            string userEmail = await HttpContext.GetPreferredUsername();

            return Ok(userEmail);
        }
    }
}
