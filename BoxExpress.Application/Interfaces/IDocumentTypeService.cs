using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Interfaces;

public interface IDocumentTypeService
{
    Task<ApiResponse<IEnumerable<DocumentTypeDto>>> GetAllAsync(DocumentTypeFilterDto filter);
}
