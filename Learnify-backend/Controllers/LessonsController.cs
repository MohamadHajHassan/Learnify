using Learnify_backend.Data;
using Learnify_backend.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly IMongoCollection<Lesson> _lessons;

        public LessonsController(MongoDbService mongoDbService)
        {
            _lessons = mongoDbService.Database.GetCollection<Lesson>("lessons");
        }

        // GET: api/<LessonsController>
        [HttpGet]
        public async Task<IEnumerable<Lesson>> GetAllLessons()
        {
            return await _lessons.Find(FilterDefinition<Lesson>.Empty).ToListAsync();
        }

        // GET api/<LessonsController>/5
        [HttpGet("{id}")]
        public ActionResult<Lesson> GetLessonById(string id)
        {
            var filter = Builders<Lesson>.Filter.Eq(x => x.Id, id);
            var lesson = _lessons.Find(filter).FirstOrDefault();
            return lesson is not null ? Ok(lesson) : NotFound();
        }

        // POST api/<LessonsController>
        [HttpPost]
        public async Task<ActionResult> CreateLesson(Lesson lesson)
        {
            await _lessons.InsertOneAsync(lesson);
            return CreatedAtAction(nameof(GetLessonById), new { id = lesson.Id }, lesson);
        }

        // PUT api/<LessonsController>/5
        [HttpPut]
        public async Task<ActionResult> UpdateLesson(Lesson lesson)
        {
            var filter = Builders<Lesson>.Filter.Eq(x => x.Id, lesson.Id);
            await _lessons.ReplaceOneAsync(filter, lesson);
            return Ok();
        }

        // DELETE api/<LessonsController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLesson(string id)
        {
            var filter = Builders<Lesson>.Filter.Eq(x => x.Id, id);
            await _lessons.DeleteOneAsync(filter);
            return Ok();
        }
    }
}
