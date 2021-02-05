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
    public class CommunicationWeightsController : ControllerBase
    {

        private readonly EctDbContext _dbContext;

        public CommunicationWeightsController(EctDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Route("weights")]
        [HttpGet]
        public IEnumerable<CommunicationWeight> GetWeights()
        {
            return _dbContext.CommunicationWeights;
        }

        [Route("weights/update")]
        [HttpPut]
        public async Task UpdateWeights([FromBody] IEnumerable<CommunicationWeight> weights)
        {
            _dbContext.CommunicationWeights.RemoveRange(_dbContext.CommunicationWeights);       // Delete all existing records.

            _dbContext.CommunicationWeights.AddRange(weights);                                  // Add the updated list.

            await _dbContext.SaveChangesAsync();
        }
    }
}
