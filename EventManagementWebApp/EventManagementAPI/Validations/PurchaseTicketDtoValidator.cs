using EventManagementAPI.ViewModels;
using FluentValidation;

namespace EventManagementAPI.Validations
{
    public class PurchaseTicketDtoValidator : AbstractValidator<PurchaseTicketDto>
    {
        public PurchaseTicketDtoValidator() 
        {
            RuleFor(x => x.TicketTypeId).GreaterThan(0).WithMessage("TicketTypeId is invalid.");
            RuleFor(x => x.BuyerName)
                .NotEmpty()
                .Must(x => !string.IsNullOrWhiteSpace(x) && x.Length <= 100).WithMessage("BuyerName must be less than 100 chars.");
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }
}
