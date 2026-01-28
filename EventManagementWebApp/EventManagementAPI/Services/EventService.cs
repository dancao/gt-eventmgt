using EventManagementAPI.Data;
using EventManagementAPI.Domain.Entities;
using EventManagementAPI.Helpers;
using EventManagementAPI.Repositories.Interfaces;
using EventManagementAPI.Services.Interfaces;
using EventManagementAPI.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EventManagementAPI.Services
{
    public class EventService : IEventService
    {
        private readonly IVenueRepository _venueRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IPricingRepository _pricingRepository;
        private readonly AppDbContext _dbContext;
        private const string VenueIsNotExisted = "Venue is not existed.";
        private const string EventIsNotExisted = "Event is not existed.";

        public EventService(IVenueRepository venueRepository, IEventRepository eventRepository,
            AppDbContext context, IPricingRepository pricingRepository)
        {
            _venueRepository = venueRepository;
            _eventRepository = eventRepository;
            _pricingRepository = pricingRepository;
            _dbContext = context;
        }

        #region Venue
        public async Task AddVenueAsync(VenueDto venueDto)
        {
            if (venueDto == null || string.IsNullOrWhiteSpace(venueDto.Name)) throw new ArgumentException();

            var venue = EventMgtSingleton.Instance.ToVenue(venueDto);

            await _venueRepository.AddAsync(venue);
            await _dbContext.SaveChangesAsync();

            venueDto.Id = (int)venue.Id;
        }

        public async Task<bool> DeleteVenueAsync(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException();

            var venue = await _venueRepository.GetByIdAsync(id);
            if (venue == null)
            {
                throw new Exception(VenueIsNotExisted);
            }

            await _venueRepository.DeleteAsync(venue);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<VenueDto> GetVenueByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException();

            var venue = await _venueRepository.GetByIdAsync(id) ?? throw new Exception(VenueIsNotExisted);
            return EventMgtSingleton.Instance.ToVenueDto(venue);
        }

        public async Task<bool> UpdateVenueAsync(VenueDto venueDto)
        {
            if (venueDto == null || string.IsNullOrWhiteSpace(venueDto.Name)) throw new ArgumentException();

            var existedVenue = await _venueRepository.GetByIdAsync(venueDto.Id);
            if (existedVenue == null) throw new Exception(VenueIsNotExisted);
            existedVenue.Name = venueDto.Name;
            existedVenue.Description = venueDto.Description;
            existedVenue.Address = venueDto.Address;
            existedVenue.Capacity = venueDto.Capacity;
            existedVenue.IsActive = venueDto.IsActive;

            await _venueRepository.UpdateAsync(existedVenue);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<VenueDto>> GetVenuesAsync()
        {
            var venues = await _venueRepository.GetAllAsync();
            return venues.Select(EventMgtSingleton.Instance.ToVenueDto).ToList();
        }

        public async Task<List<VenueDto>> SearchVenuesAsync(string venueName, string desc, int minCapacity, int maxCapacity, bool isActive = true)
        {
            IQueryable<Venue> query = _dbContext.Venues;

            if (!string.IsNullOrWhiteSpace(venueName))
            {
                query = query.Where(x => x.Name.ToLower().StartsWith(venueName.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(desc))
            {
                query = query.Where(x => !string.IsNullOrWhiteSpace(desc) && x.Description.ToLower().StartsWith(desc.ToLower()));
            }
            if (minCapacity > 0)
            {
                query = query.Where(x => x.Capacity >= minCapacity);
            }
            if (maxCapacity > 0)
            {
                query = query.Where(x => x.Capacity <= maxCapacity);
            }
            if (isActive)
            {
                query = query.Where(x => x.IsActive);
            }

            var results = await query.OrderBy(x => x.Name).ToListAsync();
            return results?.Select(EventMgtSingleton.Instance.ToVenueDto).ToList() ?? [];
        }

        #endregion

        #region Event
        public async Task AddEventAsync(EventDto eventDto)
        {
            if (eventDto == null) throw new ArgumentException("Event is required.");

            var venue = await _venueRepository.GetByIdAsync((int)eventDto.VenueId);
            if (venue == null || !venue.IsActive) throw new ArgumentException("Venue is invalid.");

            var evt = EventMgtSingleton.Instance.CreateNewEvent(eventDto);

            var isVenueAvailable = await _eventRepository.IsVenueAvailable(evt);
            if (!isVenueAvailable) throw new ArgumentException("Venue is not available.");

            foreach (var tt in eventDto.TicketTypes)
            {
                var pricingTier = await _pricingRepository.GetPricingTierByIdAsync(tt.PricingTierId);
                if (pricingTier == null || !pricingTier.IsActive) throw new ArgumentException("Pricing Tier is invalid.");
            }

            await _eventRepository.AddAsync(evt);
            await _dbContext.SaveChangesAsync();
            eventDto.Id = evt.Id;
        }

        private async Task<Event> GetEventEntityByIdAsync(long id, bool includeVenue = false, bool includeTicketTypes = false)
        {
            IQueryable<Event> query = _dbContext.Events;

            if (includeVenue) query = query.Include(x => x.Venue);
            if (includeTicketTypes)
            {
                query = query.Include(x => x.TicketTypes).ThenInclude(x => x.Tickets);
            }

            return await query.FirstOrDefaultAsync(x => x.Id == id) ?? throw new ArgumentException("Event is not existed.");

        }
        public async Task<EventDto> GetEventByIdAsync(long id, bool includeVenue = false, bool includeTicketTypes = false)
        {
            var eventItem = await GetEventEntityByIdAsync(id, includeVenue, includeTicketTypes);
            return EventMgtSingleton.Instance.ToEventDto(eventItem);
        }

        public async Task<List<EventDto>> GetAllEventsAsync()
        {
            var results = await _eventRepository.GetAllAsync();
            return results.Select(EventMgtSingleton.Instance.ToEventDto).ToList();
        }

        public async Task<bool> UpdateEventAsync(EventDto eventDto)
        {
            if (eventDto == null || string.IsNullOrWhiteSpace(eventDto.Name) || eventDto.Id <= 0) throw new ArgumentException(nameof(eventDto));

            var existedEvent = await GetEventEntityByIdAsync(eventDto.Id, true, true);
            if (existedEvent == null) throw new Exception(EventIsNotExisted);
            if(existedEvent.EventStatus == Commons.EventStatus.Finished || !existedEvent.IsActive) throw new Exception("Event closed or not active.");

            existedEvent.Name = eventDto.Name;
            existedEvent.VenueId = eventDto.VenueId;
            existedEvent.EventDate = eventDto.EventDate.HasValue ? eventDto.EventDate.Value : throw new ArgumentException("EventDate is invalid.");
            existedEvent.Duration = eventDto.Duration;

            foreach (var ticketTypeDto in eventDto.TicketTypes)
            {
                var existedTicketType = existedEvent.TicketTypes.FirstOrDefault(x => x.Id == ticketTypeDto.Id);
                if (existedTicketType == null)
                {
                    existedEvent.TicketTypes.Add(CreateTicketTypeEntity(ticketTypeDto));
                }
                else
                {
                    existedTicketType.Name = ticketTypeDto.Name;
                    existedTicketType.TotalAvailable = ticketTypeDto.TotalAvailable;
                    existedTicketType.Remaining = ticketTypeDto.Remaining;
                    existedTicketType.PricingTierId = ticketTypeDto.PricingTierId;
                }
            }
            await _eventRepository.UpdateAsync(existedEvent);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEventAsync(long id)
        {
            var existedEvent = await GetEventEntityByIdAsync(id, true, true);
            if(existedEvent == null) throw new ArgumentException(EventIsNotExisted);

            if (existedEvent.TicketTypes != null && existedEvent.TicketTypes.Any(x => x.Tickets != null && x.Tickets.Count > 0))
            {
                // TODO: refund case
                throw new ArgumentException("Event needs to be closed or finished or inactive to delete.");
            }

            await _eventRepository.DeleteAsync(existedEvent);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private TicketType CreateTicketTypeEntity(TicketTypeDto ticketTypeDto)
        {
            return new TicketType()
            {
                Name = ticketTypeDto.Name,
                TotalAvailable = ticketTypeDto.TotalAvailable,
                Remaining = ticketTypeDto.Remaining,
                PricingTierId = ticketTypeDto.PricingTierId
            };
        }

        public async Task<(List<TicketAvailabilityDto> ticketAvailabilities, int totalCount)> GetTicketAvailabilityAsync(int pageNumber = 1, 
            int pageSize = 10)
        {
            var pagedEvents = await _eventRepository.GetTicketAvailabilityAsync(pageNumber, pageSize);
            var results = pagedEvents.events.Select(CreateTicketAvailabilityDto).ToList();
            return (results, pagedEvents.totalCount);
        }

        private TicketAvailabilityDto CreateTicketAvailabilityDto(Event eventEntity)
        {
            var ticket = new TicketAvailabilityDto();

            ticket.EventId = eventEntity.Id;
            ticket.EventName = eventEntity.Name;
            ticket.VenueName = eventEntity.Venue?.Name ?? "";
            ticket.EventDate = eventEntity.EventDate;
            ticket.TicketTypes = eventEntity.TicketTypes?.Select(x => new TicketTypeLiteDto()
            {
                Name = x.Name,
                TotalAvailable = x.TotalAvailable,
                Remaining = x.Remaining
            }).ToList() ?? [];

            return ticket;
        }
        #endregion
    }
}
