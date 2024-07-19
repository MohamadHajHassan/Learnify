using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Learnify_backend.Entities
{
    public enum DifficultyLevel
    {
        Beginner,
        Intermediate,
        Advanced
    }
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
        [BsonElement("categories"), BsonRepresentation(BsonType.Array)]
        public IEnumerable<string> Categories { get; set; } = new List<string>();

        [BsonElement("difficultyLevel"), BsonRepresentation(BsonType.String)]
        public DifficultyLevel DifficultyLevel { get; set; }

        [BsonElement("duration"), BsonRepresentation(BsonType.Double)]
        public double? Duration { get; set; }

        [BsonElement("syllabus"), BsonRepresentation(BsonType.String)]
        public string? Syllabus { get; set; }

        [BsonElement("preRequisites"), BsonRepresentation(BsonType.Array)]
        public IEnumerable<ObjectId> PreRequisites { get; set; } = new List<ObjectId>();

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("createdOn"), BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("UpdatedOn"), BsonRepresentation(BsonType.DateTime)]
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;

        [BsonRequired]
        [BsonElement("instructorId"), BsonRepresentation(BsonType.ObjectId)]
        public required string InstructorId { get; set; }

        [BsonElement("reviews"), BsonRepresentation(BsonType.Array)]
        public IEnumerable<Review> Reviews { get; set; } = new List<Review>();

        [BsonElement("averageRating"), BsonRepresentation(BsonType.Double)]
        public double? AverageRating { get; set; }
    }
}
