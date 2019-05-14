using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Svr.Core.Interfaces;

using System.Threading.Tasks;

namespace Svr.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimsController : ControllerBase
    {
        private readonly IClaimRepository repository;
        private readonly ILogger<ClaimsController> logger;

        public ClaimsController(IClaimRepository repository, ILogger<ClaimsController> logger)
        {
            this.logger = logger;
            this.repository = repository;
        }

        // GET: api/Claims
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var res = await repository.ListAllAsync();
            if (res == null)
            {
                logger.LogWarning($"Контроллер: {nameof(ClaimsController)}, res == null");
                return NotFound();
            }
            return Ok(res);
        }

        // GET: api/Claims/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get(long id)
        {
            var res = await repository.GetByIdAsync(id);
            if (res == null)
            {
                logger.LogWarning($"Контроллер: {nameof(ClaimsController)}, res == null");
                return NotFound();
            }
            return Ok(res);
        }

        // POST: api/Claims
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Claims/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}