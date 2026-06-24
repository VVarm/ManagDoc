public class DocumentSignedEvent : DomainEvent
{
    public Guid DocumentId { get; }
    public string Phone { get; }

    public DocumentSignedEvent(Guid id, string phone)
    {
        DocumentId = id;
        Phone = phone;
    }
}