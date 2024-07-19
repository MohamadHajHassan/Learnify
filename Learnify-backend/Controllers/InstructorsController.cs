using Learnify_backend.Data;
using Learnify_backend.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorsController : ControllerBase
    {
        private readonly IMongoCollection<Instructor> _instructors;

        public InstructorsController(MongoDbService mongoDbService)
        {
            _instructors = mongoDbService.Database.GetCollection<Instructor>("instructors");
        }

        // GET: api/<InstructorsController>
        [HttpGet]
        public async Task<IEnumerable<Instructor>> GetAllInstructors()
        {
            return await _instructors.Find(FilterDefinition<Instructor>.Empty).ToListAsync();
        }

        // GET api/<InstructorsController>/5
        [HttpGet("{id}")]
        public ActionResult<Instructor> GetInstructorById(string id)
        {
            var filter = Builders<Instructor>.Filter.Eq(x => x.Id, id);
            var instructor = _instructors.Find(filter).FirstOrDefault();
            return instructor is not null ? Ok(instructor) : NotFound();
        }

        // POST api/<InstructorsController>
        [HttpPost]
        public async Task<ActionResult> Createinstructor(Instructor instructor)
        {
            await _instructors.InsertOneAsync(instructor);
            return CreatedAtAction(nameof(GetInstructorById), new { id = instructor.Id }, instructor);
        }

        // PUT api/<InstructorsController>/5
        [HttpPut]
        public async Task<ActionResult> Updateinstructor(Instructor instructor)
        {
            var filter = Builders<Instructor>.Filter.Eq(x => x.Id, instructor.Id);
            await _instructors.ReplaceOneAsync(filter, instructor);
            return Ok();
        }

        // DELETE api/<InstructorsController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Deleteinstructor(string id)
        {
            var filter = Builders<Instructor>.Filter.Eq(x => x.Id, id);
            await _instructors.DeleteOneAsync(filter);
            return Ok();
        }
    }
}
