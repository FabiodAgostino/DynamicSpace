using Infocad.DynamicSpace.Feature;
using Infocad.DynamicSpace.FileManagement;
using Infocad.DynamicSpace.FileManagement.Infocad.DynamicSpace.Domain.FileManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Features;
using Volo.Abp.MultiTenancy;

namespace Infocad.DynamicSpace.Blob
{
    public class FileManagementAppService : ApplicationService, IFileManagementAppService
    {
        private readonly IBlobContainer<DocumentContainer> _documentContainer;
        private readonly IBlobContainer<ImageContainer> _imageContainer;
        private readonly IRepository<FileInfoEntity, Guid> _fileInfoRepository;
        private readonly IFeatureChecker _featureChecker;
        private readonly ICurrentTenant _currentTenant;

        public FileManagementAppService(
            IBlobContainer<DocumentContainer> documentContainer,
            IBlobContainer<ImageContainer> imageContainer,
            IRepository<FileInfoEntity, Guid> fileInfoRepository,
            IFeatureChecker featureChecker,
            ICurrentTenant currentTenant)
        {
            _documentContainer = documentContainer;
            _imageContainer = imageContainer;
            _fileInfoRepository = fileInfoRepository;
            _featureChecker = featureChecker;
            _currentTenant = currentTenant;
        }

        public async Task<FileInfoDto> UploadFileAsync(FileUploadDto input)
        {
            await ValidateFileManagementAccessAsync();

            var (container, storageType, containerType) = GetContainerByFileType(input.FileName);

            if (_currentTenant.Id.HasValue)
                await ValidateTenantFeatureAccessAsync(storageType, input);

            var blobName = GenerateBlobName(input.FileName);

            await container.SaveAsync(blobName, input.Content, overrideExisting: true);

            var fileInfo = new FileInfoEntity(
                GuidGenerator.Create(),
                input.FileName,
                blobName,
                input.ContentType,
                input.Content.Length,
                CurrentTenant.Id
            );

            fileInfo.CreatedBy = CurrentUser.Id;
            fileInfo.SetProperty("StorageType", storageType);
            fileInfo.SetProperty("ContainerType", containerType);

            var savedFileInfo = await _fileInfoRepository.InsertAsync(fileInfo, true);

            // 🎯 MAPPA AL DTO
            var dto = ObjectMapper.Map<FileInfoEntity, FileInfoDto>(savedFileInfo);
            dto.StorageType = storageType;
            dto.ContainerType = containerType;

            return dto;
        }

        public async Task<FileDownloadDto> DownloadFileAsync(Guid fileId)
        {
            await ValidateFileManagementAccessAsync();

            var fileInfo = await _fileInfoRepository.GetAsync(fileId);
            var storageType = fileInfo.GetProperty<string>("StorageType");

            if (_currentTenant.Id.HasValue)
                await ValidateTenantDownloadAccessAsync(storageType);

            var container = GetContainerByStoredInfo(fileInfo);
            var fileBytes = await container.GetAllBytesAsync(fileInfo.BlobName);

            return new FileDownloadDto
            {
                FileName = fileInfo.FileName,
                ContentType = fileInfo.ContentType,
                Content = fileBytes
            };
        }

        public async Task<List<FileInfoDto>> GetFileListAsync()
        {
            var files = await _fileInfoRepository.GetListAsync();
            var fileDtos = ObjectMapper.Map<List<FileInfoEntity>, List<FileInfoDto>>(files);

            foreach (var dto in fileDtos)
            {
                var fileEntity = files.FirstOrDefault(f => f.Id == dto.Id);
                if (fileEntity != null)
                {
                    dto.StorageType = fileEntity.GetProperty<string>("StorageType") ?? "Unknown";
                    dto.ContainerType = fileEntity.GetProperty<string>("ContainerType") ?? "Unknown";
                }
            }

            return fileDtos;
        }

        public async Task DeleteFileAsync(Guid fileId)
        {
            await ValidateFileManagementAccessAsync();

            var fileInfo = await _fileInfoRepository.GetAsync(fileId);

            var container = GetContainerByStoredInfo(fileInfo);
            await container.DeleteAsync(fileInfo.BlobName);

            await _fileInfoRepository.DeleteAsync(fileId);
        }

