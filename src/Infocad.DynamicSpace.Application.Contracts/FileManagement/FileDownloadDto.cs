namespace Infocad.DynamicSpace.FileManagement
{
    public class FileDownloadDto
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }
}
