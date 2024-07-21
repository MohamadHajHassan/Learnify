using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

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
        [BsonElement("categories"), BsonRepresentation(BsonType.String)]
        public IEnumerable<string> Categories { get; set; } = new List<string>();

        [BsonElement("difficultyLevel"), BsonRepresentation(BsonType.Int32)]
        [Range(1, 3, ErrorMessage = "Difficulty level must be 1(for beginner), 2(for intermediate) or 3(for advanced).")]
        public int DifficultyLevel { get; set; }

        [BsonElement("duration"), BsonRepresentation(BsonType.Double)]
        public double? Duration { get; set; }

        [BsonElement("syllabus"), BsonRepresentation(BsonType.String)]
        public string? Syllabus { get; set; }

        [BsonElement("preRequisites"), BsonRepresentation(BsonType.String)]
        public IEnumerable<string> PreRequisites { get; set; } = new List<string>();

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("createdOn"), BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("UpdatedOn"), BsonRepresentation(BsonType.DateTime)]
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;

        [BsonRequired]
        [BsonElement("instructorId"), BsonRepresentation(BsonType.ObjectId)]
        public required string InstructorId { get; set; }

        [BsonElement("reviewsId"), BsonRepresentation(BsonType.String)]
        public IEnumerable<string> ReviewsId { get; set; } = new List<string>();

        [BsonElement("averageRating"), BsonRepresentation(BsonType.Double)]
        public double? AverageRating { get; set; }
    }
}
