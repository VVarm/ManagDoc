public class Document {
    public Guid Id { get; }
    public string Title { get; private set; }
    public DocumentStatus Status { get; private set; }
    private List<Signature> _signatures;
    public IReadOnlyList<Signature> Signatures => _signatures.AsReadOnly();

    public Document(string title)
    {
        Id = Guid.NewGuid();
        Title = title == null ? throw new ArgumentNullException(nameof(title)) : title;
        Status = DocumentStatus.Draft;
        _signatures = new List<Signature>();
    }

    // для EF Core
    private Document()
    {
        Title = string.Empty;
        Status = DocumentStatus.Draft;
        _signatures = new List<Signature>();
    }

    public void UpdateTitle(string newTitle)
    {
        Title = newTitle == null ? throw new ArgumentNullException(nameof(newTitle)) : newTitle;
    }

    public void Send()
    {
        if (Status != DocumentStatus.Draft)
        {
            throw new InvalidOperationException($"Cannot send document in status {Status}");
        }
        Status = DocumentStatus.Sent;
    }

    public void Sign(string phoneNumber, string salt, Guid userId)
    {
        if (string.IsNullOrEmpty(phoneNumber)) throw new ArgumentNullException(nameof(phoneNumber));
        if (string.IsNullOrEmpty(salt)) throw new ArgumentNullException(nameof(salt));
        if(userId == Guid.Empty) throw new ArgumentNullException("User ID cannot be empty", nameof(userId));

        string clean = PhoneHelper.NormalizePhone(phoneNumber);
        if (!PhoneHelper.IsValidPhone(clean))
            throw new ArgumentException($"Invalid phone number: {phoneNumber}");
        if (Status != DocumentStatus.Sent)
            throw new InvalidOperationException($"Document '{Title}' is not in 'Sent' status (current: {Status})");
        
        string hash = HashHelper.Hashing(clean, salt);

        if (_signatures.Any(s => s.Hash == hash)) 
            throw new InvalidOperationException($"User with phone {phoneNumber} has already signed this document");
        
        _signatures.Add(new Signature(userId, hash));
        Status = DocumentStatus.Signed;

        //Вызыв события???
    } 
}