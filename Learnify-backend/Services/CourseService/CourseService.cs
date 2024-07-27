using Learnify_backend.Controllers;
using Learnify_backend.Data;
using Learnify_backend.Entities;
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
        private readonly IMongoCollection<Instructor> _instructors;

        public CourseService(MongoDbService mongoDbService)
        {
            _courses = mongoDbService.Database.GetCollection<Course>("courses");
            _modules = mongoDbService.Database.GetCollection<Module>("modules");
            _lessons = mongoDbService.Database.GetCollection<Lesson>("lessons");
            _instructors = mongoDbService.Database.GetCollection<Instructor>("instructors");
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
    }
}
