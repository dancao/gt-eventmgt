using EventManagementAPI.Data;
using EventManagementAPI.Domain.Entities;
using EventManagementAPI.Helpers;
using EventManagementAPI.Repositories.Interfaces;
using EventManagementAPI.Services.Interfaces;
using EventManagementAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace EventManagementAPI.Services
{
    public class EventService : IEventService
    {
        private readonly IVenueRepository _venueRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IPricingRepository _pricingRepository;
        private readonly AppDbContext _dbContext;
        private const string VenueIsNotExisted = "Venue is not existed.";

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
        #endregion

        #region Event
        public async Task AddEventAsync(EventDto eventDto)
        {
            if (eventDto == null) throw new ArgumentException("Event is required.");

            var venue = await _venueRepository.GetByIdAsync((int)eventDto.VenueId);
            if (venue == null || !venue.IsActive) throw new ArgumentException("Venue is invalid.");

            var evt = EventMgtSingleton.Instance.ToEvent(eventDto);

            var isVenueAvailable = await _eventRepository.IsVenueAvailable(evt);
            if (!isVenueAvailable) throw new ArgumentException("Venue is not available.");

            var pricingTier = await _pricingRepository.GetPricingTierByIdAsync(eventDto.PricingTierId);
            if (pricingTier == null || !pricingTier.IsActive) throw new ArgumentException("Pricing Tier is invalid.");

            
            await _eventRepository.AddAsync(evt);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<EventDto> GetEventByIdAsync(long id, bool includeVenue = false, bool includePricingTier = false)
        {
            IQueryable<Event> query = _dbContext.Events;

            if (includeVenue) query = query.Include(x => x.Venue);
            if (includePricingTier) query = query.Include(x => x.PricingTier);

            var eventItem = await query.FirstOrDefaultAsync(x => x.Id == id) ?? throw new ArgumentException("Event is not existed.");
            return EventMgtSingleton.Instance.ToEventDto(eventItem) ;
        }
        #endregion
    }
}
