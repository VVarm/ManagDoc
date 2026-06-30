using FluentValidation;

public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required").Length(1, 200);
        RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone is required").Length(1, 20)
            .Must(phone => PhoneHelper.IsValidPhone(PhoneHelper.NormalizePhone(phone))).WithMessage("Phone number is invalid");
    }
}