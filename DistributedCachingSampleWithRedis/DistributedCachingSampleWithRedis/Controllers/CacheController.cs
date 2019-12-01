using System.Collections.Generic;
using DistributedCachingSampleWithRedis.Core.Caching;
using DistributedCachingSampleWithRedis.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DistributedCachingSampleWithRedis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CacheController : ControllerBase
    {
        private readonly ICacheManager cacheManager;
        private readonly IDummyDataRepository repo;
        public CacheController(ICacheManager cacheManager, IDummyDataRepository repo)
        {
            this.cacheManager = cacheManager;
            this.repo = repo;
        }

        [HttpGet("{key}")]
        public IActionResult Get(string key)
        {
            return Ok(this.cacheManager.GetOrCreate<List<string>>(key, null));
        }

        [HttpGet("{key}/create")]
        public IActionResult GetOrCreate(string key)
        {
            return Ok(this.cacheManager.GetOrCreate<List<string>>(key, this.repo.GetValues));
        }

        [HttpPost("{key}")]
        public IActionResult Add(string key, [FromBody]List<string> list)
        {
            this.cacheManager.Set<List<string>>(key, list);
            return Ok();
        }
    }
}
