using Learnify_backend.Entities;
using Learnify_backend.Services.UserService;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorsController : ControllerBase
    {
        private readonly IUserService _userService;

        public InstructorsController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IEnumerable<Instructor>> GetAllInstructors()
        {
            return await _userService.GetAllInstructorsAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Instructor>> GetInstructorById(string id)
        {
            var instructor = await _userService.GetInstructorByIdAsync(id);
            return instructor is not null ? Ok(instructor) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> CreateInstructor([FromForm] CreateInstructorRequest request)
        {
            var instructor = await _userService.CreateInstructorAsync(request);
            return CreatedAtAction(nameof(GetInstructorById), new { id = instructor.Id }, instructor);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateInstructor(string id, [FromForm] UpdateInstructorRequest request)
        {
            string result = await _userService.UpdateInstructorAsync(id, request);
            return result == "Not Found" ? NotFound() : Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteInstructor(string id)
        {
            await _userService.DeleteInstructorAsync(id);
            return Ok();
        }
    }

    public class CreateInstructorRequest
    {
        public required string Name { get; set; }
        public string? Bio { get; set; }
        public string? Qualifications { get; set; }
    }

    public class UpdateInstructorRequest
    {
        public string? Name { get; set; }
        public string? Bio { get; set; }
        public string? Qualifications { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
}
