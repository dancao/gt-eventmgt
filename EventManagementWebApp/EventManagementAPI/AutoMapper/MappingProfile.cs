using AutoMapper;
using EventManagementAPI.Domain.Entities;
using EventManagementAPI.ViewModels;
using Microsoft.Data.Sqlite;

namespace EventManagementAPI.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<string, DateTime>().ConvertUsing(s => DateTime.Parse(s));

            CreateMap<SqliteDataReader, Venue>();
            CreateMap<Venue, VenueDto>();
            CreateMap<VenueDto, Venue>()
                .ForMember(v => v.CreatedOn, opts => opts.Ignore())
                .ForMember(v => v.CreatedBy, opts => opts.Ignore())
            ;

            CreateMap<SqliteDataReader, Event>();
            CreateMap<Event, EventDto>();
        }
    }
}
