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
        private readonly ITicketService _ticketService;
        private IValidator<EventDto> _validator;
        private IValidator<PurchaseTicketDto> _purchaseTicketValidator;

        public EventController(IEventService eventService, ITicketService ticketService, IValidator<EventDto> validator,
            IValidator<PurchaseTicketDto> purchaseTicketValidator)
        {
            _eventService = eventService;
            _ticketService = ticketService;
            _validator = validator;
            _purchaseTicketValidator = purchaseTicketValidator;
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

        [HttpPut]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateEvent(EventDto eventDto)
        {
            if (eventDto == null || eventDto.Id <= 0) return BadRequest();

            var validationResult = await _validator.ValidateAsync(eventDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(EventMgtSingleton.Instance.GetValidationResultErrorMessage(validationResult));
            }

            await _eventService.UpdateEventAsync(eventDto);
            var apiResponse = EventMgtSingleton.Instance.GetApiResponse(eventDto, ApiResponseStatus.Success, "Updated successful.");
            return Ok(apiResponse);
        }

        [HttpDelete]
        [Route("{eventId}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteEvent(long eventId)
        {
            if (eventId <= 0) return BadRequest();

            var result = await _eventService.DeleteEventAsync(eventId);
            var apiResponse = EventMgtSingleton.Instance.GetApiResponse(result,
                result ? ApiResponseStatus.Success : ApiResponseStatus.Failed,
                result ? "Delete successful." : "Delete failed.");
            return Ok(apiResponse);
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

        [HttpPost]
        [Route("ticket")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> PurchaseTicket(PurchaseTicketDto purchaseTicketDto)
        {
            var validationResult = await _purchaseTicketValidator.ValidateAsync(purchaseTicketDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(EventMgtSingleton.Instance.GetValidationResultErrorMessage(validationResult));
            }

            var result = await _ticketService.PurchaseTicketAsync(purchaseTicketDto);
            var message = result ? "Purchase successful." : "Purchase failed, please try again later.";
            var apiResponse = EventMgtSingleton.Instance.GetApiResponse(result,
                result ? ApiResponseStatus.Success : ApiResponseStatus.Failed, message);
            return Ok(apiResponse);
        }

        [HttpGet]
        [Route("tickets")]
        [ProducesResponseType(typeof(TicketAvailabilityPayLoad), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTicketsAvailability([FromQuery] int pageNumber = 0, [FromQuery] int pageSize = 10)
        {
            var results = await _eventService.GetTicketAvailabilityAsync(pageNumber, pageSize);
            var payload = new TicketAvailabilityPayLoad()
            {
                Events = results.ticketAvailabilities,
                TotalCount = results.totalCount
            };
            var apiResponse = EventMgtSingleton.Instance.GetApiResponse(payload, ApiResponseStatus.Success, "");
            return Ok(apiResponse);
        }
    }
}
