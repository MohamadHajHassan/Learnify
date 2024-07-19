using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Learnify_backend.Entities
{
    public class Review
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRequired]
        [BsonElement("userId"), BsonRepresentation(BsonType.ObjectId)]
        public string? UserId { get; set; }

        [BsonRequired]
        [BsonElement("rating"), BsonRepresentation(BsonType.Int32)]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [BsonRequired]
        [BsonElement("text"), BsonRepresentation(BsonType.String)]
        public string? Text { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("createdOn"), BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [BsonRequired]
        [BsonElement("courseId"), BsonRepresentation(BsonType.ObjectId)]
        public string? CourseId { get; set; }
    }
}