        public async Task<bool> FileExistsAsync(Guid fileId)
        {
            var fileInfo = await _fileInfoRepository.FindAsync(fileId);
            if (fileInfo == null) return false;

            var container = GetContainerByStoredInfo(fileInfo);
            return await container.ExistsAsync(fileInfo.BlobName);
        }

        public async Task<Dictionary<string, object>> GetStorageStatisticsAsync()
        {
            var files = await _fileInfoRepository.GetListAsync();

            var stats = new Dictionary<string, object>
            {
                ["TotalFiles"] = files.Count,
                ["TotalSize"] = files.Sum(f => f.Size),
                ["DatabaseFiles"] = files.Count(f => f.GetProperty<string>("StorageType") == "Database"),
                ["FileSystemFiles"] = files.Count(f => f.GetProperty<string>("StorageType") == "FileSystem"),
                ["DocumentsCount"] = files.Count(f => f.GetProperty<string>("ContainerType") == "Documents"),
                ["ImagesCount"] = files.Count(f => f.GetProperty<string>("ContainerType") == "Images")
            };

            return stats;
        }

        private async Task ValidateFileManagementAccessAsync()
        {
            if (!_currentTenant.Id.HasValue)
                return;

            if (!await _featureChecker.IsEnabledAsync(DynamicSpaceFeatures.FileManagement))
                throw new UserFriendlyException("La gestione file non è abilitata per questo tenant.");
        }

        private async Task ValidateTenantFeatureAccessAsync(string storageType, FileUploadDto input)
        {
            var hasAdvancedStorage = await _featureChecker.IsEnabledAsync(DynamicSpaceFeatures.AdvancedFileStorage);

            if (storageType == "Database" && !hasAdvancedStorage)
                throw new UserFriendlyException("Il salvataggio sicuro dei documenti è disponibile solo con il pacchetto Premium.");

            if (!hasAdvancedStorage)
                await ValidateBasicLimitsAsync(input);
        }

        private async Task ValidateTenantDownloadAccessAsync(string storageType)
        {
            var hasAdvancedStorage = await _featureChecker.IsEnabledAsync(DynamicSpaceFeatures.AdvancedFileStorage);

            if (storageType == "Database" && !hasAdvancedStorage)
                throw new UserFriendlyException("Il download di documenti dal database è disponibile solo con il pacchetto Premium.");
        }

        private async Task ValidateBasicLimitsAsync(FileUploadDto input)
        {
            var currentFiles = await _fileInfoRepository.GetCountAsync();
            var maxFiles = await _featureChecker.GetOrNullAsync(DynamicSpaceFeatures.MaxProductCount) ?? "50";

            if (currentFiles >= int.Parse(maxFiles))
                throw new UserFriendlyException($"Limite file raggiunto ({maxFiles}). Passa al Premium per più spazio.");

            if (input.Content.Length > 5 * 1024 * 1024)
                throw new UserFriendlyException("Con il pacchetto Basic puoi caricare file fino a 5MB. Passa al Premium per file più grandi.");

            var extension = Path.GetExtension(input.FileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".svg" };

            if (!allowedExtensions.Contains(extension))
                throw new UserFriendlyException("Con il pacchetto Basic puoi caricare solo immagini. Passa al Premium per documenti.");
        }

        private (IBlobContainer container, string storageType, string containerType) GetContainerByFileType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension switch
            {
                ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".webp" or ".svg" =>
                    (_imageContainer, "FileSystem", "Images"),
                _ =>
                    (_documentContainer, "Database", "Documents") // Default per documenti
            };
        }

        private IBlobContainer GetContainerByStoredInfo(FileInfoEntity fileInfo)
        {
            var containerType = fileInfo.GetProperty<string>("ContainerType");

            return containerType switch
            {
                "Images" => _imageContainer,
                "Documents" => _documentContainer,
                _ => _documentContainer // Default
            };
        }

        private string GenerateBlobName(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var dateFolder = DateTime.UtcNow.ToString("yyyy/MM/dd");
            return $"{dateFolder}/{Guid.NewGuid()}{extension}";
        }

        public async Task<bool> HasAdvancedStorageAccessAsync()
        {
            if (!_currentTenant.Id.HasValue)
                return true;

            return await _featureChecker.IsEnabledAsync(DynamicSpaceFeatures.AdvancedFileStorage);
        }

    }

}