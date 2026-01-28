using EventManagementAPI.Commons;
using EventManagementAPI.Helpers;
using EventManagementAPI.Services.Interfaces;
using EventManagementAPI.ViewModels;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
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
                return BadRequest(EventMgtSingleton.Instance.GetValidationResultErrorMessage(validationResult));
            }

            await _eventService.AddVenueAsync(venueDto);
            var apiResponse = EventMgtSingleton.Instance.GetApiResponse(venueDto, ApiResponseStatus.Success, "");
            return CreatedAtAction(nameof(GetVenueById), new { id = venueDto.Id }, apiResponse);
        }

        [HttpGet("{id}", Name = "GetVenueById")]
        [ProducesResponseType(typeof(VenueDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(VenueDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVenueById(int id)
        {
            var venue = await _eventService.GetVenueByIdAsync(id);
            if (venue == null) return NotFound();
            var apiResponse = EventMgtSingleton.Instance.GetApiResponse(venue, ApiResponseStatus.Success, "");
            return Ok(apiResponse);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<VenueDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVenues()
        {
            var venues = await _eventService.GetVenuesAsync();
            var apiResponse = EventMgtSingleton.Instance.GetApiResponse(venues, ApiResponseStatus.Success, "");
            return Ok(apiResponse);
        }

        [HttpGet]
        [Route("search")]
        [ProducesResponseType(typeof(List<VenueDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchVenues([FromQuery] string venueName = "", [FromQuery] string desc = "",
            [FromQuery] int minCapacity = 0, [FromQuery] int maxCapacity = 0, bool isActive = true)
        {
            if (string.IsNullOrWhiteSpace(venueName) && string.IsNullOrWhiteSpace(desc) && minCapacity == 0 && maxCapacity == 0 && isActive)
            {
                return BadRequest("Please provide search params to process.");
            }

            var venues = await _eventService.SearchVenuesAsync(venueName, desc, minCapacity, maxCapacity, isActive);
            var apiResponse = EventMgtSingleton.Instance.GetApiResponse(venues, ApiResponseStatus.Success, "");
            return Ok(apiResponse);
        }

        [HttpPut]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateVenue(VenueDto venueDto)
        {
            var validationResult = await _validator.ValidateAsync(venueDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(EventMgtSingleton.Instance.GetValidationResultErrorMessage(validationResult));
            }

            var result = await _eventService.UpdateVenueAsync(venueDto);
            var apiResponse = EventMgtSingleton.Instance.GetApiResponse(result, ApiResponseStatus.Success, "");
            return Ok(apiResponse);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteVenue(int venueId)
        {
            var result = await _eventService.DeleteVenueAsync(venueId);
            var apiResponse = EventMgtSingleton.Instance.GetApiResponse(result, ApiResponseStatus.Success, "");
            return Ok(apiResponse);
        }
    }
}
