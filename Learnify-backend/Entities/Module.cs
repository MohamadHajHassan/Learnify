using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Learnify_backend.Entities
{
    public class Module
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRequired]
        [BsonElement("ordre"), BsonRepresentation(BsonType.Int32)]
        public int Ordre { get; set; }

        [BsonRequired]
        [BsonElement("courseId"), BsonRepresentation(BsonType.ObjectId)]
        public string? CourseId { get; set; }

        [BsonElement("lessonsId"), BsonRepresentation(BsonType.String)]
        public IEnumerable<string> LessonsId { get; set; } = new List<string>();

        [BsonElement("quizId"), BsonRepresentation(BsonType.String)]
        public string? QuizId { get; set; }
    }
}
