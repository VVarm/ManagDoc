using FluentValidation;

public class CreateDocumentRequestValidator : AbstractValidator<CreateDocumentRequest>
{
    public CreateDocumentRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required").Length(1, 200);
    }
}