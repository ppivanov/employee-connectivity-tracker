using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EctBlazorApp.Server.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EctBlazorApp.Server.Controllers
{
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
    }
}
