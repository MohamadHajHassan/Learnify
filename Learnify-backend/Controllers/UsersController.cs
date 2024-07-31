using Learnify_backend.Data;
using Learnify_backend.Entities;
using Learnify_backend.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly JWTTokenGenerator _jwtToken;
        private readonly IUserService _userService;

        public UsersController(
            JWTTokenGenerator jwtToken,
            IUserService userService)
        {
            _jwtToken = jwtToken;
            _userService = userService;
        }

        // GET: api/<UsersController>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _userService.GetAllUsersAsync();
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user is not null ? Ok(user) : NotFound();
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser([FromForm] RegisterUserRequest request)
        {
            var result = await _userService.RegisterUserAsync(request);
            if (result == "User with this email already exists!")
            {
                return BadRequest("User with this email already exists!");
            }
            if (result == "Passwords do not match!")
            {
                return BadRequest("Passwords do not match!");
            }
            var user = await _userService.GetUserByEmailAsync(request.Email);
            return Ok();
        }

        [HttpPost("confirmemail")]
        public async Task<ActionResult> ConfirmEmail(string id, string token)
        {
            var result = await _userService.ConfirmEmailAsync(id, token);
            if (result == "Not Found")
            {
                return NotFound();
            }
            if (result == "Invalid email confirmation token.")
            {
                return BadRequest("Invalid email confirmation token.");
            }
            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> LoginUser([FromForm] LoginUserRequest request)
        {
            var result = await _userService.LoginUserAsync(request);
            if (result == "Invalid Credentials!")
            {
                return BadRequest("Invalid Credentials!");
            }
            if (result == "Please confirm your email address to login.")
            {
                return BadRequest("Please confirm your email address to login.");
            }
            var user = await _userService.GetUserByEmailAsync(request.Email);
            var token = _jwtToken.GenerateJwtToken(user);

            var loginResponse = new LoginResponse
            {
                User = user,
                Token = token
            };
            return Ok(loginResponse);
        }

        [HttpPut("setadmin/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SetAdmin(string userId)
        {
            var result = await _userService.SetAdminAsync(userId);
            if (result == "Not Found")
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpPut("{userId}")]
        [Authorize]
        public async Task<ActionResult> UpdateUser(string userId, [FromForm] UpdateUserRequest request)
        {
            var result = await _userService.UpdateUserAsync(userId, request);
            if (result == "Not Found")
            {
                return NotFound();
            }
            return Ok();
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteUser(string id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok();
        }

        [HttpGet("{userId}/profilepicture")]
        [Authorize]
        public async Task<IActionResult> GetProfilePhoto(string userId)
        {
            return await _userService.GetUserProfilePhotoAsync(userId);
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

    public class UpdateUserRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
    public class LoginResponse
    {
        public required User User { get; set; }
        public required string Token { get; set; }
    }
}
