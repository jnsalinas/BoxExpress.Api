using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Services;

public class DocumentTypeService : IDocumentTypeService
{
    private readonly IDocumentTypeRepository _repository;
    private readonly IMapper _mapper;

    public DocumentTypeService(IDocumentTypeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<DocumentTypeDto>>> GetAllAsync(DocumentTypeFilterDto filter) =>
         ApiResponse<IEnumerable<DocumentTypeDto>>.Success(_mapper.Map<List<DocumentTypeDto>>(await _repository.GetAllAsync()));
}
