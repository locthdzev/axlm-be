using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;

namespace Data.Utilities.CloudStorage
{
    public class CloudStorage : ICloudStorage
    {
        private readonly StorageClient _storage;
        private readonly string _bucketName;

        public CloudStorage(CloudStorageSettings settings)
        {
            GoogleCredential credential = GoogleCredential.FromFile(settings.ServiceAccountKeyPath);
            _storage = StorageClient.Create(credential);
            _bucketName = settings.BucketName;
        }

        public async Task<List<string>> UploadFilesToFirebase(List<IFormFile> files, string filePath)
        {
            List<string> uploadedFileNames = new List<string>();

            foreach (var file in files)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var objectName = $"{filePath}/{file.FileName}";
                    _storage.UploadObject(_bucketName, objectName, file.ContentType, memoryStream);

                    uploadedFileNames.Add(file.FileName);
                }
            }

            return uploadedFileNames;
        }

        public async Task<string> UploadOneFileToFirebase(IFormFile file, string filePath)
        {
            string uploadedFileName = null;

            if (file != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var objectName = $"{filePath}/{file.FileName}";
                    _storage.UploadObject(_bucketName, objectName, file.ContentType, memoryStream);

                    uploadedFileName = file.FileName;
                }
            }
            return uploadedFileName;
        }

        public async Task<string> DownloadFileFromFirebase(string fileName, string filePath)
        {
            using (var memoryStream = new MemoryStream())
            {

                string objectName = $"{filePath}/{fileName}";

                await _storage.DownloadObjectAsync(_bucketName, objectName, memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                byte[] fileBytes = memoryStream.ToArray();


                return System.Convert.ToBase64String(fileBytes);
            }
        }

        public IEnumerable<string> ListFilesInFirebase(string classId)
        {
            string prefix = $"class_{classId}/";

            var objects = _storage.ListObjects(_bucketName, null);

            foreach (var obj in objects)
            {
                yield return obj.Name.Replace(prefix, "");
            }
        }

        public async Task RemoveFileFromFirebase(string fileName, string filePath)
        {
            try
            {
                string objectName = $"{filePath}/{fileName}";
                await _storage.DeleteObjectAsync(_bucketName, objectName);
                Console.WriteLine($"File '{fileName}' removed successfully from Firebase Storage.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while removing the file: {ex.Message}");
            }
        }
    }
}