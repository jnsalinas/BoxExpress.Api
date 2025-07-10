using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentTypesController : ControllerBase
{
    private readonly IDocumentTypeService _DocumentTypeService;

    public DocumentTypesController(IDocumentTypeService DocumentTypeService)
    {
        _DocumentTypeService = DocumentTypeService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] DocumentTypeFilterDto filter)
    {
        var result = await _DocumentTypeService.GetAllAsync(filter);
        return Ok(result);
    }
}
