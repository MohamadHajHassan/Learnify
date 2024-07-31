using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Learnify_backend.Entities
{
    public class Grade
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRequired]
        [BsonElement("enrollmentId"), BsonRepresentation(BsonType.ObjectId)]
        public string? EnrollmentId { get; set; }

        [BsonRequired]
        [BsonElement("quizId"), BsonRepresentation(BsonType.ObjectId)]
        public string? QuizId { get; set; }

        [BsonElement("userAnswers"), BsonRepresentation(BsonType.Document)]
        public Dictionary<string, UserAnswerResult> UserAnswers { get; set; } = new Dictionary<string, UserAnswerResult>();

        [BsonElement("score"), BsonRepresentation(BsonType.Double)]
        public double Score { get; set; }

        [BsonElement("highestScore"), BsonRepresentation(BsonType.Double)]
        public double HighestScore { get; set; } = 0.0;

        [BsonElement("isPassed"), BsonRepresentation(BsonType.Boolean)]
        public bool IsPassed { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("createdOn"), BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }

    public class UserAnswerResult
    {
        [BsonElement("userAnswer"), BsonRepresentation(BsonType.String)]
        public required string UserAnswer { get; set; }

        [BsonElement("isCorrect"), BsonRepresentation(BsonType.Boolean)]
        public bool IsCorrect { get; set; }
    }
}
