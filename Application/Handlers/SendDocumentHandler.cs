public class SendDocumentHandler
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ILogger<SendDocumentHandler> _logger;

    public SendDocumentHandler(IDocumentRepository documentRepository, ILogger<SendDocumentHandler> logger)
    {
        _documentRepository = documentRepository;
        _logger = logger;
    }

    public async Task Handle(SendDocumentCommand command)
    {
        var document = await _documentRepository.GetByIdAsync(command.DocumentId);
        if (document == null) throw new InvalidOperationException("Document not found");
        
        document.Send();
        _logger.LogInformation("Document {DocumentId} sent", document.Id);

        await _documentRepository.UpdateDocumentAsync(document);
    }
}