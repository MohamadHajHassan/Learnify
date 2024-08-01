using Learnify_backend.Entities;
using Learnify_backend.Services.TokenService;
using Learnify_backend.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IJWTTokenGenerator _jwtToken;
        private readonly IUserService _userService;

        public UsersController(IJWTTokenGenerator jwtToken, IUserService userService)
        {
            _jwtToken = jwtToken;
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _userService.GetAllUsersAsync();
        }

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

        [HttpPut("setadmin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SetAdmin(string id)
        {
            var result = await _userService.SetAdminAsync(id);
            if (result == "Not Found")
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateUser(string id, [FromForm] UpdateUserRequest request)
        {
            var result = await _userService.UpdateUserAsync(id, request);
            if (result == "Not Found")
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteUser(string id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok();
        }

        [HttpGet("{id}/profilepicture")]
        [Authorize]
        public async Task<IActionResult> GetProfilePhoto(string id)
        {
            return await _userService.GetUserProfilePhotoAsync(id);
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
