﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Learnify_backend.Entities
{
    public class User
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRequired]
        [BsonElement("email"), BsonRepresentation(BsonType.String)]
        public string? Email { get; set; }

        [BsonRequired]
        [BsonElement("password"), BsonRepresentation(BsonType.String)]
        public string? Password { get; set; }

        [BsonElement("firstName"), BsonRepresentation(BsonType.String)]
        public string? FirstName { get; set; }

        [BsonElement("lastName"), BsonRepresentation(BsonType.String)]
        public string? LastName { get; set; }

        [BsonElement("profilePictureId"), BsonRepresentation(BsonType.ObjectId)]
        public string? ProfilePictureId { get; set; }

        [BsonElement("role"), BsonRepresentation(BsonType.String)]
        public string? Role { get; set; }

        [BsonElement("isEmailConfirmed"), BsonRepresentation(BsonType.Boolean)]
        public bool IsEmailConfirmed { get; set; } = false;

        [BsonElement("emailConfirmationToken"), BsonRepresentation(BsonType.String)]
        public string? EmailConfirmationToken { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("createdOn"), BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
