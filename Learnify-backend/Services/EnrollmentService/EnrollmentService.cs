using Learnify_backend.Controllers;
using Learnify_backend.Data;
using Learnify_backend.Entities;
using Learnify_backend.Services.CourseService;
using MongoDB.Driver;
namespace Learnify_backend.Services.EnrollmentService
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IMongoCollection<Enrollment> _enrollments;
        private readonly ICourseService _courseService;

        public EnrollmentService(MongoDbService mongoDbService, ICourseService courseService)
        {
            _enrollments = mongoDbService.Database.GetCollection<Enrollment>("enrollments");
            _courseService = courseService;
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByUserIdAsync(string userId)
        {
            var filter = Builders<Enrollment>.Filter.Eq(x => x.UserId, userId)
                & Builders<Enrollment>.Filter.Eq(x => x.IsDropped, false);

            return await _enrollments.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseIdAsync(string courseId)
        {
            var filter = Builders<Enrollment>.Filter.Eq(x => x.CourseId, courseId)
                & Builders<Enrollment>.Filter.Eq(x => x.IsDropped, false);
            return await _enrollments.Find(filter).ToListAsync();
        }

        public async Task<Enrollment> GetEnrollmentByIdAsync(string id)
        {
            var filter = Builders<Enrollment>.Filter.Eq(x => x.Id, id);
            return await _enrollments.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Enrollment> CreateEnrollmentAsync(CreateEnrollmentRequest request)
        {
            var enrollment = new Enrollment
            {
                UserId = request.UserId,
                CourseId = request.CourseId
            };
            await _enrollments.InsertOneAsync(enrollment);
            return enrollment;
        }

        public async Task<double> CalculateProgressAsync(string courseId, Dictionary<string, ModuleProgress> moduleProgress)
        {
            var courseModules = await _courseService.GetModulesByCourseAsync(courseId);
            int totalModules = courseModules.Count();
            int completedModules = moduleProgress.Values.Count(mp => mp.IsCompleted);
            return (double)completedModules / totalModules;
        }

        public async Task<string> UpdateEnrollmentAsync(string id, UpdateEnrollmentRequest request)
        {
            var filter = Builders<Enrollment>.Filter.Eq(x => x.Id, id)
                & Builders<Enrollment>.Filter.Eq(x => x.IsDropped, false);
            var enrollment = await _enrollments.Find(filter).FirstOrDefaultAsync();

            if (enrollment == null)
            {
                return "Not Found";
            }

            var updateDefinition = Builders<Enrollment>.Update.Set(e => e.ModuleProgress[request.ModuleId].ModuleId, request.ModuleId);
            await _enrollments.UpdateOneAsync(filter, updateDefinition);

            if (!String.IsNullOrEmpty(request.LessonCompletedId))
            {
                updateDefinition = Builders<Enrollment>.Update
                    .AddToSet(e => e.ModuleProgress[request.ModuleId].ModuleCompletedLessonsId, request.LessonCompletedId);

                await _enrollments.UpdateOneAsync(filter, updateDefinition);
            }

            if (!String.IsNullOrEmpty(request.QuizCompletedId))
            {
                updateDefinition = Builders<Enrollment>.Update
                    .AddToSet(e => e.CourseCompletedQuizzesId, request.QuizCompletedId)
                    .Set(e => e.ModuleProgress[request.ModuleId].ModuleCompletedQuizId, request.QuizCompletedId)
                    .Set(e => e.ModuleProgress[request.ModuleId].IsCompleted, true)
                    .Set(e => e.Progress, await CalculateProgressAsync(enrollment.CourseId, enrollment.ModuleProgress));

                await _enrollments.UpdateOneAsync(filter, updateDefinition);
            }

            enrollment = await _enrollments.Find(filter).FirstOrDefaultAsync();
            if (enrollment.Progress == 1)
            {
                updateDefinition = Builders<Enrollment>.Update
                    .Set(e => e.IsCompleted, true)
                    .Set(e => e.FinishedOn, DateTime.UtcNow);

                await _enrollments.UpdateOneAsync(filter, updateDefinition);

                // Calculate final grade

                // Send email

                // Send notification

                // Send certificate
            }

            return "Updated";
        }

        public async Task DropCourseAsync(string id)
        {
            var filter = Builders<Enrollment>.Filter.Eq(x => x.Id, id);
            var enrollment = await _enrollments.Find(filter).FirstOrDefaultAsync();

            enrollment.IsDropped = true;
        }
    }
}
