using EctBlazorApp.Server.AuthorizationAttributes;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.Controllers
{
    [Authorize]
    [Route("api/communication")]
    [ApiController]
    public class CommunicationPercentagesController : ControllerBase
    {

        private readonly EctDbContext _dbContext;

        public CommunicationPercentagesController(EctDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Route("weights")]
        [HttpGet]
        public IEnumerable<CommunicationPercentage> GetWeights()
        {
            return _dbContext.CommunicationPercentages;
        }

        [Route("weights/update")]
        [HttpPut]
        [AuthorizeAdmin]
        public async Task<ActionResult<string>> UpdateWeights([FromBody] IEnumerable<CommunicationPercentage> mediums)
        {
            _dbContext.CommunicationPercentages.RemoveRange(_dbContext.CommunicationPercentages);       // Delete all existing records.

            _dbContext.CommunicationPercentages.AddRange(mediums);                                  // Add the updated list.
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest("Server error. Please, try again later.");
            }
            return Ok("Communication mediums updated successfully");
        }
    }
}
