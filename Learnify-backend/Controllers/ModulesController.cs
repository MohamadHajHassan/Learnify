using Learnify_backend.Data;
using Learnify_backend.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly IMongoCollection<Module> _modules;

        public ModulesController(MongoDbService mongoDbService)
        {
            _modules = mongoDbService.Database.GetCollection<Module>("modules");
        }

        // GET: api/<ModulesController>
        [HttpGet]
        public async Task<IEnumerable<Module>> GetAllModules()
        {
            return await _modules.Find(FilterDefinition<Module>.Empty).ToListAsync();
        }

        // GET api/<ModulesController>/5
        [HttpGet("{id}")]
        public ActionResult<Module> GetModuleById(string id)
        {
            var filter = Builders<Module>.Filter.Eq(x => x.Id, id);
            var module = _modules.Find(filter).FirstOrDefault();
            return module is not null ? Ok(module) : NotFound();
        }

        // POST api/<ModulesController>
        [HttpPost]
        public async Task<ActionResult> CreateModule(Module module)
        {
            await _modules.InsertOneAsync(module);
            return CreatedAtAction(nameof(GetModuleById), new { id = module.Id }, module);
        }

        // PUT api/<ModulesController>/5
        [HttpPut]
        public async Task<ActionResult> UpdateModule(Module module)
        {
            var filter = Builders<Module>.Filter.Eq(x => x.Id, module.Id);
            await _modules.ReplaceOneAsync(filter, module);
            return Ok();
        }

        // DELETE api/<ModulesController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteModule(string id)
        {
            var filter = Builders<Module>.Filter.Eq(x => x.Id, id);
            await _modules.DeleteOneAsync(filter);
            return Ok();
        }
    }
}
