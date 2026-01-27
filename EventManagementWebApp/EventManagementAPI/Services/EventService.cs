using EventManagementAPI.Commons;
using EventManagementAPI.Data;
using EventManagementAPI.Repositories.Interfaces;
using EventManagementAPI.Services.Interfaces;
using EventManagementAPI.ViewModels;

namespace EventManagementAPI.Services
{
    public class EventService : IEventService
    {
        private readonly IVenueRepository _venueRepository;
        private readonly AppDbContext _dbContext;
        private const string VenueIsNotExisted = "Venue is not existed.";

        public EventService(IVenueRepository venueRepository, AppDbContext context)
        {
            _venueRepository = venueRepository;
            _dbContext = context;
        }

        public async Task AddVenueAsync(VenueDto venueDto)
        {
            if (venueDto == null || string.IsNullOrWhiteSpace(venueDto.Name)) throw new ArgumentException();

            var venue = MapperSingleton.Instance.ToVenue(venueDto);

            await _venueRepository.AddAsync(venue);
            await _dbContext.SaveChangesAsync();

            venueDto.Id = (int)venue.Id;
        }

        public async Task<bool> DeleteVenueAsync(int id)
        {
            if(id <= 0) throw  new ArgumentOutOfRangeException();

            var venue = await _venueRepository.GetByIdAsync(id);
            if (venue == null)
            {
                throw new Exception(VenueIsNotExisted);
            }

            await _venueRepository.DeleteAsync(venue);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async  Task<VenueDto> GetVenueByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException();

            var venue = await _venueRepository.GetByIdAsync(id) ?? throw new Exception(VenueIsNotExisted);
            return MapperSingleton.Instance.ToVenueDto(venue);
        }

        public async Task<bool> UpdateVenueAsync(VenueDto venueDto)
        {
            if (venueDto == null || string.IsNullOrWhiteSpace(venueDto.Name)) throw new ArgumentException();

            var existedVenue = await _venueRepository.GetByIdAsync(venueDto.Id);
            if (existedVenue == null) throw new Exception(VenueIsNotExisted);
            existedVenue.Name = venueDto.Name;
            existedVenue.Description = venueDto.Description;
            existedVenue.Capacity = venueDto.Capacity;

            await _venueRepository.UpdateAsync(existedVenue);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
