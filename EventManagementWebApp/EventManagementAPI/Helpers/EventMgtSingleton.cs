using EventManagementAPI.Commons;
using EventManagementAPI.Domain.Entities;
using EventManagementAPI.ViewModels;
using FluentValidation.Results;

namespace EventManagementAPI.Helpers
{
    public sealed class EventMgtSingleton
    {
        private static readonly Lazy<EventMgtSingleton> lazyInstance = new Lazy<EventMgtSingleton>(() => new EventMgtSingleton());

        private EventMgtSingleton()
        {
        }

        public static EventMgtSingleton Instance => lazyInstance.Value;

        public Venue ToVenue(VenueDto venueDto)
        {
            var venue = new Venue();
            venue.Id = venueDto.Id;
            venue.Name = venueDto.Name;
            venue.Description = venueDto.Description;
            venue.Address = venueDto.Address;
            venue.Capacity = venueDto.Capacity;
            venue.IsActive = venueDto.IsActive;

            return venue;
        }

        public VenueDto ToVenueDto(Venue venue)
        {
            var venueDto = new VenueDto();
            venueDto.Id = (int)venue.Id;
            venueDto.Name = venue.Name;
            venueDto.Description = venue.Description;
            venueDto.Address = venue.Address;
            venueDto.Capacity = (int)venue.Capacity;
            venueDto.CreatedOn = venue.CreatedOn;
            venueDto.IsActive = venue.IsActive;

            return venueDto;
        }

        public Event ToEvent(EventDto eventDto)
        {
            var evt = new Event();
            evt.Name = eventDto.Name;
            evt.Description = eventDto.Description;
            evt.EventDate = eventDto.EventDate.HasValue ? eventDto.EventDate.Value : throw new Exception("EventDate is required.");
            evt.Duration = eventDto.Duration;
            evt.VenueId = eventDto.VenueId;
            evt.IsActive = eventDto.IsActive;
            evt.TicketTypes = eventDto.TicketTypes.Select(tt => new TicketType()
            {
                Name = tt.Name,
                PricingTierId = tt.PricingTierId,
                TotalAvailable = tt.TotalAvailable,
                Remaining = tt.Remaining
            }).ToList();
            return evt;
        }

        public EventDto ToEventDto(Event eventItem)
        {
            var evtDto = new EventDto();
            evtDto.Id = eventItem.Id;
            evtDto.Name = eventItem.Name;
            evtDto.Description = eventItem.Description;
            evtDto.EventDate = eventItem.EventDate;
            evtDto.Duration = eventItem.Duration;
            evtDto.VenueId = eventItem.VenueId;
            evtDto.Venue = new VenueDto()
            {
                Name = eventItem.Venue?.Name ?? "",
                Description = eventItem.Venue?.Description ?? "",
                Id = eventItem.Id
            };
            evtDto.IsActive = eventItem.IsActive;
            evtDto.TicketTypes = eventItem.TicketTypes.Select(tt => new TicketTypeDto()
            {
                Id = tt.Id,
                Name = tt.Name,
                PricingTierId = tt.PricingTierId,
                PricingTier = new PricingTierDto() { 
                    Name = tt.PricingTier?.Name ?? "",
                    Price = tt.PricingTier?.Price ?? -1,
                },
                TotalAvailable = tt.TotalAvailable,
                Remaining = tt.Remaining
            }).ToList();
            return evtDto;
        }

        public DtoValidationResult GetValidationResultErrorMessage(ValidationResult validationResult)
        {
            var customErrs = validationResult.Errors.Select(x =>
                new ResultDetails() { PropertyName = x.PropertyName, ErrorMessage = x.ErrorMessage }).ToList();
            return new DtoValidationResult()
            {
                IsValid = validationResult.IsValid,
                Errors = customErrs
            };
        }

        public ApiResponse<T> GetApiResponse<T>(T data, ApiResponseStatus status, string errorMessage)
        {
            return new ApiResponse<T>()
            {
                Status = status,
                Data = data,
                ErrorMessage = errorMessage
            };
        }
    }
}
