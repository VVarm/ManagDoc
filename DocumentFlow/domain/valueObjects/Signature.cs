public class Signature {
    public Guid UserId { get; }
    public string Hash { get; private set; }
    public DateTime SignedAt { get; private set; }

    public Signature(Guid userId, string hash)
    {
        if (userId == Guid.Empty) throw new ArgumentNullException("User ID cannot be empty", nameof(userId));
        if (string.IsNullOrEmpty(hash)) throw new ArgumentNullException(nameof(hash));
        UserId = userId;
        Hash = hash;
        SignedAt = DateTime.UtcNow;
    }

    // для EF Core
    private Signature()
    {
        UserId = Guid.Empty;
        Hash = string.Empty;
        SignedAt = DateTime.UtcNow;
    }
}