using Learnify_backend.Controllers;
using Learnify_backend.Entities;
using Learnify_backend.Services.EmailService;
using Learnify_backend.Services.FileService;
using Learnify_backend.Services.MongoDbService;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Web;

namespace Learnify_backend.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly IMongoCollection<Instructor> _instructors;
        private readonly IFileService _fileService;

        public UserService(
            IConfiguration configuration,
            IEmailSender emailSender,
            IFileService fileService,
            IMongoDbService mongoDbService)
        {
            _users = mongoDbService.Database.GetCollection<User>("users");
            _configuration = configuration;
            _emailSender = emailSender;
            _instructors = mongoDbService.Database.GetCollection<Instructor>("instructors");
            _fileService = fileService;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _users.Find(FilterDefinition<User>.Empty).ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, id);
            return await _users.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Email, email);
            return await _users.Find(filter).FirstOrDefaultAsync();
        }

        private string GenerateEmailConfirmationToken()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 64)
                .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
        }

        public async Task GenerateAndSendConfirmationEmailAsync(string token, User user)
        {
            var confirmationUrl = BuildConfirmationUrl(token, user.Id);

            var emailBody = BuildConfirmationEmailBody(user.Email, confirmationUrl);

            await _emailSender.SendEmailAsync(
                _configuration["ReturnPaths:SenderEmail"],
                user.Email,
                "Confirm Email Address",
                emailBody);
        }

        private string BuildConfirmationUrl(string token, string userId)
        {
            var uriBuilder = new UriBuilder(_configuration["ReturnPaths:ConfirmEmail"]);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["id"] = userId;
            query["token"] = token;
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();

        }

        private string BuildConfirmationEmailBody(string userEmail, string confirmationUrl)
        {
            return $"Welcome to Learnify!" + Environment.NewLine +
                   $"Thanks for signing up!" + Environment.NewLine +
                   $"You must follow this link to activate your account:" + Environment.NewLine +
                   confirmationUrl;
        }

        public async Task<string> RegisterUserAsync(RegisterUserRequest request)
        {
            var existingUser = await GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return "User with this email already exists!";
            }

            if (request.Password != request.ConfirmPassword)
            {
                return "Passwords do not match!";
            }
            var token = GenerateEmailConfirmationToken();

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "Student",
                EmailConfirmationToken = token
            };

            await _users.InsertOneAsync(user);
            user = await GetUserByEmailAsync(request.Email);
            await GenerateAndSendConfirmationEmailAsync(token, user);
            return "Registered";
        }

        public async Task<string> ConfirmEmailAsync(string id, string token)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, id);
            var user = _users.Find(filter).FirstOrDefault();
            if (user == null)
            {
                return "Not Found";
            }
            else
            {
                if (user.EmailConfirmationToken == token)
                {
                    user.IsEmailConfirmed = true;
                    await _users.ReplaceOneAsync(filter, user);
                    return "Email is confirmed";
                }
                else
                {
                    return "Invalid email confirmation token.";
                }
            }
        }

        public async Task<string> LoginUserAsync(LoginUserRequest request)
        {
            var user = await GetUserByEmailAsync(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return "Invalid Credentials!";
            }

            if (!user.IsEmailConfirmed)
            {
                return "Please confirm your email address to login.";
            }

            return "OK";
        }

        public async Task<string> SetAdminAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, id);
            var user = await _users.Find(filter).FirstOrDefaultAsync();

            if (user == null)
            {
                return "Not Found";
            }

            user.Role = "Admin";
            await _users.ReplaceOneAsync(filter, user);
            return "Updated";
        }

        public async Task<string> SetStudentAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, id);
            var user = await _users.Find(filter).FirstOrDefaultAsync();

            if (user == null)
            {
                return "Not Found";
            }

            user.Role = "Student";
            await _users.ReplaceOneAsync(filter, user);
            return "Updated";
        }

        public async Task<string> UpdateUserAsync(string id, UpdateUserRequest request)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, id);
            var user = await _users.Find(filter).FirstOrDefaultAsync();
            if (user == null)
            {
                return "Not Found";
            }
            if (!String.IsNullOrEmpty(request.FirstName))
            {
                user.FirstName = request.FirstName;
            }
            if (!String.IsNullOrEmpty(request.LastName))
            {
                user.LastName = request.LastName;
            }
            if (!String.IsNullOrEmpty(request.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }
            if (request.ProfilePicture is not null)
            {
                if (!String.IsNullOrEmpty(user.ProfilePictureId))
                {
                    await _fileService.DeleteFilesAsync(new List<string> { user.ProfilePictureId });
                }
                var files = new FormFileCollection();
                files.Add(request.ProfilePicture);

                var ProfilePictureList = await _fileService.UploadFilesAsync(id, files);
                user.ProfilePictureId = ProfilePictureList[0];
            }

            await _users.ReplaceOneAsync(filter, user);
            return "Updated";
        }

        public async Task<IActionResult> GetUserProfilePhotoAsync(string id)
        {
            var user = await _users.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (user == null || string.IsNullOrEmpty(user.ProfilePictureId))
            {
                return new NotFoundResult();
            }
            return await _fileService.DownloadFileAsync(user.ProfilePictureId);
        }

        public async Task DeleteUserAsync(string id)
        {
            await _users.DeleteOneAsync(user => user.Id == id);
        }

        // Instructor
        public async Task<IEnumerable<Instructor>> GetAllInstructorsAsync()
        {
            return await _instructors.Find(FilterDefinition<Instructor>.Empty).ToListAsync();
        }

        public async Task<Instructor> GetInstructorByIdAsync(string id)
        {
            var filter = Builders<Instructor>.Filter.Eq(x => x.Id, id);
            return await _instructors.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Instructor> CreateInstructorAsync(CreateInstructorRequest request)
        {
            var instructor = new Instructor
            {
                Name = request.Name,
                CreatedOn = DateTime.UtcNow
            };

            if (!String.IsNullOrEmpty(request.Bio))
            {
                instructor.Bio = request.Bio;
            }

            if (!String.IsNullOrEmpty(request.Qualifications))
            {
                instructor.Qualifications = request.Qualifications;
            }
            await _instructors.InsertOneAsync(instructor);
            return instructor;
        }

        public async Task<string> UpdateInstructorAsync(string id, UpdateInstructorRequest request)
        {
            var filter = Builders<Instructor>.Filter.Eq(x => x.Id, id);
            var instructor = await _instructors.Find(filter).FirstOrDefaultAsync();
            if (instructor == null)
            {
                return "Not Found";
            }
            if (!String.IsNullOrEmpty(request.Name))
            {
                instructor.Name = request.Name;
            }
            if (!String.IsNullOrEmpty(request.Bio))
            {
                instructor.Bio = request.Bio;
            }
            if (!String.IsNullOrEmpty(request.Qualifications))
            {
                instructor.Qualifications = request.Qualifications;
            }
            if (request.ProfilePicture is not null)
            {
                if (!String.IsNullOrEmpty(instructor.ProfilePictureId))
                {
                    await _fileService.DeleteFilesAsync(new List<string> { instructor.ProfilePictureId });
                }
                var files = new FormFileCollection();
                files.Add(request.ProfilePicture);

                var ProfilePictureList = await _fileService.UploadFilesAsync(id, files);
                instructor.ProfilePictureId = ProfilePictureList[0];
            }
            await _instructors.ReplaceOneAsync(filter, instructor);
            return "Updated";
        }

        public async Task DeleteInstructorAsync(string id)
        {
            await _instructors.DeleteOneAsync(instructor => instructor.Id == id);
        }

        public async Task<IActionResult> GetInstructorProfilePhotoAsync(string id)
        {
            var instructor = await _instructors.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (instructor == null || string.IsNullOrEmpty(instructor.ProfilePictureId))
            {
                return new NotFoundResult();
            }
            return await _fileService.DownloadFileAsync(instructor.ProfilePictureId);
        }
    }
}
