public class DocumentResponse
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Status { get; set; }
    public List<SignatureDto>? Signatures { get; set; }
}