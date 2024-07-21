using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Learnify_backend.Entities
{
    public class Enrollment
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRequired]
        [BsonElement("userId"), BsonRepresentation(BsonType.ObjectId)]
        public string? UserId { get; set; }

        [BsonRequired]
        [BsonElement("courseId"), BsonRepresentation(BsonType.ObjectId)]
        public string? CourseId { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("enrolledOn"), BsonRepresentation(BsonType.DateTime)]
        public DateTime EnrolledOn { get; set; } = DateTime.UtcNow;

        [BsonElement("progress"), BsonRepresentation(BsonType.Double)]
        public double Progress { get; set; } = 0;

        [BsonElement("completedModulesId"), BsonRepresentation(BsonType.String)]
        public IEnumerable<string> CompletedModulesId { get; set; } = new List<string>();

        [BsonElement("gradesId"), BsonRepresentation(BsonType.String)]
        public IEnumerable<string> GradesId { get; set; } = new List<string>();
    }
}
