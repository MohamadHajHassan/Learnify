using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Learnify_backend.Entities
{
    public class Lesson
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRequired]
        [BsonElement("title"), BsonRepresentation(BsonType.String)]
        public string? Title { get; set; }

        [BsonRequired]
        [BsonElement("ordre"), BsonRepresentation(BsonType.Int32)]
        public int Ordre { get; set; }

        [BsonRequired]
        [BsonElement("moduleId"), BsonRepresentation(BsonType.ObjectId)]
        public string? ModuleId { get; set; }

        [BsonElement("textContent"), BsonRepresentation(BsonType.String)]
        public string? TextContent { get; set; }

        [BsonElement("imagesURL"), BsonRepresentation(BsonType.String)]
        public IEnumerable<string> ImagesURL { get; set; } = new List<string>();

        [BsonElement("videoURL"), BsonRepresentation(BsonType.String)]
        public string? VideoURL { get; set; }
    }
}
