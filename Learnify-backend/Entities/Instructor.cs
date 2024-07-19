using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Learnify_backend.Entities
{
    public class Instructor
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRequired]
        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public string? Name { get; set; }

        [BsonElement("profilePicture"), BsonRepresentation(BsonType.String)]
        public string? ProfilePicture { get; set; }

        [BsonElement("bio"), BsonRepresentation(BsonType.String)]
        public string? Bio { get; set; }

        [BsonElement("qualifications"), BsonRepresentation(BsonType.String)]
        public string? Qualifications { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("createdOn"), BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedOn { get; set; }
    }
}
