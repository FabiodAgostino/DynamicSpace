using Infocad.DynamicSpace.Feature;
using Shouldly;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Features;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Xunit;

namespace Infocad.DynamicSpace.FileManagement
{
    public abstract class FileManagementAppService_Tests<TStartupModule> : DynamicSpaceApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IFileManagementAppService _fileManagementService;
        private readonly ITenantRepository _tenantRepository;
        private readonly IFeatureChecker _featureChecker;

        protected FileManagementAppService_Tests()
        {
            _fileManagementService = GetRequiredService<IFileManagementAppService>();
            _tenantRepository = GetRequiredService<ITenantRepository>();
            _featureChecker = GetRequiredService<IFeatureChecker>();
        }

        [Fact]
        public async Task Should_Upload_Image_To_FileSystem()
        {
            // Arrange
            var tenant = await CreateTestTenantAsync("Image Upload Tenant");
            await SetCurrentTenantAsync(tenant.Id);

            var imageUpload = CreateImageUploadDto("test-image.jpg");

            // Act
            var result = await _fileManagementService.UploadFileAsync(imageUpload);

            // Assert
            result.ShouldNotBeNull();
            result.FileName.ShouldBe("test-image.jpg");
            result.StorageType.ShouldBe("FileSystem");
            result.ContainerType.ShouldBe("Images");
            result.Size.ShouldBe(imageUpload.Content.Length);
        }

        [Fact]
        public async Task Should_Upload_Document_To_Database_With_Premium()
        {
            // Arrange
            var tenant = await CreateTestTenantAsync("Premium Document Tenant");
            await SetCurrentTenantAsync(tenant.Id);
            await EnableAdvancedFileStorageAsync(tenant.Id);

            var documentUpload = CreateDocumentUploadDto("test-document.pdf");

            // Act
            var result = await _fileManagementService.UploadFileAsync(documentUpload);

            // Assert
            result.ShouldNotBeNull();
            result.FileName.ShouldBe("test-document.pdf");
            result.StorageType.ShouldBe("Database");
            result.ContainerType.ShouldBe("Documents");
            result.Size.ShouldBe(documentUpload.Content.Length);
        }


        [Fact]
        public async Task Should_Download_File_Successfully()
        {
            // Arrange
            var tenant = await CreateTestTenantAsync("Download Tenant");
            await SetCurrentTenantAsync(tenant.Id);

            var uploadDto = CreateImageUploadDto("download-test.png");
            var uploadedFile = await _fileManagementService.UploadFileAsync(uploadDto);

            // Act
            var downloadResult = await _fileManagementService.DownloadFileAsync(uploadedFile.Id);

            // Assert
            downloadResult.ShouldNotBeNull();
            downloadResult.FileName.ShouldBe("download-test.png");
            downloadResult.ContentType.ShouldBe("image/png");
            downloadResult.Content.Length.ShouldBe(uploadDto.Content.Length);
        }

     

        [Fact]
        public async Task Should_Get_File_List()
        {
            // Arrange
            var tenant = await CreateTestTenantAsync("File List Tenant");
            await SetCurrentTenantAsync(tenant.Id);

            await _fileManagementService.UploadFileAsync(CreateImageUploadDto("image1.jpg"));
            await _fileManagementService.UploadFileAsync(CreateImageUploadDto("image2.png"));

            // Act
            var fileList = await _fileManagementService.GetFileListAsync();

            // Assert
            fileList.ShouldNotBeNull();
            fileList.Count.ShouldBeGreaterThanOrEqualTo(2);

            var image1 = fileList.FirstOrDefault(f => f.FileName == "image1.jpg");
            image1.ShouldNotBeNull();
            image1.StorageType.ShouldBe("FileSystem");
            image1.ContainerType.ShouldBe("Images");
        }

        [Fact]
        public async Task Should_Delete_File_Successfully()
        {
            // Arrange
            var tenant = await CreateTestTenantAsync("Delete File Tenant");
            await SetCurrentTenantAsync(tenant.Id);

            var uploadDto = CreateImageUploadDto("to-delete.jpg");
            var uploadedFile = await _fileManagementService.UploadFileAsync(uploadDto);

            // Act
            await _fileManagementService.DeleteFileAsync(uploadedFile.Id);

            // Assert
            var fileExists = await _fileManagementService.FileExistsAsync(uploadedFile.Id);
            fileExists.ShouldBeFalse();
        }

