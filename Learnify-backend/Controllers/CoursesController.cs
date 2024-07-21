using Learnify_backend.Data;
using Learnify_backend.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly IMongoCollection<Course> _courses;
        private readonly IMongoCollection<Instructor> _instructors;

        public CoursesController(MongoDbService mongoDbService)
        {
            _courses = mongoDbService.Database.GetCollection<Course>("courses");
            _instructors = mongoDbService.Database.GetCollection<Instructor>("instructors");
        }

        // GET: api/<CoursesController>
        [HttpGet]
        public async Task<IEnumerable<Course>> GetAllCourses()
        {
            return await _courses.Find(FilterDefinition<Course>.Empty).ToListAsync();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCourses([FromQuery] CourseSearchParameters searchParameters)
        {
            var filterBuilder = Builders<Course>.Filter;
            var filter = Builders<Course>.Filter.Empty;

            if (!string.IsNullOrEmpty(searchParameters.Keyword))
            {
                var keywordFilter = filterBuilder.Regex(c => c.Title, new BsonRegularExpression(searchParameters.Keyword, "i"));
                filter = filter & keywordFilter;
            }

            if (searchParameters.Categories?.Any() ?? false)
            {
                var categoryFilter = filterBuilder.AnyIn(c => c.Categories, searchParameters.Categories);
                filter = filter & categoryFilter;
            }

            if (!string.IsNullOrEmpty(searchParameters.InstructorName))
            {
                var instFilter = Builders<Instructor>.Filter.Regex(
                    i => i.Name,
                    new BsonRegularExpression(searchParameters.InstructorName, "i"));
                var instructors = await _instructors.Find(instFilter).ToListAsync();
                var instructorIdsList = instructors.Select(i => i.Id);

                var instructorFilter = filterBuilder.In(c => c.InstructorId, instructorIdsList);
                filter = filter & instructorFilter;
            }

            if (searchParameters.DifficultyLevel.HasValue)
            {
                var difficultyFilter = filterBuilder.Eq(c => c.DifficultyLevel, searchParameters.DifficultyLevel.Value);
                filter = filter & difficultyFilter;
            }

            if (searchParameters.MinDuration.HasValue)
            {
                var minDurationFilter = filterBuilder.Gte(c => c.Duration, searchParameters.MinDuration.Value);
                filter = filter & minDurationFilter;
            }

            if (searchParameters.MaxDuration.HasValue)
            {
                var maxDurationFilter = filterBuilder.Lte(c => c.Duration, searchParameters.MaxDuration.Value);
                filter = filter & maxDurationFilter;
            }

            if (searchParameters.MinRating.HasValue)
            {
                var minRatingFilter = filterBuilder.Gte(c => c.AverageRating, searchParameters.MinRating.Value);
                filter = filter & minRatingFilter;
            }

            if (searchParameters.MaxRating.HasValue)
            {
                var maxRatingFilter = filterBuilder.Lte(c => c.AverageRating, searchParameters.MaxRating.Value);
                filter = filter & maxRatingFilter;
            }

            return Ok(await _courses.Find(filter).ToListAsync());

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

    public class CourseSearchParameters
    {
        public string? Keyword { get; set; }
        public List<string>? Categories { get; set; }
        public string? InstructorName { get; set; }
        public int? DifficultyLevel { get; set; }
        public int? MinDuration { get; set; }
        public int? MaxDuration { get; set; }
        public int? MinRating { get; set; }
        public int? MaxRating { get; set; }
    }
}
