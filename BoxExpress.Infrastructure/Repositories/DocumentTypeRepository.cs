using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class DocumentTypeRepository : Repository<DocumentType>, IDocumentTypeRepository
{
    private readonly BoxExpressDbContext _context;

    public DocumentTypeRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }
}