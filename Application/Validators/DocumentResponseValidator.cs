using FluentValidation;

public class DocumentResponseValidator : AbstractValidator<DocumentResponse>
{
    public DocumentResponseValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required");
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required").Length(1, 200);
        RuleFor(x => x.Status).NotEmpty().WithMessage("Status is required");
        RuleFor(x => x.Signatures).NotEmpty().WithMessage("Signatures is required");
    }
}