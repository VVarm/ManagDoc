using Microsoft.EntityFrameworkCore;

public class EfEmployeeRepository : IEmployeeRepository
{
    private AppDbContext _context;
    public EfEmployeeRepository(AppDbContext appDb)
    {
        _context = appDb;
    }

    public async Task<Employee?> GetByIdAsync(Guid id)
    {
        return await _context.Employees.FindAsync(id);
    }

    public async Task<Employee?> GetByPhoneAsync(string phone)
    {
        var clean = PhoneHelper.NormalizePhone(phone);
        if (!PhoneHelper.IsValidPhone(clean)) return null;
        return await _context.Employees.FirstOrDefaultAsync(o => o.Phone == clean);
    }

    public async Task AddEmployeeAsync(Employee employee)
    {
        if (employee == null) throw new ArgumentNullException(nameof(employee));
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateEmployeeAsync(Employee employee)
    {
        if (employee == null) throw new ArgumentNullException(nameof(employee));
        if (await _context.Employees.FirstOrDefaultAsync(o => o.Id == employee.Id) == null) throw new InvalidOperationException("Employee not found");
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteEmployeeAsync(Guid id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) throw new InvalidOperationException($"Employee with id {id} not found");
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _context.Employees.ToListAsync();
    }
}