using Learnify_backend.Data;
using Learnify_backend.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizzesController : ControllerBase
    {
        private readonly IMongoCollection<Quiz> _quizzes;

        public QuizzesController(MongoDbService mongoDbService)
        {
            _quizzes = mongoDbService.Database.GetCollection<Quiz>("quizzes");
        }

        // GET: api/<QuizzesController>
        [HttpGet]
        public async Task<IEnumerable<Quiz>> GetAllQuizzes()
        {
            return await _quizzes.Find(FilterDefinition<Quiz>.Empty).ToListAsync();
        }

        // GET api/<QuizzesController>/5
        [HttpGet("{id}")]
        public ActionResult<Quiz> GetQuizById(string id)
        {
            var filter = Builders<Quiz>.Filter.Eq(x => x.Id, id);
            var quiz = _quizzes.Find(filter).FirstOrDefault();
            return quiz is not null ? Ok(quiz) : NotFound();
        }

        // POST api/<QuizzesController>
        [HttpPost]
        public async Task<ActionResult> CreateQuiz(Quiz quiz)
        {
            await _quizzes.InsertOneAsync(quiz);
            return CreatedAtAction(nameof(GetQuizById), new { id = quiz.Id }, quiz);
        }

        // PUT api/<QuizzesController>/5
        [HttpPut]
        public async Task<ActionResult> UpdateQuiz(Quiz quiz)
        {
            var filter = Builders<Quiz>.Filter.Eq(x => x.Id, quiz.Id);
            await _quizzes.ReplaceOneAsync(filter, quiz);
            return Ok();
        }

        // DELETE api/<QuizzesController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteQuiz(string id)
        {
            var filter = Builders<Quiz>.Filter.Eq(x => x.Id, id);
            await _quizzes.DeleteOneAsync(filter);
            return Ok();
        }
    }
}
