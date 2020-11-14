using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.Graph;
using static EctBlazorApp.Server.CommonMethods.CommonDateMethods;
using Newtonsoft.Json;
using System.Net.Http;
using EctBlazorApp.Client.Models;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

namespace EctBlazorApp.Server.Controllers
{
    [Authorize]
    [Route("api/main")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly EctDbContext _dbContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(EctDbContext context, ILogger<HomeController> logger)
        {
            _dbContext = context;
            _logger = logger;
        }

        [Route("update-records")]
        [HttpGet]
        public async Task<ActionResult> UpdateDatabaseRecordsForUser([FromQuery] string graphToken, [FromQuery] string userId)
        {
            var userForUserIdParm = _dbContext.Users.First(user => user.Email.Equals(userId));
            if(userForUserIdParm == null)
            {
                // request user 
                // save user
                _logger.LogInformation($"Adding new user: {userId}"); // bad idea?
            }
            // update calendarEvents
            // update receivedMail
            // update sentMail
            return Ok();
        }

    }
}
