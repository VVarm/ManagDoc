using Microsoft.EntityFrameworkCore;

public class EfOrganizationRepository : IOrganizationRepository
{
    private readonly AppDbContext _context;
    public EfOrganizationRepository(AppDbContext appDb)
    {
        _context = appDb;
    }

    public async Task<Organization?> GetByIdAsync(Guid id)
    {
        return await _context.Organizations.FindAsync(id);
    }

    public async Task<Organization?> GetByNameAsync(string name)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));
        return await _context.Organizations.FirstOrDefaultAsync(o => o.Name == name);
    }

    public async Task AddOrganizationAsync(Organization organization)
    {
        if (organization == null) throw new ArgumentNullException(nameof(organization));
        await _context.Organizations.AddAsync(organization);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateOrganizationAsync(Organization organization)
    {
        if (organization == null) throw new ArgumentNullException(nameof(organization));
        if (await _context.Organizations.FirstOrDefaultAsync(o => o.Id == organization.Id) == null) throw new InvalidOperationException("Organization not found");
        _context.Organizations.Update(organization);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteOrganizationAsync(Guid id)
    {
        var organization = await _context.Organizations.FindAsync(id);
        if (organization == null) throw new InvalidOperationException($"Organization with id {id} not found");
        _context.Organizations.Remove(organization);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Organization>> GetAllAsync()
    {
        return await _context.Organizations.ToListAsync();
    }
}