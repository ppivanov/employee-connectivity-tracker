﻿using EctBlazorApp.Server.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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
    }
}
