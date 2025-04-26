using System.Text.Json.Serialization;

namespace BoxExpress.Application.Dtos.Common;

public class ApiResponse<T>
{
    [JsonPropertyName("success")]
    public bool IsSuccess { get; set; } = true;

    public string? Message { get; set; }
    public T? Data { get; set; }
    public PaginationDto? Pagination { get; set; }

    public static ApiResponse<T> Success(T? data, PaginationDto? pagination = null, string? message = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Data = data,
            Pagination = pagination,
            Message = message
        };
    }

    public static ApiResponse<T> Fail(string message)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Message = message
        };
    }
}
