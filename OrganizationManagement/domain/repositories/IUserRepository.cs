public interface IUserRepository
{
    Task<User?> GetByPhoneAsync(string phone);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
}