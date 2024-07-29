using Learnify_backend.Controllers;
using Learnify_backend.Entities;

namespace Learnify_backend.Services.UserService
{
    public interface IUserService
    {
        // Instructor
        Task<IEnumerable<Instructor>> GetAllInstructorsAsync();
        Task<Instructor> GetInstructorByIdAsync(string id);
        Task<Instructor> CreateInstructorAsync(CreateInstructorRequest request);
        Task<string> UpdateInstructorAsync(string id, UpdateInstructorRequest request);
        Task DeleteInstructorAsync(string id);
    }
}
