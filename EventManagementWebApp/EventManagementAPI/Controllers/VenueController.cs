using EventManagementAPI.Services.Interfaces;
using EventManagementAPI.ViewModels;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class VenueController : ControllerBase
    {
        private readonly IEventService _eventService;
        private IValidator<VenueDto> _validator;

        public VenueController(IEventService eventService, IValidator<VenueDto> validator)
        { 
            _eventService = eventService;
            _validator = validator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(VenueDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddVenue(VenueDto venueDto)
        {
            var validationResult = await _validator.ValidateAsync(venueDto);
            if (!validationResult.IsValid)
            {
                var customerErrs = validationResult.Errors.Select(x => new { x.PropertyName, x.ErrorMessage}).ToList();
                return BadRequest(new { validationResult.IsValid, errors = customerErrs });
            }

            await _eventService.AddVenueAsync(venueDto);
            return CreatedAtAction(nameof(GetVenueById), new { id = venueDto.Id }, venueDto);
        }

        [HttpGet("{id}", Name = "GetVenueById")]
        [ProducesResponseType(typeof(VenueDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(VenueDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVenueById(int id) 
        {
            var venue = await _eventService.GetVenueByIdAsync(id);
            if(venue == null) return NotFound();
            return Ok(venue);
        }

        [HttpPut]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateVenue(VenueDto venueDto)
        {
            var validationResult = await _validator.ValidateAsync(venueDto);
            if (!validationResult.IsValid)
            {
                var customerErrs = validationResult.Errors.Select(x => new { x.PropertyName, x.ErrorMessage }).ToList();
                return BadRequest(new { validationResult.IsValid, errors = customerErrs });
            }

            var result = await _eventService.UpdateVenueAsync(venueDto);
            return Ok(result);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteVenue(VenueDto venueDto)
        {
            var validationResult = await _validator.ValidateAsync(venueDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _eventService.UpdateVenueAsync(venueDto);
            return Ok(result);
        }
    }
}
