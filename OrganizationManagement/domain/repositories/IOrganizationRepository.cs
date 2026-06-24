public interface IOrganizationRepository {
    Task<Organization?> GetByIdAsync(Guid id);
    Task<Organization?> GetByNameAsync(string name);
    Task AddOrganizationAsync(Organization organization);
    Task UpdateOrganizationAsync(Organization organization);
    Task DeleteOrganizationAsync(Guid id);
    Task<IEnumerable<Organization>> GetAllAsync();
}