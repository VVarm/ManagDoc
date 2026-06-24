public class TransferEmployeeRequest
{
    public Guid FromOrganizationId { get; set; }
    public Guid ToOrganizationId { get; set; }
    public string? Phone { get; set; }
}