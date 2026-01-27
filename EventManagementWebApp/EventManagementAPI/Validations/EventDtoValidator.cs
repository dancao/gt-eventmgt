using EventManagementAPI.ViewModels;
using FluentValidation;

namespace EventManagementAPI.Validations
{
    public class EventDtoValidator : AbstractValidator<EventDto>
    {
        public EventDtoValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Event Name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .Must(x => string.IsNullOrWhiteSpace(x) || x.Length < 400).WithMessage("Description is too long");

            RuleFor(x => x.EventDate)
                .NotNull()
                .GreaterThanOrEqualTo(DateTime.UtcNow.AddDays(7)) // booking 7 days 
                ;

            RuleFor(x => x.Duration).GreaterThan(0);

            RuleFor(x => x.VenueId).GreaterThan(0);
            RuleFor(x => x.PricingTierId).GreaterThan(0);
        }
    }
}
