using Learnify_backend.Controllers;
using Learnify_backend.Entities;

namespace Learnify_backend.Services.EnrollmentService
{
    public interface IEnrollmentService
    {
        public Task<IEnumerable<Enrollment>> GetEnrollmentsByUserIdAsync(string userId);
        public Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseIdAsync(string courseId);
        public Task<Enrollment> GetEnrollmentByIdAsync(string id);
        public Task<Enrollment> CreateEnrollmentAsync(CreateEnrollmentRequest request);
        public Task<string> UpdateEnrollmentAsync(string id, UpdateEnrollmentRequest request);
        public Task<double> CalculateProgressAsync(string courseId, Dictionary<string, ModuleProgress> moduleProgress);
        public Task DropCourseAsync(string id);

    }
}
