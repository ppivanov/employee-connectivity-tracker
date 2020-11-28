using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        [HttpPut]
        public async Task<ActionResult> CreateNewTeam()
        {
            return Ok("Create team endpoint");
        }
    }
}
