public class Signature {
    public string Phone { get; private set; }
    public DateTime SignedAt { get; private set; }

    public Signature(string phone)
    {
        if (string.IsNullOrEmpty(phone)) throw new ArgumentNullException(nameof(phone));
        string clean = PhoneHelper.NormalizePhone(phone);
        if (!PhoneHelper.IsValidPhone(clean)) throw new ArgumentException("Invalid phone number", nameof(phone));
        Phone = clean;
        SignedAt = DateTime.UtcNow;
    }

    // для EF Core
    private Signature()
    {
        Phone = string.Empty;
        SignedAt = DateTime.UtcNow;
    }
}