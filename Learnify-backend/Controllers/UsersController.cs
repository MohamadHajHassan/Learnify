using Learnify_backend.Data;
using Learnify_backend.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;

        public UsersController(MongoDbService mongoDbService)
        {
            _users = mongoDbService.Database.GetCollection<User>("users");
        }

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<IEnumerable<User>> Get()
        {
            return await _users.Find(FilterDefinition<User>.Empty).ToListAsync();
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public ActionResult<User> GetById(string id)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, id);
            var user = _users.Find(filter).FirstOrDefault();
            return user is not null ? Ok(user) : NotFound();
        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<ActionResult> CreateUser(User user)
        {
            await _users.InsertOneAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
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

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = HashedPassword,
                Role = "student"
            };
            await _users.InsertOneAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        // PUT api/<UsersController>
        [HttpPut]
        public async Task<ActionResult> UpdateUser(User user)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, user.Id);
            await _users.ReplaceOneAsync(filter, user);
            return Ok();
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, id);
            await _users.DeleteOneAsync(filter);
            return Ok();
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
}
