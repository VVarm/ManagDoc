using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(IEmployeeRepository employeeRepository, ILogger<EmployeesController> logger) : ControllerBase
{
    IEmployeeRepository _employeeRepository = employeeRepository;
    ILogger<EmployeesController> _logger = logger;

    /// <summary>
    /// Returns a paginated list of all employees.
    /// </summary>
    /// <param name="page">Page number (default 1).</param>
    /// <param name="pageSize">Items per page (default 10).</param>
    /// <returns>Paginated list of employees.</returns>
    /// <response code="200">List returned successfully.</response>
    /// <response code="400">Invalid pagination parameters.</response>
    [HttpGet]
    public async Task<IActionResult> GetPageEmployees([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Fetching employees with page={Page}, pageSize={PageSize}", page, pageSize);
        if (page < 1 || pageSize < 1) return BadRequest("Page and pageSize must be greater than 0");
        IEnumerable<Employee> listEmployees;
        listEmployees = await _employeeRepository.GetAllAsync();
        return Ok(new PagedResponse<EmployeeResponse>
        {
            TotalCount = listEmployees.Count(),
            Page = page,
            PageSize = pageSize,
            Items = listEmployees.Skip((page - 1) * pageSize).Take(pageSize).Select(MapToResponse).ToList()
        });
    }

    /// <summary>
    /// Retrieves an employee by its ID.
    /// </summary>
    /// <param name="id">Employee ID.</param>
    /// <returns>Employee details.</returns>
    /// <response code="200">Employee found.</response>
    /// <response code="404">Employee not found.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployee(Guid id) {
        _logger.LogInformation("Fetching employee {EmployeeId}", id);
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null) return NotFound("Employee not found");
        return Ok(MapToResponse(employee));
    }

    /// <summary>
    /// Creates a new employee.
    /// </summary>
    /// <param name="request">Employee name and phone.</param>
    /// <returns>Created employee.</returns>
    /// <response code="201">Employee created successfully.</response>
    /// <response code="400">Invalid request (validation failed).</response>
    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest request)
    {
        _logger.LogInformation("Creating employee with name {Name}", request.Name);
        var employee = new Employee(request.Name, request.Phone);
        await _employeeRepository.AddEmployeeAsync(employee);
        return CreatedAtAction(nameof(GetEmployee), new {}, MapToResponse(employee));
    }

    /// <summary>
    /// Updates an existing employee.
    /// </summary>
    /// <param name="id">Employee ID.</param>
    /// <param name="request">New name and phone.</param>
    /// <returns>Updated employee.</returns>
    /// <response code="200">Employee updated successfully.</response>
    /// <response code="404">Employee not found.</response>
    /// <response code="400">Invalid request (validation failed).</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] UpdateEmployeeRequest request)
    {
        _logger.LogInformation("Updating employee {EmployeeId} to name {Name}, phone {Phone}", id, request.Name, request.Phone);
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
            return NotFound("Document not found");
        employee.UpdateEmployee(request.Name, request.Phone);
        await _employeeRepository.UpdateEmployeeAsync(employee);
        return Ok(MapToResponse(employee));
    }

    /// <summary>
    /// Deletes an employee by its ID.
    /// </summary>
    /// <param name="id">Employee ID.</param>
    /// <returns>No content.</returns>
    /// <response code="204">Employee deleted successfully.</response>
    /// <response code="404">Employee not found.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(Guid id)
    {
        _logger.LogInformation("Deleting employee {EmployeeId}", id);
        await _employeeRepository.DeleteEmployeeAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Searches for an employee by their phone number.
    /// </summary>
    /// <param name="phone">Phone number.</param>
    /// <returns>Employee details.</returns>
    /// <response code="200">Employee found.</response>
    /// <response code="404">Employee not found.</response>
    [HttpGet("by-phone/{phone}")]
    public async Task<IActionResult> SearchByPhone(string phone)
    {
        _logger.LogInformation("Searching employee by phone {Phone}", phone);
        var employee = await _employeeRepository.GetByPhoneAsync(phone);
        if (employee == null) return NotFound("Employee not found");
        return Ok(MapToResponse(employee));
    }

    private static EmployeeResponse MapToResponse(Employee employee)
    {
        EmployeeResponse response = new();
        response.Id = employee.Id;
        response.Name = employee.Name;
        response.Phone = employee.Phone;
        return response;
    }
}