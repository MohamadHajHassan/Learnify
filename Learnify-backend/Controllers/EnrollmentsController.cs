using Learnify_backend.Data;
using Learnify_backend.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IMongoCollection<Enrollment> _enrollments;

        public EnrollmentsController(MongoDbService mongoDbService)
        {
            _enrollments = mongoDbService.Database.GetCollection<Enrollment>("enrollments");
        }

        // GET api/<EnrollmentsController>/5
        [HttpGet("{id}")]
        public ActionResult<Enrollment> GetEnrollmentById(string id)
        {
            var filter = Builders<Enrollment>.Filter.Eq(x => x.Id, id);
            var enrollment = _enrollments.Find(filter).FirstOrDefault();
            return enrollment is not null ? Ok(enrollment) : NotFound();
        }

        [HttpGet("{userId}/enrollments")]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetEnrollmentsByUser(string userId)
        {
            var filter = Builders<Enrollment>.Filter.Eq(x => x.UserId, userId);
            var enrollments = await _enrollments.Find(filter).ToListAsync();
            return enrollments is not null ? Ok(enrollments) : NotFound();
        }

        [HttpPost("{userId}/courses/{courseId}")]
        public async Task<ActionResult> EnrollUserToCourse(string userId, string courseId)
        {
            var enrollment = new Enrollment
            {
                UserId = userId,
                CourseId = courseId
            };
            await _enrollments.InsertOneAsync(enrollment);
            return CreatedAtAction(nameof(GetEnrollmentById), new { id = enrollment.Id }, enrollment);
        }

        [HttpPut("{enrollmentId}/progress")]
        public async Task<IActionResult> UpdateEnrollmentProgress(string enrollmentId, [FromBody] double progress)
        {
            var filter = Builders<Enrollment>.Filter.Eq(e => e.Id, enrollmentId);
            var update = Builders<Enrollment>.Update.Set(e => e.Progress, progress);
            await _enrollments.UpdateOneAsync(filter, update);
            return Ok();
        }

        [HttpPut("{enrollmentId}/modules/{moduleId}")]
        public async Task<IActionResult> CompleteModule(string enrollmentId, string moduleId)
        {
            var filter = Builders<Enrollment>.Filter.Eq(e => e.Id, enrollmentId);
            var update = Builders<Enrollment>.Update.AddToSet(e => e.CompletedModulesId, moduleId);
            await _enrollments.UpdateOneAsync(filter, update);
            return Ok();
        }

        [HttpPut("{enrollmentId}/grades")]
        public async Task<IActionResult> AddGrade(string enrollmentId, [FromBody] string gradeId)
        {
            var filter = Builders<Enrollment>.Filter.Eq(e => e.Id, enrollmentId);
            var update = Builders<Enrollment>.Update.AddToSet(e => e.GradesId, gradeId);
            await _enrollments.UpdateOneAsync(filter, update);
            return Ok();
        }

        [HttpDelete("{userId}/courses/{courseId}")]
        public async Task<IActionResult> DropCourse(string userId, string courseId)
        {
            var filter = Builders<Enrollment>.Filter.And(
                Builders<Enrollment>.Filter.Eq(e => e.UserId, userId),
                Builders<Enrollment>.Filter.Eq(e => e.CourseId, courseId)
            );
            await _enrollments.DeleteOneAsync(filter);
            return Ok();
        }
    }
}
