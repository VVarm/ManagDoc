using FluentValidation;

public class SignatureDtoValidator : AbstractValidator<SignatureDto>
{
    public SignatureDtoValidator()
    {
        RuleFor(x => x.Hash).NotEmpty().WithMessage("Hash is required").Length(1, 100);
        RuleFor(x => x.SignedAt).NotEmpty().WithMessage("SignedAt is required");
    }
}