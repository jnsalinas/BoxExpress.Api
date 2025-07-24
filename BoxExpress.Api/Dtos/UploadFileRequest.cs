using Microsoft.AspNetCore.Http;

namespace BoxExpress.Api.Dtos.Upload;

public class UploadFileRequest
{
    public IFormFile File { get; set; } = null!;
    public int? StoreId { get; set; }
}