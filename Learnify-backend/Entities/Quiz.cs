using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Learnify_backend.Entities
{
    public class Quiz
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRequired]
        [BsonElement("moduleId"), BsonRepresentation(BsonType.ObjectId)]
        public string? ModuleId { get; set; }

        [BsonElement("questionsId"), BsonRepresentation(BsonType.String)]
        public IEnumerable<string> QuestionsId { get; set; } = new List<string>();

        [BsonElement("passingScore"), BsonRepresentation(BsonType.Double)]
        public double PassingScore { get; set; }
    }
}
