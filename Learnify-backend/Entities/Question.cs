using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Learnify_backend.Entities
{
    public class Question
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRequired]
        [BsonElement("quizId"), BsonRepresentation(BsonType.ObjectId)]
        public string? QuizId { get; set; }

        [BsonRequired]
        [BsonElement("questionText"), BsonRepresentation(BsonType.String)]
        public string? QuestionText { get; set; }

        [BsonRequired]
        [BsonElement("options"), BsonRepresentation(BsonType.String)]
        public IEnumerable<string> Options { get; set; } = new List<string>();

        [BsonRequired]
        [BsonElement("correctAnswer"), BsonRepresentation(BsonType.String)]
        public string? CorrectAnswer { get; set; }
    }
}
