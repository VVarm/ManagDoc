public class OrganizationResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public List<Guid>? EmployeeIds { get; set; }
}