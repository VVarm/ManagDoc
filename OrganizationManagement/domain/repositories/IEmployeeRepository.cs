public interface IEmployeeRepository {
    Task<Employee?> GetByIdAsync(Guid id);
    Task<Employee?> GetByPhoneAsync(string phone);
    Task AddEmployeeAsync(Employee employee);
    Task UpdateEmployeeAsync(Employee employee);
    Task DeleteEmployeeAsync(Guid id);
    Task<IEnumerable<Employee>> GetAllAsync();
}