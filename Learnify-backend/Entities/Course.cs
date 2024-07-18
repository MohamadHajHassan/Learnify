using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Learnify_backend.Entities
{
    public class Course
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRequired]
        [BsonElement("title"), BsonRepresentation(BsonType.String)]
        public string? Title { get; set; }

        [BsonElement("description"), BsonRepresentation(BsonType.String)]
        public string? Description { get; set; }

        [BsonRequired]
        [BsonElement("category"), BsonRepresentation(BsonType.String)]
        public string? Category { get; set; }

        [BsonElement("difficultyLevel"), BsonRepresentation(BsonType.String)]
        public string? DifficultyLevel { get; set; }

        [BsonElement("duration"), BsonRepresentation(BsonType.Double)]
        public double? Duration { get; set; }

        [BsonElement("syllabus"), BsonRepresentation(BsonType.String)]
        public string? Syllabus { get; set; }

        [BsonElement("preRequisites"), BsonRepresentation(BsonType.String)]
        public List<ObjectId>? PreRequisites { get; set; } = new List<ObjectId>();

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("createdOn"), BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("UpdatedOn"), BsonRepresentation(BsonType.DateTime)]
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;

        public User? Instuctor { get; set; }
    }
}
