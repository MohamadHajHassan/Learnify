using Learnify_backend.Entities;
using Learnify_backend.Services.CourseService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("/moduleId/{moduleId}/quiz")]
        [Authorize]
        public async Task<ActionResult<Quiz>> GetQuizByModule(string moduleId)
        {
            var quiz = await _courseService.GetQuizByModuleAsync(moduleId);
            return quiz is not null ? Ok(quiz) : NotFound();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Quiz>> GetQuizById(string id)
        {
            var quiz = await _courseService.GetQuizByIdAsync(id);
            return quiz is not null ? Ok(quiz) : NotFound();
        }

        [HttpPost("/moduleId/{moduleId}/quiz")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateQuiz([FromForm] CreateQuizRequest request)
        {
            var quiz = await _courseService.CreateQuizAsync(request);
            return CreatedAtAction(nameof(GetQuizById), new { id = quiz.Id }, quiz);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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
