using Microsoft.AspNetCore.Mvc;

namespace Learnify_backend.Services.FileService
{
    public interface IFileService
    {
        Task<List<string>> UploadFilesAsync(string Id, IFormFileCollection files);
        Task<IActionResult> DownloadFileAsync(string fileId);
        Task DeleteFilesAsync(IEnumerable<string> filesId);
    }
}
