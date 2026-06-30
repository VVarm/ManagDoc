using FluentValidation;

public class TransferEmployeeRequestValidator : AbstractValidator<TransferEmployeeRequest>
{
    public TransferEmployeeRequestValidator()
    {
        RuleFor(x => x.FromOrganizationId).NotEmpty().WithMessage("FromOrganizationId is required");
        RuleFor(x => x.ToOrganizationId).NotEmpty().WithMessage("ToOrganizationId is required");
        RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone is required").Length(1, 20)
            .Must(phone => PhoneHelper.IsValidPhone(PhoneHelper.NormalizePhone(phone))).WithMessage("Phone number is invalid");
    }
}