using EctBlazorApp.Server.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.Controllers
{
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

            return userIsAdmin;
        }

        [Route("is-leader/{teamId?}")]
        [HttpGet]
        public async Task<ActionResult<Boolean>> IsLeader(string teamId = "")
        {
            string userEmail = await HttpContext.GetPreferredUsername();
            bool userIsLeader = false;
            if (string.IsNullOrEmpty(teamId))
                userIsLeader = _dbContext.IsEmailForLeader(userEmail);                                      // can potentially throw an exception
            //else
            //    userIsLeader = _dbContext.IsLeaderForTeam(userEmail, teamId);

            return userIsLeader;
        }

        [Route("get-app-users")]
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetUserEmails()
        {
            var appUsers = _dbContext.Users.Where(u => u.MemberOfId.HasValue == false).Select(u => u.Email).ToList();

            return appUsers;
        }
    }
}
