using Learnify_backend.Entities;
using Learnify_backend.Services.CourseService;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public QuestionsController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet("{quizId}/questions")]
        public async Task<ActionResult<IEnumerable<Question>>> GetQuestionsByQuiz(string quizId)
        {
            var questions = await _courseService.GetQuestionsByQuizAsync(quizId);
            return questions is not null ? Ok(questions) : NotFound();
        }

        // GET api/<QuestionsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Question>> GetQuestionById(string id)
        {
            var question = await _courseService.GetQuestionByIdAsync(id);
            return question is not null ? Ok(question) : NotFound();
        }

        [HttpPost("{quizId}/questions")]
        public async Task<ActionResult> CreateQuestion([FromForm] CreateQuestionRequest request)
        {
            var question = await _courseService.CreateQuestionAsync(request);
            return CreatedAtAction(nameof(GetQuestionById), new { id = question.Id }, question);
        }

        [HttpPut("{quizId}/questions/{id}")]
        public async Task<ActionResult> UpdateQuestion(string id, [FromForm] UpdateQuestionRequest request)
        {
            var result = await _courseService.UpdateQuestionAsync(id, request);
            if (result == "Not Found")
            {
                return NotFound();
            }
            return Ok();
        }

        // DELETE api/<QuestionsController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteQuestion(string id)
        {
            await _courseService.DeleteQuestionAsync(id);
            return Ok();
        }
    }

    public class CreateQuestionRequest
    {
        public required string QuizId { get; set; }
        public required string QuestionText { get; set; }
        public required IEnumerable<string> Options { get; set; }
        public required string CorrectAnswer { get; set; }
    }

    public class UpdateQuestionRequest
    {
        public string? QuestionText { get; set; }
        public IEnumerable<string>? Options { get; set; }
        public string? CorrectAnswer { get; set; }
    }
}
