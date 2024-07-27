using Learnify_backend.Data;
using Learnify_backend.Entities;
using Learnify_backend.Services.CourseService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(MongoDbService mongoDbService)
        {
            _courseService = new CourseService(mongoDbService);
        }

        // GET: api/<CoursesController>
        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Course>> GetAllCourses()
        {
            return await _courseService.GetAllCoursesAsync();
        }

        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> SearchCourses([FromQuery] CourseSearchParameters searchParameters)
        {
            var courses = await _courseService.SearchCoursesAsync(searchParameters);
            return Ok(courses);
        }

        // GET api/<CoursesController>/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Course>> GetCourseById(string id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            return course is not null ? Ok(course) : NotFound();
        }

        // POST api/<CoursesController>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateCourse(CreateCourseRequest request)
        {
            var course = await _courseService.CreateCourseAsync(request);
            return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, course);
        }

        // PUT api/<CoursesController>/5
        [HttpPut("{courseId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateCourse(string courseId, [FromBody] UpdateCourseRequest request)
        {
            string result = await _courseService.UpdateCourseAsync(courseId, request);
            if (result == "Not Found")
            {
                return NotFound();
            }
            return Ok();
        }

        // DELETE api/<CoursesController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteCourse(string id)
        {
            await _courseService.DeleteCourseAsync(id);
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

    public class CreateCourseRequest
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required List<string> Categories { get; set; }
        public int? DifficultyLevel { get; set; }
        public required double Duration { get; set; }
        public required string Syllabus { get; set; }
        public List<string>? PreRequisites { get; set; }
        public required string InstructorId { get; set; }
    }

    public class UpdateCourseRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<string>? Categories { get; set; }
        public int? DifficultyLevel { get; set; }
        public double? Duration { get; set; }
        public string? Syllabus { get; set; }
        public List<string>? PreRequisites { get; set; }
        public string? InstructorId { get; set; }
    }
}
