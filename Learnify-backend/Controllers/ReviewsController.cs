using Learnify_backend.Entities;
using Learnify_backend.Services.MongoDbService;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Learnify_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IMongoCollection<Review> _reviews;

        public ReviewsController(IMongoDbService mongoDbService)
        {
            _reviews = mongoDbService.Database.GetCollection<Review>("reviews");
        }

        // GET: api/<ReviewsController>
        [HttpGet]
        public async Task<IEnumerable<Review>> GetAllreviews()
        {
            return await _reviews.Find(FilterDefinition<Review>.Empty).ToListAsync();
        }

        // GET api/<ReviewsController>/5
        [HttpGet("{id}")]
        public ActionResult<Review> GetreviewById(string id)
        {
            var filter = Builders<Review>.Filter.Eq(x => x.Id, id);
            var review = _reviews.Find(filter).FirstOrDefault();
            return review is not null ? Ok(review) : NotFound();
        }

        // POST api/<ReviewsController>
        [HttpPost]
        public async Task<ActionResult> Createreview(Review review)
        {
            await _reviews.InsertOneAsync(review);
            return CreatedAtAction(nameof(GetreviewById), new { id = review.Id }, review);
        }

        // PUT api/<ReviewsController>/5
        [HttpPut]
        public async Task<ActionResult> Updatereview(Review review)
        {
            var filter = Builders<Review>.Filter.Eq(x => x.Id, review.Id);
            await _reviews.ReplaceOneAsync(filter, review);
            return Ok();
        }

        // DELETE api/<ReviewsController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Deletereview(string id)
        {
            var filter = Builders<Review>.Filter.Eq(x => x.Id, id);
            await _reviews.DeleteOneAsync(filter);
            return Ok();
        }
    }
}

