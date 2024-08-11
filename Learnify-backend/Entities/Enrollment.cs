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
        public double Progress { get; set; } = 0.0;

        [BsonElement("completedModulesId"), BsonRepresentation(BsonType.String)]
        public IEnumerable<string> CompletedModulesId { get; set; } = new List<string>();

        [BsonElement("courceCompletedQuizzesId"), BsonRepresentation(BsonType.String)]
        public IEnumerable<string> CourseCompletedQuizzesId { get; set; } = new List<string>();

        [BsonElement("gradesId"), BsonRepresentation(BsonType.String)]
        public IEnumerable<string> GradesId { get; set; } = new List<string>();

        [BsonElement("finalGrade"), BsonRepresentation(BsonType.Double)]
        public double? FinalGrade { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("finishedOn"), BsonRepresentation(BsonType.DateTime)]
        public DateTime? FinishedOn { get; set; }

        [BsonElement("isCompleted"), BsonRepresentation(BsonType.Boolean)]
        public bool IsCompleted { get; set; } = false;

        [BsonElement("isDropped"), BsonRepresentation(BsonType.Boolean)]
        public bool IsDropped { get; set; } = false;

        [BsonElement("modulesProgress"), BsonRepresentation(BsonType.Document)]
        public Dictionary<string, ModuleProgress> ModuleProgress { get; set; } = new Dictionary<string, ModuleProgress>();
    }
    public class ModuleProgress
    {
        [BsonElement("moduleId"), BsonRepresentation(BsonType.ObjectId)]
        public required string ModuleId { get; set; }

        [BsonElement("moduleCompletedLessonsId"), BsonRepresentation(BsonType.String)]
        public IEnumerable<string> ModuleCompletedLessonsId { get; set; } = new List<string>();

        [BsonElement("moduleCompletedQuizId"), BsonRepresentation(BsonType.ObjectId)]
        public string? ModuleCompletedQuizId { get; set; }

        [BsonElement("moduleGrade"), BsonRepresentation(BsonType.Double)]
        public double? ModuleGrade { get; set; }

        [BsonElement("isCompleted"), BsonRepresentation(BsonType.Boolean)]
        public bool IsCompleted { get; set; } = false;
    }
}
