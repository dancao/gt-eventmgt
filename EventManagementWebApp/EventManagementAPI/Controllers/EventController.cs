using EventManagementAPI.Commons;
using EventManagementAPI.Helpers;
using EventManagementAPI.Services.Interfaces;
using EventManagementAPI.ViewModels;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private IValidator<EventDto> _validator;

        public EventController(IEventService eventService, IValidator<EventDto> validator)
        {
            _eventService = eventService;
            _validator = validator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddEvent(EventDto eventDto)
        {
            var validationResult = await _validator.ValidateAsync(eventDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(EventMgtSingleton.Instance.GetValidationResultErrorMessage(validationResult));
            }

            await _eventService.AddEventAsync(eventDto);
            var apiResponse = EventMgtSingleton.Instance.GetApiResponse(eventDto, ApiResponseStatus.Success, "");
            return CreatedAtAction(nameof(GetEventById), new { id = eventDto.Id }, apiResponse);
        }

        [HttpGet("{id}", Name = "GetEventById")]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEventById(int id, [FromQuery] string incVenue, [FromQuery] string incPricingTier)
        {
            if (id <= 0) return BadRequest();

            var includeVenue = string.Equals(incVenue, "Y", StringComparison.OrdinalIgnoreCase);
            var includePricingTier = string.Equals(incPricingTier, "Y", StringComparison.OrdinalIgnoreCase);
            var evt = await _eventService.GetEventByIdAsync(id, includeVenue, includePricingTier);

            if (evt == null) return NotFound();

            var apiResponse = EventMgtSingleton.Instance.GetApiResponse(evt, ApiResponseStatus.Success, "");
            return Ok(apiResponse);
        }
    }
}
