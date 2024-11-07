using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using FluentValidation;
using FluentValidation.Results;
using NetNinja.AzureStorageHandler.Abstractions;
using NetNinja.AzureStorageHandler.Models.Commands;
using NetNinja.AzureStorageHandler.Configurations;
using NetNinja.AzureStorageHandler.Models.Responses;
using NetNinja.AzureStorageHandler.Validators;

namespace NetNinja.AzureStorageHandler.Services
{
    public class StorageHandler : IStorageHandler
    {
        private readonly StorageConfiguration _storageConfiguration;
        private readonly BlobServiceClient _blobServiceClient;

        public StorageHandler(
            StorageConfiguration storageConfiguration,
            BlobServiceClient blobServiceClient
            )
        {
            _storageConfiguration = storageConfiguration;
            _blobServiceClient = blobServiceClient;
        }

        #region Container Methods

        public BlobContainerClient CreateContainer(string containerName)
        {
            ContainerCreateValidator validator = new ContainerCreateValidator();
            
            ValidationResult results = validator.Validate(containerName);

            if (!results.IsValid)
            {
                throw new ValidationException(results.Errors);
            }

            BlobContainerClient container = new BlobContainerClient(_storageConfiguration.ConnectionString , containerName);
            
            container.Create();

            return container;
        }

        public BlobContainerClient DeleteContainer(string containerName)
        {
            BlobContainerClient container = new BlobContainerClient(_storageConfiguration.ConnectionString , containerName);
            try
            {
                container.Delete();

                return container;
            }
            catch (RequestFailedException ex)when (ex.ErrorCode == BlobErrorCode.ContainerBeingDeleted || ex.ErrorCode == BlobErrorCode.ContainerNotFound)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        #endregion
        
        #region Blob Methods

        public async Task<AzureBlobReference> UploadIFormFileToBlobAsync(MultiPartFileCommand command)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_storageConfiguration.ContainerName);
            
            var blobClient = blobContainerClient.GetBlobClient(command.Name);

            using (var stream = command.File.OpenReadStream())
            {
                await UploadFileAsync(blobClient, stream, command.Metadata);
            }

            var sasUri = GenerateSasUriFromBlobName(blobClient.Name);
            
            var blobReference = BuildBlobReference(blobContainerClient, blobClient, sasUri, command.Name);

            return blobReference;
        }

        public async Task<AzureBlobReference> Uploadbase64BlobAsync(FileCommandBase64Command command)
        {
            byte[] fileBytes = Convert.FromBase64String(command.File);

            using var stream = new MemoryStream(fileBytes);

            var targetContainer = ReturnTargetContainer(command.ContainerName);

            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(targetContainer);

            var blobClient = blobContainerClient.GetBlobClient(command.Name);

            await UploadFileAsync(blobClient, stream, command.Metadata);

            var sasUri = GenerateSasUriFromBlobName(blobClient.Name);

            var blobReference = BuildBlobReference(blobContainerClient, blobClient, sasUri, command.Name);

            return blobReference;
        }

        public Uri GenerateSasUriFromBlobName(string blobName)
        {
            var blobContainerClient = new BlobContainerClient(_storageConfiguration.ConnectionString, _storageConfiguration.ContainerName);

            var blobClient = blobContainerClient.GetBlobClient(blobName);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _storageConfiguration.ContainerName,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(_storageConfiguration.ExpiresOnInHours)
            };
            
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(sasBuilder);
        }

        public Pageable<BlobItem> ListAllBlobs( string containerName = "" )
        {
            var targetContainer = ReturnTargetContainer(containerName);
            
            BlobContainerClient containerClient = new BlobContainerClient(_storageConfiguration.ConnectionString , targetContainer);

            Pageable<BlobItem>? blobs = containerClient.GetBlobs();

            if (blobs == null) throw new Exception("container does not exists");

            return blobs;
        }

        #endregion

        #region Private Methods

        private AzureBlobReference BuildBlobReference( BlobContainerClient blobContainerClient, BlobClient blobClient, Uri sasUri, string fileName )
        {
            DateTime sasExpiryTime = DateTimeOffset.UtcNow.AddHours(_storageConfiguration.ExpiresOnInHours).UtcDateTime;
            
            return new AzureBlobReference
            {
                Container = blobContainerClient.Name,
                Name = blobClient.Name,
                Uri = blobClient.Uri.ToString(),
                SasUri = sasUri.ToString(),
                SasExpires = sasExpiryTime,
                Extension = Path.GetExtension(fileName),
                Metadata = new Dictionary<string, string>()
            };
        }

        private async Task UploadFileAsync(BlobClient blobClient, Stream stream, Dictionary<string,string> metadata, bool overwright = true)
        {
            await blobClient.UploadAsync(stream, overwrite: true);

            await blobClient.SetMetadataAsync(metadata);
        }

        private string ReturnTargetContainer(string containerName = "")
        {
            return string.IsNullOrEmpty(containerName) ? _storageConfiguration.ContainerName : containerName; 
        }
        
        #endregion
    }
};

