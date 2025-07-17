using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Infocad.DynamicSpace.FileManagement
{
    public class FileInfoEntity : AggregateRoot<Guid>, IMultiTenant
    {
        public string FileName { get; set; }
        public string BlobName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? TenantId { get; set; }

        protected FileInfoEntity()
        {
        }

        public FileInfoEntity(
            Guid id,
            string fileName,
            string blobName,
            string contentType,
            long size,
            Guid? tenantId = null) : base(id)
        {
            FileName = fileName;
            BlobName = blobName;
            ContentType = contentType;
            Size = size;
            CreatedDate = DateTime.UtcNow;
            TenantId = tenantId;
        }
    }
}
