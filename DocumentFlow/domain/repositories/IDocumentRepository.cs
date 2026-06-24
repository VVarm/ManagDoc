public interface IDocumentRepository {
    Task<Document?> GetByIdAsync(Guid id);
    Task AddDocumentAsync(Document document);
    Task UpdateDocumentAsync(Document document);
    Task DeleteDocumentAsync(Guid id);
    Task<IEnumerable<Document>> GetAllAsync();
    Task<IEnumerable<Document>> GetDocumentsByStatusAsync(DocumentStatus status);
    Task<IEnumerable<Document>> SearchDocumentByEmployeeIdAsync(Guid employeeId);
}