using Microsoft.Net.Http.Headers;

public class Organization {
    public Guid Id { get; }
    public string Name { get; private set; }
    public List<Guid> EmployeeIds { get; private set; }

    public Organization(string name)
    {
        Id = Guid.NewGuid();
        Name = name == null ? throw new ArgumentNullException(nameof(name)) : name;
        EmployeeIds = new List<Guid>();
    }

    // для EF Core
    private Organization()
    {
        EmployeeIds = new List<Guid>();
        Name = string.Empty;
    }

    public void UpdateName(string newName)
    {
        Name = newName == null ? throw new ArgumentNullException(nameof(newName)) : newName;
    }

    public void AddEmployee(Guid employeeId)
    {
        if (EmployeeIds.Contains(employeeId)) throw new InvalidOperationException($"Employee with id {employeeId} already exists in the organization");
        EmployeeIds.Add(employeeId);
    }
    public void RemoveEmployee(Guid employeeId)
    {
        bool removed = EmployeeIds.Remove(employeeId);
        if (removed == false)
            throw new InvalidOperationException($"Employee with id {employeeId} not found");
    }
    public bool HasEmployee(Guid employeeId)
    {
        if (EmployeeIds.Contains(employeeId)) return true;
        return false;
    }
}