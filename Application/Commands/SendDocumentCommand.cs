public class SendDocumentCommand
{
    public Guid DocumentId { get; }

    public SendDocumentCommand (Guid id)
    {
        DocumentId = id;
    }
}