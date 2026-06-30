using FluentValidation;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone is required").Length(1, 20)
            .Must(phone => PhoneHelper.IsValidPhone(PhoneHelper.NormalizePhone(phone))).WithMessage("Phone number is invalid");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .Length(8, 20)
            .Must(p => p.Any(char.IsDigit)).WithMessage("Password must contain at least one digit")
            .Must(p => p.Any(char.IsUpper)).WithMessage("Password must contain at least one uppercase letter")
            .Must(p => p.Any(char.IsLower)).WithMessage("Password must contain at least one lowercase letter");
    }
}