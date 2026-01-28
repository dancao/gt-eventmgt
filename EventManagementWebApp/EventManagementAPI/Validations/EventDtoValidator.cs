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
                .Must(x => string.IsNullOrWhiteSpace(x) || x.Length < 100).WithMessage("Description is too long");

            RuleFor(x => x.EventDate)
                .NotNull()
                .GreaterThanOrEqualTo(DateTime.UtcNow);

            RuleFor(x => x.Duration).GreaterThan(0);
            RuleFor(x => x.VenueId).GreaterThan(0);

            RuleFor(x => x.TicketTypes).NotNull().NotEmpty();
            RuleForEach(x => x.TicketTypes).SetValidator(new TicketTypeDtoValidator());
        }
    }

    public class TicketTypeDtoValidator : AbstractValidator<TicketTypeDto>
    {
        public TicketTypeDtoValidator()
        {
            RuleFor(tt => tt.Name).NotEmpty();
            RuleFor(tt => tt.TotalAvailable).NotNull().GreaterThan(0);
            RuleFor(tt => tt.PricingTierId).GreaterThan(0);
            RuleFor(tt => tt).Must(x => x.Remaining > 0 && x.Remaining <= x.TotalAvailable)
                .WithMessage("Remaining must be a positive number and less than or equal to TotalAvailable.");
        }
    }
}
