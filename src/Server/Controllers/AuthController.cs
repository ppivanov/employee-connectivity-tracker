using EctBlazorApp.Server.AuthorizationAttributes;
using EctBlazorApp.Server.Extensions;
using EctBlazorApp.Shared;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<bool>> IsAdmin()
        {
            string userEmail = await HttpContext.GetPreferredUsername();
            bool userIsAdmin = _dbContext.IsEmailForAdmin(userEmail);

            return Ok(userIsAdmin);
        }

        [Route("is-leader")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<bool>> IsLeader()
        {
            string userEmail = await HttpContext.GetPreferredUsername();
            bool userIsLeader = _dbContext.IsEmailForLeader(userEmail);

            return Ok(userIsLeader);
        }

        [Route("is-leader-for-team")]
        [AuthorizeLeader]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<EctTeamRequestDetails>> IsLeaderForTeam([FromQuery] string TID = "")
        {
            string userEmail = await HttpContext.GetPreferredUsername();
            EctTeamRequestDetails teamDetails = _dbContext.IsLeaderForTeamId(userEmail, TID);

            return Ok(teamDetails);
        }

        [Route("app-users")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<string>> GetUserEmails()
        {
            var appUsers = _dbContext.Users.Where(u => u.MemberOfId.HasValue == false)
                .Select(u => FormatFullNameAndEmail(u.FullName, u.Email));

            return Ok(appUsers);
        }

        [Route("administrators")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<EctUser>> GetAdminstrators()
        {
            List<EctUser> administrators = _dbContext.Administrators.Include(a => a.User).Select(a => a.User).ToList();
            if(administrators == null)
                administrators = new List<EctUser>();

            return Ok(administrators);
        }
    }
}
