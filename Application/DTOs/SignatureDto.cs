public class SignatureDto
{
    public Guid? UserId { get; set; }
    public string? Hash { get; set; }
    public DateTime? SignedAt { get; set; }
}