using Learnify_backend.Controllers;
using Learnify_backend.Entities;
using Learnify_backend.Services.FileService;
using Learnify_backend.Services.MongoDbService;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Learnify_backend.Services.CourseService
{
    public class CourseService : ICourseService
    {
        private readonly IMongoCollection<Course> _courses;
        private readonly IMongoCollection<Module> _modules;
        private readonly IMongoCollection<Lesson> _lessons;
        private readonly IMongoCollection<Quiz> _quizzes;
        private readonly IMongoCollection<Question> _questions;
        private readonly IMongoCollection<Instructor> _instructors;
        private readonly IFileService _fileService;

        public CourseService(IMongoDbService mongoDbService, IFileService fileService)
        {
            _courses = mongoDbService.Database.GetCollection<Course>("courses");
            _modules = mongoDbService.Database.GetCollection<Module>("modules");
            _lessons = mongoDbService.Database.GetCollection<Lesson>("lessons");
            _quizzes = mongoDbService.Database.GetCollection<Quiz>("quizzes");
            _questions = mongoDbService.Database.GetCollection<Question>("questions");
            _instructors = mongoDbService.Database.GetCollection<Instructor>("instructors");
            _fileService = fileService;

        }

        // Course
        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _courses.Find(FilterDefinition<Course>.Empty).ToListAsync();

        }

        public async Task<IEnumerable<Course>> SearchCoursesAsync(CourseSearchParameters searchParameters)
        {
            var filterBuilder = Builders<Course>.Filter;
            var filter = Builders<Course>.Filter.Empty;

            if (!string.IsNullOrEmpty(searchParameters.Keyword))
            {
                var keywordFilter = filterBuilder.Regex(c => c.Title, new BsonRegularExpression(searchParameters.Keyword, "i"));
                filter = filter & keywordFilter;
            }

            if (searchParameters.Categories?.Any() ?? false)
            {
                var categoryFilter = filterBuilder.AnyIn(c => c.Categories, searchParameters.Categories);
                filter = filter & categoryFilter;
            }

            if (!string.IsNullOrEmpty(searchParameters.InstructorName))
            {
                var instFilter = Builders<Instructor>.Filter.Regex(
                    i => i.Name,
                    new BsonRegularExpression(searchParameters.InstructorName, "i"));
                var instructors = await _instructors.Find(instFilter).ToListAsync();
                var instructorIdsList = instructors.Select(i => i.Id);

                var instructorFilter = filterBuilder.In(c => c.InstructorId, instructorIdsList);
                filter = filter & instructorFilter;
            }

            if (searchParameters.DifficultyLevel.HasValue)
            {
                var difficultyFilter = filterBuilder.Eq(c => c.DifficultyLevel, searchParameters.DifficultyLevel.Value);
                filter = filter & difficultyFilter;
            }

            if (searchParameters.MinDuration.HasValue)
            {
                var minDurationFilter = filterBuilder.Gte(c => c.Duration, searchParameters.MinDuration.Value);
                filter = filter & minDurationFilter;
            }

            if (searchParameters.MaxDuration.HasValue)
            {
                var maxDurationFilter = filterBuilder.Lte(c => c.Duration, searchParameters.MaxDuration.Value);
                filter = filter & maxDurationFilter;
            }

            if (searchParameters.MinRating.HasValue)
            {
                var minRatingFilter = filterBuilder.Gte(c => c.AverageRating, searchParameters.MinRating.Value);
                filter = filter & minRatingFilter;
            }

            if (searchParameters.MaxRating.HasValue)
            {
                var maxRatingFilter = filterBuilder.Lte(c => c.AverageRating, searchParameters.MaxRating.Value);
                filter = filter & maxRatingFilter;
            }

            return await _courses.Find(filter).ToListAsync();
        }

        public async Task<Course> GetCourseByIdAsync(string id)
        {
            return await _courses.Find(course => course.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Course> CreateCourseAsync(CreateCourseRequest request)
        {
            var course = new Course
            {
                Title = request.Title,
                Categories = request.Categories,
                Duration = request.Duration,
                Syllabus = request.Syllabus,
                InstructorId = request.InstructorId
            };

            if (!string.IsNullOrEmpty(request.Description))
            {
                course.Description = request.Description;
            }

            if (request.DifficultyLevel.HasValue)
            {
                course.DifficultyLevel = (int)request.DifficultyLevel;

            }

            if (!request.PreRequisites.IsNullOrEmpty())
            {
                course.PreRequisites = request.PreRequisites;
            }

            await _courses.InsertOneAsync(course);
            return course;
        }

        public async Task<string> UpdateCourseAsync(string id, UpdateCourseRequest request)
        {
            var filter = Builders<Course>.Filter.Eq(x => x.Id, id);
            var course = await _courses.Find(filter).FirstOrDefaultAsync();
            if (course is null)
            {
                return "Not Found";
            }
            if (!string.IsNullOrEmpty(request.Title))
            {
                course.Title = request.Title;
            }
            if (!string.IsNullOrEmpty(request.Description))
            {
                course.Description = request.Description;
            }
            if (request.Categories?.Any() ?? false)
            {
                course.Categories = request.Categories;
            }
            if (request.DifficultyLevel.HasValue)
            {
                course.DifficultyLevel = (int)request.DifficultyLevel;
            }
            if (request.Duration.HasValue)
            {
                course.Duration = request.Duration;
            }
            if (!string.IsNullOrEmpty(request.Syllabus))
            {
                course.Syllabus = request.Syllabus;
            }
            if (request.PreRequisites?.Any() ?? false)
            {
                course.PreRequisites = request.PreRequisites;
            }
            if (!string.IsNullOrEmpty(request.InstructorId))
            {
                course.InstructorId = request.InstructorId;
            }
            course.UpdatedOn = DateTime.UtcNow;
            await _courses.ReplaceOneAsync(filter, course);
            return "Updated";
        }

        public async Task DeleteCourseAsync(string id)
        {
            await _courses.DeleteOneAsync(course => course.Id == id);
        }

        // Module
        public async Task<IEnumerable<Module>> GetModulesByCourseAsync(string courseId)
        {
            var filter = Builders<Module>.Filter.Eq(x => x.CourseId, courseId);
            return await _modules.Find(filter).ToListAsync();
        }

        public async Task<Module> GetModuleByIdAsync(string id)
        {
            return await _modules.Find(module => module.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Module> CreateModuleAsync(CreateModuleRequest request)
        {
            var module = new Module
            {
                CourseId = request.CourseId,
                Ordre = request.Ordre
            };
            await _modules.InsertOneAsync(module);
            return module;
        }

        public async Task<string> UpdateModuleAsync(string id, UpdateModuleRequest request)
        {
            var filter = Builders<Module>.Filter.Eq(x => x.Id, id);
            var module = await _modules.Find(filter).FirstOrDefaultAsync();
            if (module is null)
            {
                return "Not Found";
            }
            module.Ordre = request.Ordre;
            await _modules.ReplaceOneAsync(filter, module);
            return "Updated";
        }

        public async Task DeleteModuleAsync(string id)
        {
            await _modules.DeleteOneAsync(module => module.Id == id);
        }

        // Lesson
        public async Task<IEnumerable<Lesson>> GetLessonsByModuleAsync(string moduleId)
        {
            var filter = Builders<Lesson>.Filter.Eq(x => x.ModuleId, moduleId);
            return await _lessons.Find(filter).ToListAsync();
        }

        public async Task<Lesson> GetLessonByIdAsync(string id)
        {
            return await _lessons.Find(lesson => lesson.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Lesson> CreateLessonAsync(CreateLessonRequest request)
        {
            var lesson = new Lesson
            {
                Title = request.Title,
                Ordre = request.Ordre,
                ModuleId = request.ModuleId
            };

            if (!string.IsNullOrEmpty(request.TextContent))
            {
                lesson.TextContent = request.TextContent;
            }

            if (request.Files?.Any() ?? false)
            {
                lesson.FilesId = await _fileService.UploadFilesAsync(request.ModuleId, request.Files);
            }
            await _lessons.InsertOneAsync(lesson);
            return lesson;
        }

        public async Task<string> UpdateLessonAsync(string id, UpdateLessonRequest request)
        {
            var filter = Builders<Lesson>.Filter.Eq(x => x.Id, id);
            var lesson = await _lessons.Find(filter).FirstOrDefaultAsync();
            if (lesson is null)
            {
                return "Not Found";
            }
            if (!string.IsNullOrEmpty(request.Title))
            {
                lesson.Title = request.Title;
            }
            if (request.Ordre.HasValue)
            {
                lesson.Ordre = request.Ordre.Value;
            }
            if (!string.IsNullOrEmpty(request.TextContent))
            {
                lesson.TextContent = request.TextContent;
            }
            if (request.Files?.Any() ?? false)
            {
                if (lesson.FilesId?.Any() ?? false)
                {
                    await _fileService.DeleteFilesAsync(lesson.FilesId);
                }
                lesson.FilesId = await _fileService.UploadFilesAsync(lesson.ModuleId, request.Files);
            }
            await _lessons.ReplaceOneAsync(filter, lesson);
            return "Updated";
        }

        public async Task DeleteLessonAsync(string id)
        {
            await _lessons.DeleteOneAsync(lesson => lesson.Id == id);
        }

        // Quiz
        public async Task<Quiz> GetQuizByModuleAsync(string moduleId)
        {
            return await _quizzes.Find(quiz => quiz.ModuleId == moduleId).FirstOrDefaultAsync();
        }

        public async Task<Quiz> GetQuizByIdAsync(string id)
        {
            return await _quizzes.Find(quiz => quiz.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Quiz> CreateQuizAsync(CreateQuizRequest request)
        {
            var quiz = new Quiz
            {
                ModuleId = request.ModuleId,
                PassingScore = request.PassingScore
            };
            await _quizzes.InsertOneAsync(quiz);
            return quiz;
        }

        public async Task DeleteQuizAsync(string id)
        {
            await _quizzes.DeleteOneAsync(quiz => quiz.Id == id);
        }

        // Question
        public async Task<IEnumerable<Question>> GetQuestionsByQuizAsync(string quizId)
        {
            var filter = Builders<Question>.Filter.Eq(x => x.QuizId, quizId);
            return await _questions.Find(filter).ToListAsync();
        }

        public async Task<Question> GetQuestionByIdAsync(string id)
        {
            return await _questions.Find(question => question.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Question> CreateQuestionAsync(CreateQuestionRequest request)
        {
            var question = new Question
            {
                QuizId = request.QuizId,
                QuestionText = request.QuestionText,
                Options = request.Options,
                CorrectAnswer = request.CorrectAnswer
            };
            await _questions.InsertOneAsync(question);
            return question;
        }

        public async Task<string> UpdateQuestionAsync(string id, UpdateQuestionRequest request)
        {
            var filter = Builders<Question>.Filter.Eq(x => x.Id, id);
            var question = await _questions.Find(filter).FirstOrDefaultAsync();
            if (question is null)
            {
                return "Not Found";
            }
            if (!string.IsNullOrEmpty(request.QuestionText))
            {
                question.QuestionText = request.QuestionText;
            }
            if (request.Options?.Any() ?? false)
            {
                question.Options = request.Options;
            }
            if (!string.IsNullOrEmpty(request.CorrectAnswer))
            {
                question.CorrectAnswer = request.CorrectAnswer;
            }
            await _questions.ReplaceOneAsync(filter, question);
            return "Updated";
        }

        public async Task DeleteQuestionAsync(string id)
        {
            await _questions.DeleteOneAsync(question => question.Id == id);
        }
    }
}
