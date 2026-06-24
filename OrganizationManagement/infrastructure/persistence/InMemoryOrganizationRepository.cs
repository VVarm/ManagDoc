public class InMemoryOrganizationRepository : IOrganizationRepository {
    List<Organization> _storage;

    public InMemoryOrganizationRepository()
    {
        _storage = new List<Organization>();
    }

    Task<Organization?> IOrganizationRepository.GetByIdAsync(Guid id)
    {
        return Task.FromResult(_storage.Find(doc => doc.Id == id));
    }

    Task<Organization?> IOrganizationRepository.GetByNameAsync(string name)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));
        return Task.FromResult(_storage.Find(doc => doc.Name == name));
    }

    Task IOrganizationRepository.AddOrganizationAsync(Organization organization)
    {
        _storage.Add(organization == null ? throw new ArgumentNullException(nameof(organization)) : organization);
        return Task.CompletedTask;
    }

    Task IOrganizationRepository.UpdateOrganizationAsync(Organization organization)
    {
        int index = _storage.FindIndex(d => d.Id == organization.Id);
        if (index >= 0) _storage[index] = organization;
        else throw new InvalidOperationException("Organization not found");
        return Task.CompletedTask;
    }

    Task IOrganizationRepository.DeleteOrganizationAsync(Guid id)
    {
        int removed = _storage.RemoveAll(doc => doc.Id == id);
        if (removed == 0)
            throw new InvalidOperationException($"Organization with id {id} not found");
        return Task.CompletedTask;
    }

    Task<IEnumerable<Organization>> IOrganizationRepository.GetAllAsync()
    {
        return Task.FromResult(_storage.AsEnumerable());
    }
}