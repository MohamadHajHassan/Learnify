using Learnify_backend.Controllers;
using Learnify_backend.Entities;
using Learnify_backend.Services.CourseService;
using Learnify_backend.Services.MongoDbService;
using MongoDB.Driver;
namespace Learnify_backend.Services.EnrollmentService
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IMongoCollection<Enrollment> _enrollments;
        private readonly IMongoCollection<Grade> _grades;
        private readonly IMongoCollection<Question> _questions;
        private readonly IMongoCollection<Quiz> _quizzes;
        private readonly ICourseService _courseService;

        public EnrollmentService(IMongoDbService mongoDbService, ICourseService courseService)
        {
            _enrollments = mongoDbService.Database.GetCollection<Enrollment>("enrollments");
            _grades = mongoDbService.Database.GetCollection<Grade>("grades");
            _questions = mongoDbService.Database.GetCollection<Question>("questions");
            _quizzes = mongoDbService.Database.GetCollection<Quiz>("quizzes");
            _courseService = courseService;
        }

        // Enrollment
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
            return (double)completedModules / totalModules * 100;
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
                    .AddToSet(e => e.CompletedModulesId, request.ModuleId)
                    .Set(e => e.ModuleProgress[request.ModuleId].ModuleCompletedQuizId, request.QuizCompletedId)
                    .Set(e => e.ModuleProgress[request.ModuleId].IsCompleted, true)
                    .Set(e => e.Progress, await CalculateProgressAsync(enrollment.CourseId, enrollment.ModuleProgress));

                await _enrollments.UpdateOneAsync(filter, updateDefinition);
            }
            if (!String.IsNullOrEmpty(request.GradeId))
            {
                var gradeFilter = Builders<Grade>.Filter.Eq(x => x.Id, request.GradeId);
                var grade = await _grades.Find(gradeFilter).FirstOrDefaultAsync();

                updateDefinition = Builders<Enrollment>.Update
                    .AddToSet(e => e.GradesId, request.GradeId)
                    .Set(e => e.ModuleProgress[request.ModuleId].ModuleGrade, grade.HighestScore);

                await _enrollments.UpdateOneAsync(filter, updateDefinition);
            }

            enrollment = await _enrollments.Find(filter).FirstOrDefaultAsync();
            if (enrollment.Progress == 100)
            {
                updateDefinition = Builders<Enrollment>.Update
                    .Set(e => e.IsCompleted, true)
                    .Set(e => e.FinishedOn, DateTime.UtcNow)
                    .Set(e => e.FinalGrade, await CalculateFinalGradeAsync(enrollment.Id));

                await _enrollments.UpdateOneAsync(filter, updateDefinition);

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

        // Grade
        public async Task<IEnumerable<Grade>> GetGradesByEnrollmentAsync(string enrollmentId)
        {
            var filter = Builders<Grade>.Filter.Eq(x => x.EnrollmentId, enrollmentId);
            return await _grades.Find(filter).ToListAsync();
        }

        public async Task<Grade> GetGradeByIdAsync(string id)
        {
            var filter = Builders<Grade>.Filter.Eq(x => x.Id, id);
            return await _grades.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Dictionary<string, UserAnswerResult>> CheckUserAnswersAsync(Dictionary<string, string> userAnswers)
        {
            Dictionary<string, UserAnswerResult> userAnswersResult = new Dictionary<string, UserAnswerResult>();

            foreach (var userAnswer in userAnswers)
            {
                var filter = Builders<Question>.Filter.Eq(x => x.Id, userAnswer.Key);
                var question = await _questions.Find(filter).FirstOrDefaultAsync();

                var userAnswerResult = new UserAnswerResult
                {
                    UserAnswer = userAnswer.Value,
                    IsCorrect = false
                };
                if (question.CorrectAnswer == userAnswer.Value)
                {
                    userAnswerResult.IsCorrect = true;
                }
                userAnswersResult.Add(userAnswer.Key, userAnswerResult);
            }
            return userAnswersResult;
        }

        public double CalculateQuizScore(Dictionary<string, UserAnswerResult> userAnswers)
        {
            int CorrectAnswerCount = userAnswers.Count(ua => ua.Value.IsCorrect);
            if (userAnswers.Count == 0)
            {
                return 0;
            }
            return (double)CorrectAnswerCount / userAnswers.Count * 100;
        }

        public async Task<Grade> CreateGradeAsync(CreateGradeRequest request)
        {
            var grade = new Grade
            {
                EnrollmentId = request.EnrollmentId,
                QuizId = request.QuizId,
                UserAnswers = await CheckUserAnswersAsync(request.UserAnswers),
            };
            grade.Score = CalculateQuizScore(grade.UserAnswers);
            grade.HighestScore = grade.Score > grade.HighestScore ? grade.Score : grade.HighestScore;

            var quizFilter = Builders<Quiz>.Filter.Eq(x => x.Id, grade.QuizId);
            var quiz = await _quizzes.Find(quizFilter).FirstOrDefaultAsync();
            grade.IsPassed = grade.Score >= quiz.PassingScore ? true : false;

            await _grades.InsertOneAsync(grade);

            var gradeFilter = Builders<Grade>.Filter.Eq(x => x.Id, grade.Id);
            grade = await _grades.Find(gradeFilter).FirstOrDefaultAsync();

            if (grade.IsPassed)
            {
                await UpdateEnrollmentAsync(
                    request.EnrollmentId,
                    new UpdateEnrollmentRequest
                    {
                        ModuleId = quiz.ModuleId,
                        QuizCompletedId = request.QuizId,
                        GradeId = grade.Id
                    });
            }
            else
            {
                await UpdateEnrollmentAsync(
                    request.EnrollmentId,
                    new UpdateEnrollmentRequest
                    {
                        ModuleId = quiz.ModuleId,
                        GradeId = grade.Id
                    });
            }

            return grade;
        }

        public async Task<Grade> UpdateOrCreateGradeAsync(CreateGradeRequest request)
        {
            var filter = Builders<Grade>.Filter.Eq(x => x.EnrollmentId, request.EnrollmentId)
                & Builders<Grade>.Filter.Eq(x => x.QuizId, request.QuizId);
            var grade = await _grades.Find(filter).FirstOrDefaultAsync();

            if (grade == null)
            {
                return await CreateGradeAsync(request);
            }
            else
            {
                grade.UserAnswers = await CheckUserAnswersAsync(request.UserAnswers);
                grade.Score = CalculateQuizScore(grade.UserAnswers);
                grade.HighestScore = grade.Score > grade.HighestScore ? grade.Score : grade.HighestScore;

                var quizFilter = Builders<Quiz>.Filter.Eq(x => x.Id, grade.QuizId);
                var quiz = await _quizzes.Find(quizFilter).FirstOrDefaultAsync();
                if (!grade.IsPassed)
                {
                    grade.IsPassed = grade.Score >= quiz.PassingScore ? true : false;
                }

                if (grade.IsPassed)
                {
                    await UpdateEnrollmentAsync(
                        request.EnrollmentId,
                        new UpdateEnrollmentRequest
                        {
                            ModuleId = quiz.ModuleId,
                            QuizCompletedId = request.QuizId,
                            GradeId = grade.Id
                        });
                }
                else
                {
                    await UpdateEnrollmentAsync(
                        request.EnrollmentId,
                        new UpdateEnrollmentRequest
                        {
                            ModuleId = quiz.ModuleId,
                            GradeId = grade.Id
                        });
                }

                await _grades.ReplaceOneAsync(filter, grade);
                return grade;
            }
        }

        public async Task DeleteGradeAsync(string id)
        {
            await _grades.DeleteOneAsync(grade => grade.Id == id);
        }

        public async Task<double> CalculateFinalGradeAsync(string enrollmentId)
        {
            var filter = Builders<Enrollment>.Filter.Eq(x => x.Id, enrollmentId);
            var enrollment = await _enrollments.Find(filter).FirstOrDefaultAsync();

            var grades = await _grades.Find(x => enrollment.GradesId.Contains(x.Id)).ToListAsync();
            var totalGrades = grades.Sum(g => g.HighestScore);
            return totalGrades / grades.Count;
        }
    }
}
