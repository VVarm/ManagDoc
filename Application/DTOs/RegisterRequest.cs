public class RegisterRequest
{
    public string? Phone { get; set; }
    public string? Password { get; set; }
    public UserRole Role { get; set; } = UserRole.Employee;
}