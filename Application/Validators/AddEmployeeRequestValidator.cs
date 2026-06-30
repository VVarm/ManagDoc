using FluentValidation;

public class AddEmployeeRequestValidator : AbstractValidator<AddEmployeeRequest>
{
    public AddEmployeeRequestValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("EmployeeId is required");
    }
}