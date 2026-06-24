using Microsoft.AspNetCore.Mvc;
using Serilog;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController(IDocumentRepository documentRepository, SendDocumentHandler sendDocumentHandler, SignDocumentHandler signDocumentHandler, ILogger<DocumentsController> logger) : ControllerBase
{
    IDocumentRepository _documentRepository = documentRepository;
    SendDocumentHandler _sendDocumentHandler = sendDocumentHandler;
    SignDocumentHandler _signDocumentHandler = signDocumentHandler;
    ILogger<DocumentsController> _logger = logger;

    /// <summary>
    /// Creates a new document
    /// </summary>
    /// <param name="request">Document title.</param>
    /// <returns>Created document with ID and status.</returns>
    /// <response code="201">Document created sucessfully.</response>
    /// <response code="400">Invalid request (validation faild).</response>
    [HttpPost]
    public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentRequest request)
    {
        _logger.LogInformation("Creating document with title {Title}", request.Title);
        var document = new Document(request.Title);
        await _documentRepository.AddDocumentAsync(document);
        return CreatedAtAction(nameof(GetDocument), new { id = document.Id }, MapToResponse(document));
    }

    /// <summary>
    /// Retrieves a document by its ID.
    /// </summary>
    /// <param name="id">Document ID.</param>
    /// <returns>Document details.</returns>
    /// <response code="200">Document found.</response>
    /// <response code="404">Document not found.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDocument(Guid id) {
        _logger.LogInformation("Requesting document {DocumentId}", id);
        var document = await _documentRepository.GetByIdAsync(id);
        if (document == null) return NotFound("Document not found");
        return Ok(MapToResponse(document));
    }

    /// <summary>
    /// Sends a document (changes status from Draft to Sent).
    /// </summary>
    /// <param name="id">Document ID.</param>
    /// <returns>Operation result.</returns>
    /// <response code="200">Document sent successfully.</response>
    /// <response code="404">Document not found.</response>
    /// <response code="400">Invalid operation (e.g., document already sent).</response>
    [HttpPost("{id}/send")]
    public async Task<IActionResult> SendDocument(Guid id)
    {
        _logger.LogInformation("Sending document {DocumentId}", id);
        var command = new SendDocumentCommand(id);
        await _sendDocumentHandler.Handle(command);
        return Ok("Document sended");
    }

    /// <summary>
    /// Signs a document by a employee phone number.
    /// </summary>
    /// <param name="id">Document ID.</param>
    /// <param name="request">Phone number.</param>
    /// <returns>Operation result.</returns>
    /// <response code="200">Document signed successfully.</response>
    /// <response code="404">Document or employee not found.</response>
    /// <response code="400">Invalid request (phone validation failed).</response>
    [HttpPost("{id}/sign")]
    public async Task<IActionResult> SignDocument(Guid id, [FromBody] SignDocumentRequest request)
    {
        _logger.LogInformation("Sign request for document {DocumentId} by phone {Phone}", id, request.Phone);
        var command = new SignDocumentCommand(id, request.Phone);
        await _signDocumentHandler.Handle(command);
        return Ok("Document signed");
    }

    /// <summary>
    /// Returns a paginated list of documents with optional status filter.
    /// </summary>
    /// <param name="page">Page number (default 1).</param>
    /// <param name="pageSize">Items per page (default 10).</param>
    /// <param name="status">Optional status filter (Draft, Sent, Signed).</param>
    /// <returns>Paginated list of documents.</returns>
    /// <response code="200">List returned successfully.</response>
    /// <response code="400">Invalid pagination parameters.</response>
    [HttpGet]
    public async Task<IActionResult> GetPageDocuments([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] DocumentStatus? status = null)
    {
        _logger.LogInformation("Fetching documents with page={Page}, pageSize={PageSize}, status={Status}", page, pageSize, status);
        if (page < 1 || pageSize < 1) return BadRequest("Page and pageSize must be greater than 0");
        IEnumerable<Document> documents = status.HasValue
                    ? await _documentRepository.GetDocumentsByStatusAsync(status.Value)
                    : await _documentRepository.GetAllAsync();
        return Ok(new PagedResponse<DocumentResponse>
        {
            TotalCount = documents.Count(),
            Page = page,
            PageSize = pageSize,
            Items = documents.Skip((page - 1) * pageSize).Take(pageSize).Select(MapToResponse).ToList()
        });
    }

    /// <summary>
    /// Searches documents signed by a specific employee.
    /// </summary>
    /// <param name="employeeId">Employee ID.</param>
    /// <returns>List of documents signed by the employee.</returns>
    /// <response code="200">Search completed successfully.</response>
    [HttpGet("search")]
    public async Task<IActionResult> SearchEmployeeId(Guid employeeId)
    {
        _logger.LogInformation("Searching documents by employee {EmployeeId}", employeeId);
        IEnumerable<Document> listDocuments;
        listDocuments = await _documentRepository.SearchDocumentByEmployeeIdAsync(employeeId);
        return Ok(listDocuments.Select(MapToResponse).ToList());
        
    }

    /// <summary>
    /// Deletes a document by its ID.
    /// </summary>
    /// <param name="id">Document ID.</param>
    /// <returns>No content.</returns>
    /// <response code="204">Document deleted successfully.</response>
    /// <response code="404">Document not found.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDocument(Guid id)
    {
        _logger.LogInformation("Deleting document {DocumentId}", id);
        await _documentRepository.DeleteDocumentAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Updates the title of an existing document.
    /// </summary>
    /// <param name="id">Document ID.</param>
    /// <param name="request">New title.</param>
    /// <returns>Updated document.</returns>
    /// <response code="200">Document updated successfully.</response>
    /// <response code="404">Document not found.</response>
    /// <response code="400">Invalid request (title validation failed).</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDocument(Guid id, [FromBody] UpdateDocumentRequest request)
    {
        _logger.LogInformation("Updating document {DocumentId} to title {Title}", id, request.Title);
        var document = await _documentRepository.GetByIdAsync(id);
        if (document == null)
            return NotFound("Document not found");
        document.UpdateTitle(request.Title);
        await _documentRepository.UpdateDocumentAsync(document);
        return Ok(MapToResponse(document));
    }

    private static DocumentResponse MapToResponse(Document document)
    {
        DocumentResponse response = new();
        response.Id = document.Id;
        response.Title = document.Title;
        response.Status = document.Status.ToString();
        response.Signatures = document.Signatures.Select(s => new SignatureDto { Phone = s.Phone, SignedAt = s.SignedAt }).ToList();
        return response;
    }
}