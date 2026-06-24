using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Organization> Organizations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.Metadata.FindNavigation(nameof(Document.Signatures))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            entity.OwnsMany(d => d.Signatures, owned =>
            {
                owned.WithOwner().HasForeignKey("DocumentId");
                owned.ToTable("Signatures");
            });
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<Organization>(entity =>
        {   
            entity.HasKey(o => o.Id);

            entity.Property(o => o.EmployeeIds)
                .HasColumnName("EmployeeIds")
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) ?? new List<Guid>(),
                new ValueComparer<List<Guid>>(
                    (c1, c2) => 
                        c1 == null && c2 == null ? true :
                        c1 == null || c2 == null ? false :
                        c1.SequenceEqual(c2),
                    c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c == null ? new List<Guid>() : c.ToList()
                ));
        });
    }
}