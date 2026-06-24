public class TransferEmployeeHandler
{
    IOrganizationRepository _organizationRepository;
    IEmployeeRepository _employeeRepository;

    public TransferEmployeeHandler (IOrganizationRepository organizationRepository, IEmployeeRepository employeeRepository)
    {
        _organizationRepository = organizationRepository;
        _employeeRepository = employeeRepository;
    }

    public async Task Handle(TransferEmployeeCommand command)
    {
        var fromOrganization = await _organizationRepository.GetByIdAsync(command.FromOrganizationId);
        if (fromOrganization == null) throw new InvalidOperationException("From organization not found");

        var toOrganization = await _organizationRepository.GetByIdAsync(command.ToOrganizationId);
        if (toOrganization == null) throw new InvalidOperationException("To organization not found");

        var employee = await _employeeRepository.GetByPhoneAsync(command.Phone);
        if(employee == null) throw new InvalidOperationException("Employee not found");

        if(fromOrganization.HasEmployee(employee.Id) && !toOrganization.HasEmployee(employee.Id))
        {
            fromOrganization.RemoveEmployee(employee.Id);
            toOrganization.AddEmployee(employee.Id);
            await _organizationRepository.UpdateOrganizationAsync(fromOrganization);
            await _organizationRepository.UpdateOrganizationAsync(toOrganization);
        }
        else throw new InvalidOperationException("Employee cannot be transferred");
    }
}