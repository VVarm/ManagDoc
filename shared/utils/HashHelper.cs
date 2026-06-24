using System.Security.Cryptography;
using System.Text;

public static class HashHelper
{
    private static readonly SHA256 Sha256 = SHA256.Create();
    public static string Hashing(string input, string salt)
    {
        if (string.IsNullOrEmpty(input))
            throw new ArgumentNullException(nameof(input));
        if (string.IsNullOrEmpty(salt))
            throw new ArgumentNullException(nameof(salt));

        return BitConverter.ToString(Sha256.ComputeHash(Encoding.UTF8.GetBytes(input + salt)))
                            .Replace("-", "")
                            .ToLowerInvariant();
    }
}