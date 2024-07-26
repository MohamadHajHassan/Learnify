using Learnify_backend.Data;
using Learnify_backend.Entities;
using Learnify_backend.Services.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System.Web;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;
        private readonly JWTTokenGenerator _jwtToken;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly GridFSBucket _gridFsBucket;

        public UsersController(
            MongoDbService mongoDbService,
            JWTTokenGenerator jwtToken,
            IConfiguration configuration,
            IEmailSender emailSender)
        {
            _users = mongoDbService.Database.GetCollection<User>("users");
            _jwtToken = jwtToken;
            _configuration = configuration;
            _emailSender = emailSender;
            _gridFsBucket = new GridFSBucket(mongoDbService.Database);
        }

        // GET: api/<UsersController>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IEnumerable<User>> Get()
        {
            return await _users.Find(FilterDefinition<User>.Empty).ToListAsync();
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<User> GetById(string id)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, id);
            var user = _users.Find(filter).FirstOrDefault();
            return user is not null ? Ok(user) : NotFound();
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser(
            [FromBody] RegisterUserRequest request)
        {
            var existingUser = _users.Find(x => x.Email == request.Email).FirstOrDefault();
            if (existingUser != null)
            {
                return BadRequest("User with this email already exists!");
            }

            if (request.Password != request.ConfirmPassword)
            {
                return BadRequest("Passwords do not match!");
            }

            var HashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var token = GenerateEmailConfirmationTokenAsync();
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = HashedPassword,
                Role = "Student",
                EmailConfirmationToken = token,
            };
            await _users.InsertOneAsync(user);

            var uriBuilder = new UriBuilder(_configuration["ReturnPaths:ConfirmEmail"]);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["token"] = token;
            query["userid"] = user.Id;
            uriBuilder.Query = query.ToString();
            var urlString = uriBuilder.ToString();

            var senderEmail = _configuration["ReturnPaths:SenderEmail"];
            await _emailSender.SendEmailAsync(senderEmail, user.Email, "Confirm Email Address",
                "Welcome to Learnify!" + Environment.NewLine +
                "Thanks for signing up!" + Environment.NewLine +
                "You must follow this link to activate your account:" + Environment.NewLine +
                urlString);

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        private string GenerateEmailConfirmationTokenAsync()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 64)
                .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
        }

        [HttpPost("confirmemail")]
        public async Task<ActionResult> ConfirmEmail(string id, string token)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, id);
            var user = _users.Find(filter).FirstOrDefault();
            if (user == null)
            {
                return BadRequest();
            }
            else
            {
                if (user.EmailConfirmationToken == token)
                {
                    user.IsEmailConfirmed = true;
                    await _users.ReplaceOneAsync(filter, user);
                    return Ok();
                }
                else
                {
                    return BadRequest("Invalid email confirmation token.");
                }
            }
        }

        [HttpPost("login")]
        public ActionResult<LoginResponse> LoginUser([FromBody] LoginUserRequest request)
        {
            var user = _users.Find(x => x.Email == request.Email).FirstOrDefault();
            if (user == null)
            {
                return BadRequest("Invalid Credentials!");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return BadRequest("Invalid Credentials!");
            }

            if (!user.IsEmailConfirmed)
            {
                return BadRequest("Please confirm your email address to login.");
            }

            var token = _jwtToken.GenerateJwtToken(user);

            var loginResponse = new LoginResponse
            {
                User = user,
                Token = token
            };

            return Ok(loginResponse);
        }

        // PUT api/<UsersController>
        [HttpPut("setadmin/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SetAdmin(string userId)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var user = await _users.Find(filter).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            user.Role = "Admin";
            await _users.ReplaceOneAsync(filter, user);
            return Ok();
        }

        [HttpPut("{userId}")]
        [Authorize]
        public async Task<ActionResult> UpdateUserInfo(string userId, [FromBody] UpdateUserInfoRequest request)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);

            var user = await _users.Find(filter).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(request.FirstName))
            {
                user.FirstName = request.FirstName;
            }

            if (!string.IsNullOrEmpty(request.LastName))
            {
                user.LastName = request.LastName;
            }

            if (!string.IsNullOrEmpty(request.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            await _users.ReplaceOneAsync(filter, user);
            return Ok();
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, id);
            await _users.DeleteOneAsync(filter);
            return Ok();
        }

        [HttpPut("{userId}/profilepicture")]
        [Authorize]
        public async Task<ActionResult> UpdateProfilePicture(string userId, IFormFile file)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var user = await _users.Find(filter).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(user.ProfilePictureId))
            {
                await _gridFsBucket.DeleteAsync(new ObjectId(user.ProfilePictureId));
            }
            var options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument { { "contentType", file.ContentType } }
            };

            using (var stream = file.OpenReadStream())
            {
                var uploadResult = await _gridFsBucket.UploadFromStreamAsync($"{userId}_profile_photo", stream, options);
                user.ProfilePictureId = uploadResult.ToString();
            }

            await _users.ReplaceOneAsync(x => x.Id == userId, user);

            return Ok();
        }

        [HttpGet("{userId}/profilepicture")]
        [Authorize]
        public async Task<IActionResult> GetProfilePhoto(string userId)
        {
            var user = await _users.Find(x => x.Id == userId).FirstOrDefaultAsync();
            if (user == null || string.IsNullOrEmpty(user.ProfilePictureId))
            {
                return NotFound();
            }

            var downloadStream = await _gridFsBucket.OpenDownloadStreamAsync(new ObjectId(user.ProfilePictureId));
            if (downloadStream == null)
            {
                return NotFound();
            }

            var contentType = downloadStream.FileInfo.Metadata.GetValue("contentType").AsString;
            return new FileStreamResult(downloadStream, contentType);
        }
    }

    public class RegisterUserRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }

    public class LoginUserRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class UpdateUserInfoRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }
    }
    public class LoginResponse
    {
        public User User { get; set; }
        public string Token { get; set; }
    }
}
