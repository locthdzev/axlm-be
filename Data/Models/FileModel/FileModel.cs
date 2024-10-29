namespace Data.Models.FileModel
{
    public class ExcelFile
    {
        public Stream FileStream { get; set; }
    }

    public class ExcelResultDTO
    {
        public byte[] FileContent { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}