using EventManagementAPI.Commons;
using EventManagementAPI.Helpers;
using EventManagementAPI.Services.Interfaces;
using EventManagementAPI.ViewModels;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System;

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
        public async Task<IActionResult> GetEventById(int id, [FromQuery] string incVenue = "", [FromQuery] string incTicketTypes = "")
        {
            if (id <= 0) return BadRequest();

            var evt = await _eventService.GetEventByIdAsync(id, 
                                string.Equals(incVenue, "Y", StringComparison.OrdinalIgnoreCase),
                                string.Equals(incTicketTypes, "Y", StringComparison.OrdinalIgnoreCase));

            if (evt == null) return NotFound();

            var apiResponse = EventMgtSingleton.Instance.GetApiResponse(evt, ApiResponseStatus.Success, "");
            return Ok(apiResponse);
        }

        [HttpGet]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllEvents()
        {
            var results = await _eventService.GetAllEventsAsync();
            var apiResponse = EventMgtSingleton.Instance.GetApiResponse(results, ApiResponseStatus.Success, "");
            return Ok(apiResponse);
        }
    }
}
