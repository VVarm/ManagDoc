using Microsoft.EntityFrameworkCore;

public class EfDocumentRepository : IDocumentRepository
{
    private AppDbContext _context;
    public EfDocumentRepository(AppDbContext appDb)
    {
        _context = appDb;
    }

    public async Task<Document?> GetByIdAsync(Guid id)
    {
        return await _context.Documents.FindAsync(id);
    }

    public async Task AddDocumentAsync(Document document)
    {
        if (document == null) throw new ArgumentNullException(nameof(document));
        await _context.Documents.AddAsync(document);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateDocumentAsync(Document document)
    {
        if (document == null) throw new ArgumentNullException(nameof(document));
        if (await _context.Documents.FirstOrDefaultAsync(o => o.Id == document.Id) == null) throw new InvalidOperationException("Document not found");
        _context.Documents.Update(document);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteDocumentAsync(Guid id)
    {
        var document = await _context.Documents.FindAsync(id);
        if (document == null) throw new InvalidOperationException($"Document with id {id} not found");
        _context.Documents.Remove(document);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Document>> GetAllAsync()
    {
        return await _context.Documents.ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetDocumentsByStatusAsync(DocumentStatus status)
    {
        return await _context.Documents.Where(d => d.Status == status).ToListAsync();
    }

    public async Task<IEnumerable<Document>> SearchDocumentByEmployeeIdAsync(Guid employeeId)
    {
        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee == null) return Enumerable.Empty<Document>();
        return await _context.Documents.Where(d => d.Signatures.Any(s => s.Phone == employee.Phone)).ToListAsync();
    }
}