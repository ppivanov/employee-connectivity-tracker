using EctBlazorApp.Server.AuthorizationAttributes;
using EctBlazorApp.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.Controllers
{
    [Route("api/communication")]
    [ApiController]
    [AuthorizeAdmin]
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
        public async Task UpdateWeights([FromBody] IEnumerable<CommunicationPercentage> mediums)
        {
            _dbContext.CommunicationPercentages.RemoveRange(_dbContext.CommunicationPercentages);       // Delete all existing records.

            _dbContext.CommunicationPercentages.AddRange(mediums);                                  // Add the updated list.

            await _dbContext.SaveChangesAsync();
        }
    }
}
