public interface IUserRepository
{
    Task<User?> GetByPhoneAsync(string phone);
    Task AddAsync(User user);
}