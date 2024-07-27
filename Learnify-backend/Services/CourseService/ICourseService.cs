using Learnify_backend.Controllers;
using Learnify_backend.Entities;

namespace Learnify_backend.Services.CourseService
{
    public interface ICourseService
    {
        // Course
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<IEnumerable<Course>> SearchCoursesAsync(CourseSearchParameters searchParameters);
        Task<Course> GetCourseByIdAsync(string id);
        Task<Course> CreateCourseAsync(CreateCourseRequest request);
        Task<string> UpdateCourseAsync(string id, UpdateCourseRequest request);
        Task DeleteCourseAsync(string id);

        // Module
        Task<IEnumerable<Module>> GetModulesByCourseAsync(string courseId);
        Task<Module> GetModuleByIdAsync(string id);
        Task<Module> CreateModuleAsync(CreateModuleRequest request);
        Task<string> UpdateModuleAsync(string id, UpdateModuleRequest request);
        Task DeleteModuleAsync(string id);
    }
}
