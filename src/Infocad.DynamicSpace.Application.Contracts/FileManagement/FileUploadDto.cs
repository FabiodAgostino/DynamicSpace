using System.ComponentModel.DataAnnotations;

namespace Infocad.DynamicSpace.FileManagement
{
    public class FileUploadDto
    {
        [Required]
        public string FileName { get; set; }

        [Required]
        public string ContentType { get; set; }

        [Required]
        public byte[] Content { get; set; }
    }
}
