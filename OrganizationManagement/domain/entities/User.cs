public class User {
    public Guid Id { get; }
    public string Phone { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }
    public Guid? EmployeeId { get; private set; }

    public User(string phone, string password, UserRole role) {

        Id = Guid.NewGuid();
        Phone = phone == null ? throw new ArgumentNullException(nameof(phone)) :
                                PhoneHelper.IsValidPhone(PhoneHelper.NormalizePhone(phone)) ? PhoneHelper.NormalizePhone(phone) :
                                    throw new ArgumentException("Invalid phone number", nameof(phone));
        PasswordHash = PasswordHasher.HashPassword(password ?? throw new ArgumentNullException(nameof(password)));
        Role = role;
    }

    public void SetRefreshToken(string token, DateTime expiry) {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token));
        if (expiry <= DateTime.UtcNow)
            throw new ArgumentException("Expiry time must be in the future", nameof(expiry));
        RefreshToken = token;
        RefreshTokenExpiryTime = expiry;
    }

    public void ClearRefreshToken() {
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
    }

    public bool VerifyPassword(string password) {
        return PasswordHasher.VerifyPassword(password ?? throw new ArgumentNullException(nameof(password)), PasswordHash);
    }

    // TODO для EF Core
    private User()
    {
        Phone = string.Empty;
        PasswordHash = string.Empty;
        Role = UserRole.Employee;
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
        EmployeeId = null;
    }
}