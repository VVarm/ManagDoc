public class SignDocumentHandler
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly DocumentSignedEventHandler _eventHandler;
    private readonly ILogger<SignDocumentHandler> _logger;

    public SignDocumentHandler(IDocumentRepository documentRepository, IEmployeeRepository employeeRepository, DocumentSignedEventHandler eventHandler, ILogger<SignDocumentHandler> logger)
    {
        _documentRepository = documentRepository;
        _employeeRepository = employeeRepository;
        _eventHandler = eventHandler;
        _logger = logger;
    }
    public async Task Handle(SignDocumentCommand command)
    {
        var document = await _documentRepository.GetByIdAsync(command.DocumentId);
        if (document == null) throw new InvalidOperationException("Document not found");

        var employee = await _employeeRepository.GetByPhoneAsync(command.Phone);
        if(employee == null) throw new InvalidOperationException("Employee not found");

        _logger.LogInformation("Attempting to sign document {DocumentId} by {Phone}", document.Id, command.Phone);
        document.Sign(command.Phone);
        await _documentRepository.UpdateDocumentAsync(document);

        var signEvent = new DocumentSignedEvent(document.Id, command.Phone);
        await _eventHandler.Handle(signEvent);
    } 
}