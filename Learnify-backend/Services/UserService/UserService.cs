using Learnify_backend.Controllers;
using Learnify_backend.Data;
using Learnify_backend.Entities;
using Learnify_backend.Services.FileService;
using MongoDB.Driver;

namespace Learnify_backend.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<Instructor> _instructors;
        private readonly IFileService _fileService;

        public UserService(MongoDbService mongoDbService, IFileService fileService)
        {
            _instructors = mongoDbService.Database.GetCollection<Instructor>("instructors");
            _fileService = fileService;
        }

        // Instructor
        public async Task<IEnumerable<Instructor>> GetAllInstructorsAsync()
        {
            return await _instructors.Find(FilterDefinition<Instructor>.Empty).ToListAsync();
        }

        public async Task<Instructor> GetInstructorByIdAsync(string id)
        {
            var filter = Builders<Instructor>.Filter.Eq(x => x.Id, id);
            return await _instructors.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Instructor> CreateInstructorAsync(CreateInstructorRequest request)
        {
            var instructor = new Instructor
            {
                Name = request.Name,
                CreatedOn = DateTime.UtcNow
            };

            if (!String.IsNullOrEmpty(request.Bio))
            {
                instructor.Bio = request.Bio;
            }

            if (!String.IsNullOrEmpty(request.Qualifications))
            {
                instructor.Qualifications = request.Qualifications;
            }
            await _instructors.InsertOneAsync(instructor);
            return instructor;
        }

        public async Task<string> UpdateInstructorAsync(string id, UpdateInstructorRequest request)
        {
            var filter = Builders<Instructor>.Filter.Eq(x => x.Id, id);
            var instructor = await _instructors.Find(filter).FirstOrDefaultAsync();
            if (instructor == null)
            {
                return "Not Found";
            }
            if (!String.IsNullOrEmpty(request.Name))
            {
                instructor.Name = request.Name;
            }
            if (!String.IsNullOrEmpty(request.Bio))
            {
                instructor.Bio = request.Bio;
            }
            if (!String.IsNullOrEmpty(request.Qualifications))
            {
                instructor.Qualifications = request.Qualifications;
            }
            if (request.ProfilePicture is not null)
            {
                if (!String.IsNullOrEmpty(instructor.ProfilePictureId))
                {
                    await _fileService.DeleteFilesAsync(new List<string> { instructor.ProfilePictureId });
                }
                var files = new FormFileCollection();
                files.Add(request.ProfilePicture);

                var ProfilePictureList = await _fileService.UploadFilesAsync(id, files);
                instructor.ProfilePictureId = ProfilePictureList[0];
            }
            await _instructors.ReplaceOneAsync(filter, instructor);
            return "Updated";
        }

        public async Task DeleteInstructorAsync(string id)
        {
            await _instructors.DeleteOneAsync(instructor => instructor.Id == id);
        }
    }
}
