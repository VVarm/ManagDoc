public class InMemoryEmployeeRepository : IEmployeeRepository {
    List<Employee> _storage;

    public InMemoryEmployeeRepository()
    {
        _storage = new List<Employee>();
    }

    Task<Employee?> IEmployeeRepository.GetByIdAsync(Guid id)
    {
        return Task.FromResult(_storage.Find(doc => doc.Id == id));
    }

    Task<Employee?> IEmployeeRepository.GetByPhoneAsync(string phone)
    {
        string clean = PhoneHelper.NormalizePhone(phone);
        if (PhoneHelper.IsValidPhone(clean))
        return Task.FromResult(_storage.Find(doc => doc.Phone == clean));
        else throw new ArgumentException("Invalid phone number", nameof(phone));
    }

    Task IEmployeeRepository.AddEmployeeAsync(Employee employee)
    {
        _storage.Add(employee == null ? throw new ArgumentNullException(nameof(employee)) : employee);
        return Task.CompletedTask;
    }

    Task IEmployeeRepository.UpdateEmployeeAsync(Employee employee)
    {
        int index = _storage.FindIndex(d => d.Id == employee.Id);
        if (index >= 0) _storage[index] = employee;
        else throw new InvalidOperationException("Employee not found");
        return Task.CompletedTask;
    }

    Task IEmployeeRepository.DeleteEmployeeAsync(Guid id)
    {
        int removed = _storage.RemoveAll(doc => doc.Id == id);
        if (removed == 0)
            throw new InvalidOperationException($"Employee with id {id} not found");
        return Task.CompletedTask;
    }

    Task<IEnumerable<Employee>> IEmployeeRepository.GetAllAsync()
    {
        return Task.FromResult(_storage.AsEnumerable());
    }
}