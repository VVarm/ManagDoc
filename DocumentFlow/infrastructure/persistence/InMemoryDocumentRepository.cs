public class InMemoryDocumentRepository : IDocumentRepository {
    List<Document> _storage;
    List<Employee> _storageEmployee;

    public InMemoryDocumentRepository() {
        _storage = new List<Document>();
        _storageEmployee = new List<Employee>();
    }

    Task<Document?> IDocumentRepository.GetByIdAsync(Guid id)
    {
        return Task.FromResult(_storage.Find(doc => doc.Id == id));
    }

    Task IDocumentRepository.AddDocumentAsync(Document document)
    {
        _storage.Add(document == null ? throw new ArgumentNullException(nameof(document)) : document);
        return Task.CompletedTask;
    }

    Task IDocumentRepository.UpdateDocumentAsync(Document document)
    {
        int index = _storage.FindIndex(d => d.Id == document.Id);
        if (index >= 0) _storage[index] = document;
        else throw new InvalidOperationException("Document not found");
        return Task.CompletedTask;
    }

    Task IDocumentRepository.DeleteDocumentAsync(Guid id)
    {
        int removed = _storage.RemoveAll(doc => doc.Id == id);
        if (removed == 0)
            throw new InvalidOperationException($"Document with id {id} not found");
        return Task.CompletedTask;
    }

    Task<IEnumerable<Document>> IDocumentRepository.GetAllAsync()
    {
        return Task.FromResult(_storage.AsEnumerable());
    }

    Task<IEnumerable<Document>> IDocumentRepository.GetDocumentsByStatusAsync(DocumentStatus status)
    {
        return Task.FromResult(_storage.Where(doc => doc.Status == status).AsEnumerable());
    }

    Task<IEnumerable<Document>> IDocumentRepository.SearchDocumentByEmployeeIdAsync(Guid employeeId)
    {
        var employee = _storageEmployee.Find(doc => doc.Id == employeeId);
        if (employee == null) throw new InvalidOperationException("Employee not found");
        return Task.FromResult(_storage.Where(d => d.Signatures.Any(s => s.Phone == employee.Phone)).AsEnumerable());
    }
}