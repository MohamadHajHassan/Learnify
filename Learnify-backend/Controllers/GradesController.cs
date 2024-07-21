using Learnify_backend.Data;
using Learnify_backend.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradesController : ControllerBase
    {
        private readonly IMongoCollection<Grade> _grades;

        public GradesController(MongoDbService mongoDbService)
        {
            _grades = mongoDbService.Database.GetCollection<Grade>("grades");
        }

        // GET: api/<GradesController>
        [HttpGet]
        public async Task<IEnumerable<Grade>> GetAllCourses()
        {
            return await _grades.Find(FilterDefinition<Grade>.Empty).ToListAsync();
        }

        // GET api/<GradesController>/5
        [HttpGet("{id}")]
        public ActionResult<Grade> GetGradeById(string id)
        {
            var filter = Builders<Grade>.Filter.Eq(x => x.Id, id);
            var grade = _grades.Find(filter).FirstOrDefault();
            return grade is not null ? Ok(grade) : NotFound();
        }

        // POST api/<GradesController>
        [HttpPost]
        public async Task<ActionResult> CreateGrade(Grade grade)
        {
            await _grades.InsertOneAsync(grade);
            return CreatedAtAction(nameof(GetGradeById), new { id = grade.Id }, grade);
        }

        // PUT api/<GradesController>/5
        [HttpPut]
        public async Task<ActionResult> UpdateGrade(Grade grade)
        {
            var filter = Builders<Grade>.Filter.Eq(x => x.Id, grade.Id);
            await _grades.ReplaceOneAsync(filter, grade);
            return Ok();
        }

        // DELETE api/<GradesController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGrade(string id)
        {
            var filter = Builders<Grade>.Filter.Eq(x => x.Id, id);
            await _grades.DeleteOneAsync(filter);
            return Ok();
        }
    }
}
