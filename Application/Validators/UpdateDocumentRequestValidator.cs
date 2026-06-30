using FluentValidation;

public class UpdateDocumentRequestValidator : AbstractValidator<UpdateDocumentRequest>
{
    public UpdateDocumentRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required").Length(1, 200);
    }
}