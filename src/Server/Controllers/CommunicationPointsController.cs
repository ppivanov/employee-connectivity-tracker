using EctBlazorApp.Server.AuthorizationAttributes;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.Controllers
{
    [Produces("application/json")]
    [Authorize]
    [Route("api/communication")]
    [ApiController]
    public class CommunicationPointsController : ControllerBase
    {

        private readonly EctDbContext _dbContext;

        public CommunicationPointsController(EctDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Route("points")]
        [HttpGet]
        public IEnumerable<CommunicationPoint> GetWeights()
        {
            return _dbContext.CommunicationPoints;
        }

        [Route("points/update")]
        [HttpPut]
        [AuthorizeAdmin]
        public async Task<ActionResult<string>> UpdateWeights([FromBody] IEnumerable<CommunicationPoint> mediums)
        {
            _dbContext.CommunicationPoints.RemoveRange(_dbContext.CommunicationPoints);       // Delete all existing records.

            _dbContext.CommunicationPoints.AddRange(mediums);                                  // Add the updated list.
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
