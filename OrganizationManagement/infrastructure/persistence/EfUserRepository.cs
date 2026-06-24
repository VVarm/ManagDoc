using Microsoft.EntityFrameworkCore;

public class EfUserRepository : IUserRepository
{
        private readonly AppDbContext _context;
        public EfUserRepository(AppDbContext appDb)
        {
            _context = appDb;
        }

        public async Task<User?> GetByPhoneAsync(string phone)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Phone == phone);
        }
        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
}