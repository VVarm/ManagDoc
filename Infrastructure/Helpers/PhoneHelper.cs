using System.Text.RegularExpressions;
public static class PhoneHelper {
    private static readonly Regex digitsOnly = new(@"[\s\-\(\)\+]");

    public static string NormalizePhone(string phone) => digitsOnly.Replace(phone, "");

    public static bool IsValidPhone(string phone) // Принимается очищенный телефон - NormalizePhone(phone)
    {
        if (string.IsNullOrEmpty(phone)) return false;
        return phone.All(char.IsDigit) && phone.Length >= 10 && phone.Length <= 15;
    }
}