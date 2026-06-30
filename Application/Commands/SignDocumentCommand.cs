public class SignDocumentCommand
{
    public Guid DocumentId { get; }
    public string Phone { get; }

    public SignDocumentCommand(Guid documentId, string phone)
    {
        DocumentId = documentId;
        Phone = phone;
    }
}