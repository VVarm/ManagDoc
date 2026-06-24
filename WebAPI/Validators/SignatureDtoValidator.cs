using FluentValidation;

public class SignatureDtoValidator : AbstractValidator<SignatureDto>
{
    public SignatureDtoValidator()
    {
        RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone is required").Length(1, 20)
            .Must(phone => PhoneHelper.IsValidPhone(PhoneHelper.NormalizePhone(phone))).WithMessage("Phone number is invalid");
        RuleFor(x => x.SignedAt).NotEmpty().WithMessage("SignedAt is required");
    }
}