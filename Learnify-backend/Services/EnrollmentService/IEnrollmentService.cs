using Learnify_backend.Controllers;
using Learnify_backend.Entities;

namespace Learnify_backend.Services.EnrollmentService
{
    public interface IEnrollmentService
    {
        // Enrollment
        public Task<IEnumerable<Enrollment>> GetEnrollmentsByUserIdAsync(string userId);
        public Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseIdAsync(string courseId);
        public Task<Enrollment> GetEnrollmentByIdAsync(string id);
        public Task<Enrollment> CreateEnrollmentAsync(CreateEnrollmentRequest request);
        public Task<string> UpdateEnrollmentAsync(string id, UpdateEnrollmentRequest request);
        public Task<double> CalculateProgressAsync(string courseId, Dictionary<string, ModuleProgress> moduleProgress);
        public Task DropCourseAsync(string id);

        // Grade
        public Task<IEnumerable<Grade>> GetGradesByEnrollmentAsync(string enrollmentId);
        public Task<Grade> GetGradeByIdAsync(string id);
        public Task<Dictionary<string, UserAnswerResult>> CheckUserAnswersAsync(Dictionary<string, string> userAnswers);
        public double CalculateQuizScore(Dictionary<string, UserAnswerResult> userAnswers);
        public Task<Grade> CreateGradeAsync(CreateGradeRequest request);
        public Task<Grade> UpdateOrCreateGradeAsync(CreateGradeRequest request);
        public Task DeleteGradeAsync(string id);
        public Task<double> CalculateFinalGradeAsync(string enrollmentId);

        // Certificate
        public Task<Certificate> GetCertificateByEnrollmentIdAsync(string enrollmentId);
        public Task GenerateCertificateAsync(string enrollmentId);
    }
}
