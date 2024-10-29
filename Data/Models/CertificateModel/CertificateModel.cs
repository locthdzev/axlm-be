using Microsoft.AspNetCore.Http;

namespace Data.Models.CertificateModel
{
    public class CertificateModel
    {
        public Guid studentId { get; set; }
        public Guid programId { get; set; }
    }

    public class MockFormFile : IFormFile
    {
        private readonly Stream _stream;

        public MockFormFile(Stream stream, long length, long? offset, string name, string fileName)
        {
            _stream = stream;
            Length = length;
            ContentType = "application/octet-stream";
            FileName = fileName;
            Name = name;
        }

        public string? ContentType { get; }
        public string? ContentDisposition { get; }
        public IHeaderDictionary? Headers { get; }
        public long Length { get; }
        public string? Name { get; }
        public string? FileName { get; }
        public void CopyTo(Stream target) => _stream.CopyTo(target);
        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            return _stream.CopyToAsync(target, 4096, cancellationToken);
        }
        public Stream OpenReadStream() => _stream;
    }
}