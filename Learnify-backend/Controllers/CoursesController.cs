using Learnify_backend.Data;
using Learnify_backend.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly IMongoCollection<Course> _courses;

        public CoursesController(MongoDbService mongoDbService)
        {
            _courses = mongoDbService.Database.GetCollection<Course>("courses");
        }

        // GET: api/<CoursesController>
        [HttpGet]
        public async Task<IEnumerable<Course>> GetAllCourses()
        {
            return await _courses.Find(FilterDefinition<Course>.Empty).ToListAsync();
        }

        // GET api/<CoursesController>/5
        [HttpGet("{id}")]
        public ActionResult<Course> GetCourseById(string id)
        {
            var filter = Builders<Course>.Filter.Eq(x => x.Id, id);
            var course = _courses.Find(filter).FirstOrDefault();
            return course is not null ? Ok(course) : NotFound();
        }

        // POST api/<CoursesController>
        [HttpPost]
        public async Task<ActionResult> CreateCourse(Course course)
        {
            await _courses.InsertOneAsync(course);
            return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, course);
        }

        // PUT api/<CoursesController>/5
        [HttpPut]
        public async Task<ActionResult> UpdateCourse(Course course)
        {
            var filter = Builders<Course>.Filter.Eq(x => x.Id, course.Id);
            await _courses.ReplaceOneAsync(filter, course);
            return Ok();
        }

        // DELETE api/<CoursesController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCourse(string id)
        {
            var filter = Builders<Course>.Filter.Eq(x => x.Id, id);
            await _courses.DeleteOneAsync(filter);
            return Ok();
        }
    }
}
