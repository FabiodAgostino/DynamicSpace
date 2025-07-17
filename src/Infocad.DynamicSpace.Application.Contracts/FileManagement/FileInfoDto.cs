using System;
using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.FileManagement
{
    public class FileInfoDto : EntityDto<Guid>
    {
        public string FileName { get; set; }
        public string BlobName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }

        public string StorageType { get; set; } // "Database" o "FileSystem"
        public string ContainerType { get; set; } // "Documents", "Images"

        public string FormattedSize => FormatFileSize(Size);

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
