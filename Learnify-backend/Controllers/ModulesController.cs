using Learnify_backend.Entities;
using Learnify_backend.Services.CourseService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public ModulesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet("{courseId}/modules")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Module>>> GetModulesByCourse(string courseId)
        {
            var modules = await _courseService.GetModulesByCourseAsync(courseId);
            return modules is not null ? Ok(modules) : NotFound();
        }

        // GET api/<ModulesController>/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Module>> GetModuleById(string id)
        {
            var module = await _courseService.GetModuleByIdAsync(id);
            return module is not null ? Ok(module) : NotFound();
        }

        [HttpPost("{courseId}/modules")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateModule([FromBody] CreateModuleRequest request)
        {
            var module = await _courseService.CreateModuleAsync(request);
            return CreatedAtAction(nameof(GetModuleById), new { id = module.Id }, module);
        }

        [HttpPut("{courseId}/modules/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateModule(string id, [FromBody] UpdateModuleRequest request)
        {
            var result = await _courseService.UpdateModuleAsync(id, request);
            if (result == "Not Found")
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpDelete("{courseId}/modules/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteModule(string id)
        {
            await _courseService.DeleteModuleAsync(id);
            return Ok();
        }
    }
    public class CreateModuleRequest
    {
        public required int Ordre { get; set; }
        public required string courseId { get; set; }
    }

    public class UpdateModuleRequest
    {
        public required int Ordre { get; set; }
    }
}
