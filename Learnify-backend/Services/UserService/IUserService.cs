using Learnify_backend.Controllers;
using Learnify_backend.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Learnify_backend.Services.UserService
{
    public interface IUserService
    {
        // User
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(string id);
        Task<User> GetUserByEmailAsync(string email);
        Task GenerateAndSendConfirmationEmailAsync(string token, User user);
        Task<string> RegisterUserAsync(RegisterUserRequest request);
        Task<string> ConfirmEmailAsync(string id, string token);
        Task<string> LoginUserAsync(LoginUserRequest request);
        Task<string> SetAdminAsync(string id);
        Task<string> UpdateUserAsync(string id, UpdateUserRequest request);
        Task<IActionResult> GetUserProfilePhotoAsync(string id);
        Task DeleteUserAsync(string id);

        // Instructor
        Task<IEnumerable<Instructor>> GetAllInstructorsAsync();
        Task<Instructor> GetInstructorByIdAsync(string id);
        Task<Instructor> CreateInstructorAsync(CreateInstructorRequest request);
        Task<string> UpdateInstructorAsync(string id, UpdateInstructorRequest request);
        Task DeleteInstructorAsync(string id);
    }
}
