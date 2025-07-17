using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Infocad.DynamicSpace.FileManagement
{
    public interface IFileManagementAppService : IApplicationService
    {
        Task<FileInfoDto> UploadFileAsync(FileUploadDto input);
        Task<FileDownloadDto> DownloadFileAsync(Guid fileId);
        Task<List<FileInfoDto>> GetFileListAsync();
        Task DeleteFileAsync(Guid fileId);
        Task<bool> FileExistsAsync(Guid fileId);
        Task<Dictionary<string, object>> GetStorageStatisticsAsync();

    }
}
