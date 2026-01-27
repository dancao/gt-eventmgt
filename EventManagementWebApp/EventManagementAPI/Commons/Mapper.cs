using EventManagementAPI.Domain.Entities;
using EventManagementAPI.ViewModels;

namespace EventManagementAPI.Commons
{
    public sealed class MapperSingleton
    {
        private static readonly Lazy<MapperSingleton> lazyInstance = new Lazy<MapperSingleton>(() => new MapperSingleton());
        private MapperSingleton()
        {
        }

        public static MapperSingleton Instance => lazyInstance.Value;

        public Venue ToVenue(VenueDto venueDto)
        {
            var venue = new Venue();
            venue.Id = venueDto.Id;
            venue.Name = venueDto.Name;
            venue.Description = venueDto.Description;
            venue.Capacity = venueDto.Capacity;

            return venue;
        }

        public VenueDto ToVenueDto(Venue venue)
        {
            var venueDto = new VenueDto();
            venueDto.Id = (int)venue.Id;
            venueDto.Name = venue.Name;
            venueDto.Description = venue.Description;
            venueDto.Capacity = (int)venue.Capacity;
            venueDto.CreatedOn = venue.CreatedOn;

            return venueDto;
        }
    }
}
