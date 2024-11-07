using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using NetNinja.AzureStorageHandler.Models.Commands;
using NetNinja.AzureStorageHandler.Models.Responses;

namespace NetNinja.AzureStorageHandler.Abstractions
{
    public interface IStorageHandler
    {
        Task<AzureBlobReference> UploadIFormFileToBlobAsync(MultiPartFileCommand command);
        Task<AzureBlobReference> Uploadbase64BlobAsync(FileCommandBase64Command command);
        Uri GenerateSasUriFromBlobName(string blobName);
        BlobContainerClient CreateContainer(string containerName);
        Pageable<BlobItem> ListAllBlobs(string containerName = "");
        BlobContainerClient DeleteContainer(string containerName);
    }
};

