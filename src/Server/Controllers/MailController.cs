using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EctBlazorApp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static EctBlazorApp.Shared.SharedCommonMethods;

namespace EctBlazorApp.Server.Controllers
{
    [Authorize]
    [Route("api/mail")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly EctDbContext _dbContext;

        public MailController(EctDbContext context)
        {
            _dbContext = context;
        }

        [Route("get-received-mail")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReceivedMail>>> GetReceivedMailInDateRange([FromQuery] string fromDate, [FromQuery] string toDate)
        {
            var receivedMail =
                await _dbContext.ReceivedEmails.Where(c =>
                    c.ReceivedAt >= NewDateTimeFromString(fromDate)
                    && c.ReceivedAt < NewDateTimeFromString(toDate)).ToListAsync();
            return receivedMail;
        }

        [Route("get-sent-mail")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SentMail>>> GetSentMailInDateRange([FromQuery] string fromDate, [FromQuery] string toDate)
        {
            var sentMail =
                await _dbContext.SentEmails.Where(c =>
                    c.SentAt >= NewDateTimeFromString(fromDate)
                    && c.SentAt < NewDateTimeFromString(toDate)).ToListAsync();
            return sentMail;
        }
    }
}
