using iTextSharp.text;
using iTextSharp.text.pdf;
using Learnify_backend.Controllers;
using Learnify_backend.Entities;
using Learnify_backend.Services.CourseService;
using Learnify_backend.Services.Email;
using Learnify_backend.Services.MongoDbService;
using Learnify_backend.Services.UserService;
using MongoDB.Driver;
using System.Net.Mail;
using System.Net.Mime;
namespace Learnify_backend.Services.EnrollmentService
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IMongoCollection<Enrollment> _enrollments;
        private readonly IMongoCollection<Grade> _grades;
        private readonly IMongoCollection<Question> _questions;
        private readonly IMongoCollection<Quiz> _quizzes;
        private readonly IMongoCollection<Certificate> _certificates;
        private readonly IConfiguration _configuration;
        private readonly ICourseService _courseService;
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;

        public EnrollmentService(
            IMongoDbService mongoDbService,
            ICourseService courseService,
            IUserService userService,
            IEmailSender emailSender,
            IConfiguration configuration)
        {
            _enrollments = mongoDbService.Database.GetCollection<Enrollment>("enrollments");
            _grades = mongoDbService.Database.GetCollection<Grade>("grades");
            _questions = mongoDbService.Database.GetCollection<Question>("questions");
            _quizzes = mongoDbService.Database.GetCollection<Quiz>("quizzes");
            _certificates = mongoDbService.Database.GetCollection<Certificate>("certificates");
            _courseService = courseService;
            _userService = userService;
            _emailSender = emailSender;
            _configuration = configuration;
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

                var certificate = await GetCertificateByEnrollmentIdAsync(enrollment.Id);
                if (certificate == null || !certificate.IsSent)
                {
                    await GenerateCertificateAsync(enrollment.Id);
                }

                // Send email

                // Send notification
            }

            return "Updated";
        }

        public async Task DropCourseAsync(string id)
        {
            var filter = Builders<Enrollment>.Filter.Eq(x => x.Id, id);
            var enrollment = await _enrollments.Find(filter).FirstOrDefaultAsync();

            enrollment.IsDropped = true;
            await _enrollments.ReplaceOneAsync(filter, enrollment);
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

        // Certificate
        public async Task<Certificate> GetCertificateByEnrollmentIdAsync(string enrollmentId)
        {
            var filter = Builders<Certificate>.Filter.Eq(x => x.EnrollmentId, enrollmentId);
            return await _certificates.Find(filter).FirstOrDefaultAsync();

        }
        public async Task GenerateCertificateAsync(string enrollmentId)
        {
            var certificate = new Certificate
            {
                EnrollmentId = enrollmentId,
            };
            await _certificates.InsertOneAsync(certificate);

            certificate = await GetCertificateByEnrollmentIdAsync(enrollmentId);
            certificate.CertificateNumber = GenerateUniqueCertificateNumber(certificate.IssuedOn);

            await _certificates.ReplaceOneAsync(x => x.Id == certificate.Id, certificate);

            var enrollment = await GetEnrollmentByIdAsync(enrollmentId);
            var user = await _userService.GetUserByIdAsync(enrollment.UserId);
            var course = await _courseService.GetCourseByIdAsync(enrollment.CourseId);

            var pdfBytes = await GenerateCertificatePdfAsync(certificate, user, course);

            await SendCertificateEmailAsync(pdfBytes, certificate, enrollment, user, course);

            certificate.IsSent = true;

            await _certificates.ReplaceOneAsync(x => x.Id == certificate.Id, certificate);
        }

        private string GenerateUniqueCertificateNumber(DateTime dateTime)
        {
            var random = new Random();
            var randomNumber = random.Next(100000, 999999);
            var certificateNumber = $"CERT_{dateTime}-{randomNumber}";
            return certificateNumber;
        }

        private async Task<byte[]> GenerateCertificatePdfAsync(
            Certificate certificate,
            User user,
            Course course)
        {
            var pdfDoc = new Document();
            var memoryStream = new MemoryStream();
            var writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
            pdfDoc.Open();

            var headerParagraph = new Paragraph("Learnify");
            headerParagraph.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(headerParagraph);

            var paragraph = new Paragraph($"Certificate of Completion\n\n" +
                $"Certificate Number: {certificate.CertificateNumber}\n\n" +
                $"Awarded to:\n" +
                $"{user.FirstName + " " + user.LastName}\n\n" +
                $"For successful completion of the\n" +
                $"{course.Title} course\n\n" +
                $"Issued on: {certificate.IssuedOn}");
            pdfDoc.Add(paragraph);

            pdfDoc.Close();
            return memoryStream.ToArray();
        }

        private async Task SendCertificateEmailAsync(
            byte[] pdfBytes,
            Certificate certificate,
            Enrollment enrollment,
            User user,
            Course course
            )
        {
            var attachmentStream = new MemoryStream(pdfBytes);
            var attachment = new Attachment(attachmentStream, "certificate.pdf", MediaTypeNames.Application.Pdf);

            await _emailSender.SendEmailAsync(
                _configuration["ReturnPaths:SenderEmail"],
                user.Email,
                "Certificate of Completion",
                $"Dear {user.FirstName + " " + user.LastName},\n\n" +
                $"Congratulations! You have successfully completed the {course.Title} course.\n\n" +
                $"Please find attached your certificate of completion.\n\n" +
                $"Best regards,\n" +
                "Learnify Team",
                new List<Attachment> { attachment });
        }
    }
}
