public class DocumentSignedEventHandler
{
    private readonly ILogger<DocumentSignedEventHandler> _logger;

    public DocumentSignedEventHandler(ILogger<DocumentSignedEventHandler> logger)
    {
        _logger = logger;
    }
    public Task Handle(DocumentSignedEvent evented)
    {
        _logger.LogInformation("Document {DocumentId} signed by {Phone}", evented.DocumentId, evented.Phone);
        return Task.CompletedTask;
    }
}