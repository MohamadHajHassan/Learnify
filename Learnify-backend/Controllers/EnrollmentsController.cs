using Learnify_backend.Entities;
using Learnify_backend.Services.EnrollmentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpGet("userId/{userId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetEnrollmentsByUserId(string userId)
        {
            var enrollments = await _enrollmentService.GetEnrollmentsByUserIdAsync(userId);
            return enrollments is not null ? Ok(enrollments) : NotFound();
        }

        [HttpGet("courseId/{courseId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetEnrollmentsByCourseId(string courseId)
        {
            var enrollments = await _enrollmentService.GetEnrollmentsByCourseIdAsync(courseId);
            return enrollments is not null ? Ok(enrollments) : NotFound();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Enrollment>> GetEnrollmentById(string id)
        {
            var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
            return enrollment is not null ? Ok(enrollment) : NotFound();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Enrollment>> CreateEnrollment([FromForm] CreateEnrollmentRequest request)
        {
            var enrollment = await _enrollmentService.CreateEnrollmentAsync(request);
            return CreatedAtAction(nameof(GetEnrollmentById), new { id = enrollment.Id }, enrollment);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateEnrollment(string id, [FromForm] UpdateEnrollmentRequest request)
        {
            var result = await _enrollmentService.UpdateEnrollmentAsync(id, request);
            return result == "Not Found" ? NotFound() : Ok();
        }

        [HttpPut("dropCourse/{id}")]
        [Authorize]
        public async Task<IActionResult> DropCourse(string id)
        {
            await _enrollmentService.DropCourseAsync(id);
            return Ok();
        }
    }

    public class CreateEnrollmentRequest
    {
        public required string UserId { get; set; }
        public required string CourseId { get; set; }
    }
    public class UpdateEnrollmentRequest
    {
        public required string ModuleId { get; set; }
        public string? LessonCompletedId { get; set; }
        public string? QuizCompletedId { get; set; }
        public string? GradeId { get; set; }
    }
}
