using Learnify_backend.Entities;
using Learnify_backend.Services.CourseService;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizzesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public QuizzesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet("{moduleId}/quiz")]
        public async Task<ActionResult<Quiz>> GetQuizByModule(string moduleId)
        {
            var quiz = await _courseService.GetQuizByModuleAsync(moduleId);
            return quiz is not null ? Ok(quiz) : NotFound();
        }

        // GET api/<QuizzesController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Quiz>> GetQuizById(string id)
        {
            var quiz = await _courseService.GetQuizByIdAsync(id);
            return quiz is not null ? Ok(quiz) : NotFound();
        }

        // POST api/<QuizzesController>
        [HttpPost("{moduleId}/quiz")]
        public async Task<ActionResult> CreateQuiz([FromForm] CreateQuizRequest request)
        {
            var quiz = await _courseService.CreateQuizAsync(request);
            return CreatedAtAction(nameof(GetQuizById), new { id = quiz.Id }, quiz);
        }

        // DELETE api/<QuizzesController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteQuiz(string id)
        {
            await _courseService.DeleteQuizAsync(id);
            return Ok();
        }
    }

    public class CreateQuizRequest
    {
        public required string ModuleId { get; set; }
        public required double PassingScore { get; set; }
    }
}
