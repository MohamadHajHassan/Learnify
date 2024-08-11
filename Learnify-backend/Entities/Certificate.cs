using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Learnify_backend.Entities
{
    public class Certificate
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRequired]
        [BsonElement("enrollmentId"), BsonRepresentation(BsonType.ObjectId)]
        public string? EnrollmentId { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("issuedOn"), BsonRepresentation(BsonType.DateTime)]
        public DateTime IssuedOn { get; set; } = DateTime.UtcNow;

        [BsonElement("isSent"), BsonRepresentation(BsonType.Boolean)]
        public bool IsSent { get; set; } = false;

        [BsonElement("certificateNumber"), BsonRepresentation(BsonType.String)]
        public string? CertificateNumber { get; set; }
    }
}
