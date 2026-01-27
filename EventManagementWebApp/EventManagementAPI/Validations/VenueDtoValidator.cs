using EventManagementAPI.ViewModels;
using FluentValidation;

namespace EventManagementAPI.Validations
{
    public class VenueDtoValidator: AbstractValidator<VenueDto>
    {
        public VenueDtoValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Venue Name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .Must(x => string.IsNullOrWhiteSpace(x) || x.Length < 400).WithMessage("Description is too long");

            RuleFor(x => x.Capacity)
                .GreaterThan(0)
                .LessThan(2000);
        }
    }
}
