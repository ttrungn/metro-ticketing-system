using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using UserService.Application.Common.Interfaces.Services;

namespace UserService.Infrastructure.Services;

public class AzureBlobService : IAzureBlobService
{
    private readonly BlobServiceClient _blobServiceClient;

    public AzureBlobService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<string> UploadAsync(Stream fileStream, string blobName, string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(fileStream, overwrite: true);

        return blobClient.Uri.ToString();
    }

    public async Task<Stream> DownloadAsync(string blobName, string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        var response = await blobClient.DownloadAsync();

        return response.Value.Content;
    }

    public async Task DeleteAsync(string blobName, string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
    }

    public async Task<IEnumerable<string>> ListBlobsAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobs = containerClient.GetBlobsAsync();

        var result = new List<string>();
        await foreach (var blobItem in blobs)
        {
            result.Add(blobItem.Name);
        }

        return result;
    }
}

