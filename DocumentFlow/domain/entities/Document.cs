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

    public void Sign(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber)) throw new ArgumentNullException("Телефон не может быть пуст");
        string clean = PhoneHelper.NormalizePhone(phoneNumber);
        if (!PhoneHelper.IsValidPhone(clean))
            throw new ArgumentException($"Номер телефона {phoneNumber} невалиден после нормализации");
        if (Status != DocumentStatus.Sent)
            throw new InvalidOperationException($"Документ '{Title}' не в статусе 'Sent' (текущий: {Status})");
        if (_signatures.Any(s => s.Phone == clean)) 
            throw new InvalidOperationException($"Телефон {clean} уже подписал этот документ");
        _signatures.Add(new Signature(clean));
        Status = DocumentStatus.Signed;
    } 
}