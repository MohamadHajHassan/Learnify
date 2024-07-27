using Learnify_backend.Entities;
using Learnify_backend.Services.CourseService;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public LessonsController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet("{moduleId}/lessons")]
        public async Task<ActionResult<IEnumerable<Lesson>>> GetLessonsByModule(string moduleId)
        {
            var lessons = await _courseService.GetLessonsByModuleAsync(moduleId);
            return lessons is not null ? Ok(lessons) : NotFound();
        }

        // GET api/<LessonsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Lesson>> GetLessonById(string id)
        {
            var lesson = await _courseService.GetLessonByIdAsync(id);
            return lesson is not null ? Ok(lesson) : NotFound();
        }

        [HttpPost("{moduleId}/lessons")]
        public async Task<ActionResult> CreateLesson([FromForm] CreateLessonRequest request)
        {
            var lesson = await _courseService.CreateLessonAsync(request);
            return CreatedAtAction(nameof(GetLessonById), new { id = lesson.Id }, lesson);
        }

        // PUT api/<LessonsController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateLesson(string id, Lesson lesson)
        {
            await _courseService.UpdateLesson(id, lesson);
            return Ok();
        }

        // DELETE api/<LessonsController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLesson(string id)
        {
            await _courseService.DeleteLesson(id);
            return Ok();
        }
    }

    public class CreateLessonRequest
    {
        public required string Title { get; set; }
        public required int Ordre { get; set; }
        public required string ModuleId { get; set; }
        public string? TextContent { get; set; }
        public IFormFileCollection? Files { get; set; }
    }
}
