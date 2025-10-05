using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BoxExpress.Application.Services;

public class FileService : IFileService
{
    private readonly IConfiguration _configuration;

    public FileService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return "Archivo inválido o vacío.";
        }

        var connectionString = _configuration["BlobStorage:ConnectionString"];
        var containerName = _configuration["BlobStorage:ContainerName"];

        if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(containerName))
        {
            return "Configuración de BlobStorage incompleta.";
        }

        var containerClient = new BlobContainerClient(connectionString, containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

        var extension = Path.GetExtension(file.FileName);
        var uniqueName = $"{Guid.NewGuid():N}{extension}";
        var blobClient = containerClient.GetBlobClient(uniqueName);

        var headers = new BlobHttpHeaders
        {
            ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                ? "application/octet-stream"
                : file.ContentType,
        };

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = headers });
        return uniqueName;
    }

    public async Task<string> GetTempUrlAsync(string blobName, TimeSpan? timeToLive = null)
    {
        if (string.IsNullOrWhiteSpace(blobName))
        {
            return "Nombre de blob inválido.";
        }

        var connectionString = _configuration["BlobStorage:ConnectionString"];
        var containerName = _configuration["BlobStorage:ContainerName"];

        if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(containerName))
        {
            return "Configuración de BlobStorage incompleta.";
        }

        var containerClient = new BlobContainerClient(connectionString, containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var ttl = timeToLive ?? TimeSpan.FromHours(1);
        var expiresOn = DateTimeOffset.UtcNow.Add(ttl);

        if (!blobClient.CanGenerateSasUri)
        {
            return "No es posible generar SAS con las credenciales actuales.";
        }

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
            ExpiresOn = expiresOn,
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        return blobClient.GenerateSasUri(sasBuilder).ToString();
    }
}
