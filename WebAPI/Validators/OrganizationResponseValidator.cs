using FluentValidation;

public class OrganizationResponseValidator : AbstractValidator<OrganizationResponse>
{
    public OrganizationResponseValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required").Length(1, 200);
        RuleFor(x => x.EmployeeIds).NotEmpty().WithMessage("EmployeeIds is required");
    }
}