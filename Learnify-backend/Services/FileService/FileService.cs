using Learnify_backend.Services.MongoDbService;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace Learnify_backend.Services.FileService
{
    public class FileService : IFileService
    {
        private readonly GridFSBucket _gridFSBucket;

        public FileService(IMongoDbService mongoDbService)
        {
            _gridFSBucket = new GridFSBucket(mongoDbService.Database);
        }

        public async Task<List<string>> UploadFilesAsync(string Id, IFormFileCollection files)
        {
            var filesId = new List<string>();

            foreach (var file in files)
            {
                var options = new GridFSUploadOptions
                {
                    Metadata = new BsonDocument { { "contentType", file.ContentType } }
                };

                using (var stream = file.OpenReadStream())
                {
                    var uploadResult = await _gridFSBucket.UploadFromStreamAsync($"{Id}_{Guid.NewGuid()}", stream, options);
                    filesId.Add(uploadResult.ToString());
                }
            }

            return filesId;
        }

        public async Task<IActionResult> DownloadFileAsync(string fileId)
        {
            var downloadStream = await _gridFSBucket.OpenDownloadStreamAsync(new ObjectId(fileId));
            if (downloadStream == null)
            {
                return new NotFoundResult();
            }
            var contentType = downloadStream.FileInfo.Metadata.GetValue("contentType").AsString;

            return new FileStreamResult(downloadStream, contentType);
        }

        public async Task DeleteFilesAsync(IEnumerable<string> filesId)
        {
            foreach (var fileId in filesId)
            {
                await _gridFSBucket.DeleteAsync(new ObjectId(fileId));
            }
        }
    }
}
