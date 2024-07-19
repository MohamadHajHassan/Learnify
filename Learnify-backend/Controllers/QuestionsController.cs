using Learnify_backend.Data;
using Learnify_backend.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IMongoCollection<Question> _questions;

        public QuestionsController(MongoDbService mongoDbService)
        {
            _questions = mongoDbService.Database.GetCollection<Question>("questions");
        }

        // GET: api/<QuestionsController>
        [HttpGet]
        public async Task<IEnumerable<Question>> GetAllQuestions()
        {
            return await _questions.Find(FilterDefinition<Question>.Empty).ToListAsync();
        }

        // GET api/<QuestionsController>/5
        [HttpGet("{id}")]
        public ActionResult<Question> GetQuestionById(string id)
        {
            var filter = Builders<Question>.Filter.Eq(x => x.Id, id);
            var question = _questions.Find(filter).FirstOrDefault();
            return question is not null ? Ok(question) : NotFound();
        }

        // POST api/<QuestionsController>
        [HttpPost]
        public async Task<ActionResult> CreateQuestion(Question question)
        {
            await _questions.InsertOneAsync(question);
            return CreatedAtAction(nameof(GetQuestionById), new { id = question.Id }, question);
        }

        // PUT api/<QuestionsController>/5
        [HttpPut]
        public async Task<ActionResult> UpdateQuestion(Question question)
        {
            var filter = Builders<Question>.Filter.Eq(x => x.Id, question.Id);
            await _questions.ReplaceOneAsync(filter, question);
            return Ok();
        }

        // DELETE api/<QuestionsController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteQuestion(string id)
        {
            var filter = Builders<Question>.Filter.Eq(x => x.Id, id);
            await _questions.DeleteOneAsync(filter);
            return Ok();
        }
    }
}
