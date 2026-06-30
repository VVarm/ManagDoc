using FluentValidation;

public class SignDocumentRequestValidator : AbstractValidator<SignDocumentRequest>
{
    public SignDocumentRequestValidator()
    {
        RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone is required").Length(1, 20)
            .Must(phone => PhoneHelper.IsValidPhone(PhoneHelper.NormalizePhone(phone))).WithMessage("Phone number is invalid");
    }
}