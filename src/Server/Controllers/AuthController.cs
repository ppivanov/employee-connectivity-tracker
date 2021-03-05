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
        public async Task<ActionResult<bool>> IsAdmin()
        {
            string userEmail = await HttpContext.GetPreferredUsername();
            bool userIsAdmin = _dbContext.IsEmailForAdmin(userEmail);

            return Ok(userIsAdmin);
        }

        [Route("is-leader")]
        [HttpGet]
        public async Task<ActionResult<bool>> IsLeader()
        {
            string userEmail = await HttpContext.GetPreferredUsername();
            bool userIsLeader = _dbContext.IsEmailForLeader(userEmail);

            return Ok(userIsLeader);
        }

        [Route("is-leader-for-team")]
        public async Task<ActionResult<EctTeamRequestDetails>> IsLeaderForTeam([FromQuery] string TID = "")
        {
            string userEmail = await HttpContext.GetPreferredUsername();
            EctTeamRequestDetails teamDetails = _dbContext.IsLeaderForTeamId(userEmail, TID);

            return Ok(teamDetails);
        }

        [Route("get-app-users")]
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetUserEmails()
        {
            var appUsers = _dbContext.Users.Where(u => u.MemberOfId.HasValue == false)
                .Select(u => FormatFullNameAndEmail(u.FullName, u.Email));

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
