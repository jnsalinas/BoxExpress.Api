using BoxExpress.Application.Dtos.Common;
using Microsoft.AspNetCore.Http;

namespace BoxExpress.Application.Interfaces;

public interface IFileService
{
    // Devuelve el nombre del blob almacenado
    Task<string> UploadFileAsync(IFormFile file);

    // Genera una URL SAS temporal para lectura
    Task<string> GetTempUrlAsync(string blobName, TimeSpan? timeToLive = null);
}
