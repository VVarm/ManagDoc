using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrganizationsController(IOrganizationRepository organizationRepository, IEmployeeRepository employeeRepository, TransferEmployeeHandler transferEmployeeHandler, ILogger<OrganizationsController> logger) : ControllerBase
{
    private IOrganizationRepository _organizationRepository = organizationRepository;
    private IEmployeeRepository _employeeRepository = employeeRepository;
    private TransferEmployeeHandler _transferEmployeeHandler = transferEmployeeHandler;
    ILogger<OrganizationsController> _logger = logger;

    /// <summary>
    /// Returns a paginated list of all organizations.
    /// </summary>
    /// <param name="page">Page number (default 1).</param>
    /// <param name="pageSize">Items per page (default 10).</param>
    /// <returns>Paginated list of organizations.</returns>
    /// <response code="200">List returned successfully.</response>
    /// <response code="400">Invalid pagination parameters.</response>
    [HttpGet]
    public async Task<IActionResult> GetPageOrganizations([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Fetching organizations with page={Page}, pageSize={PageSize}", page, pageSize);
        if (page < 1 || pageSize < 1) return BadRequest("Page and pageSize must be greater than 0");
        IEnumerable<Organization> listOrganizations;
        listOrganizations = await _organizationRepository.GetAllAsync();
        return Ok(new PagedResponse<OrganizationResponse>
        {
            TotalCount = listOrganizations.Count(),
            Page = page,
            PageSize = pageSize,
            Items = listOrganizations.Skip((page - 1) * pageSize).Take(pageSize).Select(MapToResponse).ToList()
        });
    }

    /// <summary>
    /// Retrieves an organization by its ID.
    /// </summary>
    /// <param name="id">Organization ID.</param>
    /// <returns>Organization details.</returns>
    /// <response code="200">Organization found.</response>
    /// <response code="404">Organization not found.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrganization(Guid id)
    {
        _logger.LogInformation("Fetching organization {OrganizationId}", id);
        var organization = await _organizationRepository.GetByIdAsync(id);
        if (organization == null) return NotFound("Organization not found");
        return Ok(MapToResponse(organization));
    }

    /// <summary>
    /// Creates a new organization.
    /// </summary>
    /// <param name="request">Organization name.</param>
    /// <returns>Created organization.</returns>
    /// <response code="201">Organization created successfully.</response>
    /// <response code="400">Invalid request (validation failed).</response>
    [HttpPost]
    public async Task<IActionResult> CreateOrganization([FromBody] CreateOrganizationRequest request)
    {
        _logger.LogInformation("Creating organization with name {Name}", request.Name);
        var organization = new Organization(request.Name);
        await _organizationRepository.AddOrganizationAsync(organization);
        return CreatedAtAction(nameof(GetOrganization), new { id = organization.Id }, MapToResponse(organization));
    }

    /// <summary>
    /// Updates an existing organization.
    /// </summary>
    /// <param name="id">Organization ID.</param>
    /// <param name="request">New name.</param>
    /// <returns>Updated organization.</returns>
    /// <response code="200">Organization updated successfully.</response>
    /// <response code="404">Organization not found.</response>
    /// <response code="400">Invalid request (validation failed).</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrganization(Guid id, [FromBody] UpdateOrganizationRequest request)
    {
        _logger.LogInformation("Updating organization {OrganizationId} to name {Name}", id, request.Name);
        var organization = await _organizationRepository.GetByIdAsync(id);
        if (organization == null)
            return NotFound("Organization not found");
        organization.UpdateName(request.Name);
        await _organizationRepository.UpdateOrganizationAsync(organization);
        return Ok(MapToResponse(organization));
    }

    /// <summary>
    /// Deletes an organization by its ID.
    /// </summary>
    /// <param name="id">Organization ID.</param>
    /// <returns>No content.</returns>
    /// <response code="204">Organization deleted successfully.</response>
    /// <response code="404">Organization not found.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrganization(Guid id)
    {
        _logger.LogInformation("Deleting organization {OrganizationId}", id);
        await _organizationRepository.DeleteOrganizationAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Adds an existing employee to an organization.
    /// </summary>
    /// <param name="id">Organization ID.</param>
    /// <param name="request">Employee ID.</param>
    /// <returns>Operation result.</returns>
    /// <response code="200">Employee added successfully.</response>
    /// <response code="404">Organization or employee not found.</response>
    /// <response code="400">Employee already in organization.</response>
    [HttpPost("{id}/employees")]
    public async Task<IActionResult> AddEmployeeInOrganization(Guid id, AddEmployeeRequest request)
    {
        _logger.LogInformation("Adding employee {EmployeeId} to organization {OrganizationId}", request.EmployeeId, id);
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
        if (employee == null)
            return NotFound("Employee not found");
        var organization = await _organizationRepository.GetByIdAsync(id);
        if (organization == null)
            return NotFound("Organization not found");
        organization.AddEmployee(employee.Id);
        return Ok();
    }

    /// <summary>
    /// Removes an employee from an organization.
    /// </summary>
    /// <param name="id">Organization ID.</param>
    /// <param name="employeeId">Employee ID.</param>
    /// <returns>Operation result.</returns>
    /// <response code="200">Employee removed successfully.</response>
    /// <response code="404">Organization or employee not found.</response>
    [HttpDelete("{id}/employees/{employeeId}")]
    public async Task<IActionResult> DeleteEmployeeFromOrganization(Guid id, Guid employeeId)
    {
        _logger.LogInformation("Removing employee {EmployeeId} from organization {OrganizationId}", employeeId, id);
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null)
            return NotFound("Employee not found");
        var organization = await _organizationRepository.GetByIdAsync(id);
        if (organization == null)
            return NotFound("Organization not found");
        organization.RemoveEmployee(employeeId);
        return Ok();
    }

    /// <summary>
    /// Transfers an employee from one organization to another.
    /// </summary>
    /// <param name="request">Source organization ID, target organization ID and employee phone.</param>
    /// <returns>Operation result.</returns>
    /// <response code="200">Employee transferred successfully.</response>
    /// <response code="404">Organization or employee not found.</response>
    /// <response code="400">Invalid phone number or employee cannot be transferred.</response>
    [HttpPost("transfer")]
    public async Task<IActionResult> TransferEmployeeBetweenOrganizations(TransferEmployeeRequest request)
    {
        _logger.LogInformation("Transferring employee {Phone} from organization {FromId} to {ToId}", request.Phone, request.FromOrganizationId, request.ToOrganizationId);
        var clean = PhoneHelper.NormalizePhone(request.Phone);
        if (!PhoneHelper.IsValidPhone(clean))
            return BadRequest($"Invalid phone number{request.Phone}");
        var command = new TransferEmployeeCommand(request.FromOrganizationId, request.ToOrganizationId, clean);
        await _transferEmployeeHandler.Handle(command);
        return Ok("Employee transfered");
    }

    private static OrganizationResponse MapToResponse(Organization organization)
    {
        OrganizationResponse response = new();
        response.Id = organization.Id;
        response.Name = organization.Name;
        response.EmployeeIds = organization.EmployeeIds.ToList();
        return response;
    }
}