public class TransferEmployeeCommand
{
    public Guid FromOrganizationId { get; }
    public Guid ToOrganizationId { get; }
    public string Phone { get; }

    public TransferEmployeeCommand(Guid fromOrganizationId, Guid toOrganizationId, string phone)
    {
        FromOrganizationId = fromOrganizationId;
        ToOrganizationId = toOrganizationId;
        Phone = phone;
    }
}