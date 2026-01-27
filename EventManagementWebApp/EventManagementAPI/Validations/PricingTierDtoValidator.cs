using EventManagementAPI.ViewModels;
using FluentValidation;

namespace EventManagementAPI.Validations
{
    public class PricingTierDtoValidator : AbstractValidator<PricingTierDto>
    {
        public PricingTierDtoValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .Must(x => string.IsNullOrWhiteSpace(x) || x.Length < 400).WithMessage("Description is too long");

            RuleFor(x => x.Price)
                .GreaterThan(0);
        }
    }
}
