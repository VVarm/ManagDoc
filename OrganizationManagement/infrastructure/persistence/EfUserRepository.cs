using Microsoft.EntityFrameworkCore;

public class EfUserRepository : IUserRepository
{
     private readonly AppDbContext _context;
    public EfUserRepository(AppDbContext appDb) => _context = appDb;

    public async Task<User?> GetByPhoneAsync(string phone) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Phone == phone);

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken) =>
        await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    
    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}