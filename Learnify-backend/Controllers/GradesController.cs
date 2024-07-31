using Learnify_backend.Entities;
using Learnify_backend.Services.EnrollmentService;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradesController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public GradesController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpGet("/enrollment/{enrollmentId}")]
        public async Task<ActionResult<IEnumerable<Grade>>> GetGradesByEnrollment(string enrollmentId)
        {
            var grades = await _enrollmentService.GetGradesByEnrollmentAsync(enrollmentId);
            return grades is not null ? Ok(grades) : NotFound();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Grade>> GetGradeById(string id)
        {
            var grade = await _enrollmentService.GetGradeByIdAsync(id);
            return grade is not null ? Ok(grade) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Grade>> UpdateOrCreateGrade([FromForm] CreateGradeRequest request)
        {
            var grade = await _enrollmentService.UpdateOrCreateGradeAsync(request);
            return CreatedAtAction(nameof(GetGradeById), new { id = grade.Id }, grade);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGrade(string id)
        {
            await _enrollmentService.DeleteGradeAsync(id);
            return Ok();
        }
    }

    public class CreateGradeRequest
    {
        public required string EnrollmentId { get; set; }
        public required string QuizId { get; set; }
        public Dictionary<string, string> UserAnswers { get; set; } = new Dictionary<string, string>();

    }
}