        [Fact]
        public async Task Should_Check_File_Exists()
        {
            // Arrange
            var tenant = await CreateTestTenantAsync("File Exists Tenant");
            await SetCurrentTenantAsync(tenant.Id);

            var uploadDto = CreateImageUploadDto("exists-test.jpg");
            var uploadedFile = await _fileManagementService.UploadFileAsync(uploadDto);

            // Act
            var exists = await _fileManagementService.FileExistsAsync(uploadedFile.Id);
            var notExists = await _fileManagementService.FileExistsAsync(Guid.NewGuid());

            // Assert
            exists.ShouldBeTrue();
            notExists.ShouldBeFalse();
        }

        [Fact]
        public async Task Should_Get_Storage_Statistics()
        {
            // Arrange
            var tenant = await CreateTestTenantAsync("Statistics Tenant");
            await SetCurrentTenantAsync(tenant.Id);
            await EnableAdvancedFileStorageAsync(tenant.Id);

            await _fileManagementService.UploadFileAsync(CreateImageUploadDto("stat-image.jpg"));
            await _fileManagementService.UploadFileAsync(CreateDocumentUploadDto("stat-doc.pdf"));

            // Act
            var stats = await _fileManagementService.GetStorageStatisticsAsync();

            // Assert
            stats.ShouldNotBeNull();
            stats.ShouldContainKey("TotalFiles");
            stats.ShouldContainKey("TotalSize");
            stats.ShouldContainKey("DatabaseFiles");
            stats.ShouldContainKey("FileSystemFiles");
            stats.ShouldContainKey("DocumentsCount");
            stats.ShouldContainKey("ImagesCount");

            ((int)stats["TotalFiles"]).ShouldBeGreaterThanOrEqualTo(2);
            ((int)stats["FileSystemFiles"]).ShouldBeGreaterThanOrEqualTo(1);
            ((int)stats["DatabaseFiles"]).ShouldBeGreaterThanOrEqualTo(1);
        }


        private async Task<Tenant> CreateTestTenantAsync(string name)
        {
            var tenantManager = GetRequiredService<ITenantManager>();
            var tenant = await tenantManager.CreateAsync(name);
            await _tenantRepository.InsertAsync(tenant, autoSave: true);
            return tenant;
        }

        private async Task SetCurrentTenantAsync(Guid tenantId)
        {
            var currentTenant = GetRequiredService<ICurrentTenant>();
            using (currentTenant.Change(tenantId))
            {
                // Context is set for the test
            }
        }

        private async Task EnableAdvancedFileStorageAsync(Guid tenantId)
        {
            var featureManager = GetRequiredService<IFeatureManager>();
            await featureManager.SetForTenantAsync(
                tenantId,
                DynamicSpaceFeatures.AdvancedFileStorage,
                "true"
            );
        }

        private async Task DisableAdvancedFileStorageAsync(Guid tenantId)
        {
            var featureManager = GetRequiredService<IFeatureManager>();
            await featureManager.SetForTenantAsync(
                tenantId,
                DynamicSpaceFeatures.AdvancedFileStorage,
                "false"
            );
        }

        private async Task DisableFileManagementAsync(Guid tenantId)
        {
            var featureManager = GetRequiredService<IFeatureManager>();
            await featureManager.SetForTenantAsync(
                tenantId,
                DynamicSpaceFeatures.FileManagement,
                "false"
            );
        }

        private async Task SetMaxProductCountAsync(Guid tenantId, int count)
        {
            var featureManager = GetRequiredService<IFeatureManager>();
            await featureManager.SetForTenantAsync(
                tenantId,
                DynamicSpaceFeatures.MaxProductCount,
                count.ToString()
            );
        }

        private FileUploadDto CreateImageUploadDto(string fileName)
        {
            var content = new byte[1024]; // 1KB image
            new Random().NextBytes(content);

            return new FileUploadDto
            {
                FileName = fileName,
                ContentType = GetContentType(fileName),
                Content = content
            };
        }

        private FileUploadDto CreateDocumentUploadDto(string fileName)
        {
            var content = new byte[2048]; // 2KB document
            new Random().NextBytes(content);

            return new FileUploadDto
            {
                FileName = fileName,
                ContentType = GetContentType(fileName),
                Content = content
            };
        }

        private FileUploadDto CreateLargeImageUploadDto(string fileName, int sizeInBytes)
        {
            var content = new byte[sizeInBytes];
            new Random().NextBytes(content);

            return new FileUploadDto
            {
                FileName = fileName,
                ContentType = GetContentType(fileName),
                Content = content
            };
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }

    }
}