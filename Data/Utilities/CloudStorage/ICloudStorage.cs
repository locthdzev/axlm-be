using Microsoft.AspNetCore.Http;

namespace Data.Utilities.CloudStorage
{
    public interface ICloudStorage
    {
        Task<List<string>> UploadFilesToFirebase(List<IFormFile> files, string filePath);
        Task<string> UploadOneFileToFirebase(IFormFile file, string filePath);
        Task<string> DownloadFileFromFirebase(string fileName, string filePath);
        IEnumerable<string> ListFilesInFirebase(string classId);
        Task RemoveFileFromFirebase(string fileName, string filePath);
    }
}