using EventManagementAPI.Commons;
using EventManagementAPI.Helpers;
using EventManagementAPI.Services.Interfaces;
using EventManagementAPI.ViewModels;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PricingController : ControllerBase
    {
        private readonly IPricingService _service;
        private IValidator<PricingTierDto> _validator;

        public PricingController(IPricingService pricingService, IValidator<PricingTierDto> validator)
        {
            _service = pricingService;
            _validator = validator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(PricingTierDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddPricingTier(PricingTierDto pricingTierDto)
        {
            var validationResult = await _validator.ValidateAsync(pricingTierDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(EventMgtSingleton.Instance.GetValidationResultErrorMessage(validationResult));
            }

            await _service.AddPricingTierAsync(pricingTierDto);
            var apiResponse = EventMgtSingleton.Instance.GetApiResponse(pricingTierDto, ApiResponseStatus.Success, "");
            return CreatedAtAction(nameof(GetPricingTierById), new { id = pricingTierDto.Id }, apiResponse);
        }

        [HttpGet("{id}", Name = "GetPricingTierById")]
        [ProducesResponseType(typeof(PricingTierDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PricingTierDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPricingTierById(int id)
        {
            var result = await _service.GetPricingTierByIdAsync(id);
            if (result == null) return NotFound();
            var apiResponse = EventMgtSingleton.Instance.GetApiResponse(result, ApiResponseStatus.Success, "");
            return Ok(apiResponse);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<PricingTierDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPricingTier([FromQuery] string inclInactive = "")
        {
            var includeInactive = string.Equals(inclInactive, "Y", StringComparison.OrdinalIgnoreCase);
            var pricingTiers = await _service.GetPricingTiersAsync(includeInactive);
            var apiResponse = EventMgtSingleton.Instance.GetApiResponse(pricingTiers, ApiResponseStatus.Success, "");
            return Ok(apiResponse);
        }

        [HttpPut]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePricingTier(PricingTierDto pricingTierDto)
        {
            var validationResult = await _validator.ValidateAsync(pricingTierDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(EventMgtSingleton.Instance.GetValidationResultErrorMessage(validationResult));
            }

            var result = await _service.UpdatePricingTierAsync(pricingTierDto);
            var apiResponse = EventMgtSingleton.Instance.GetApiResponse(result, result ? ApiResponseStatus.Success : ApiResponseStatus.Failed, "");
            return Ok(apiResponse);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeletePricingTier(int pricingTierId)
        {
            var result = await _service.DeletePricingTierAsync(pricingTierId);
            var apiResponse = EventMgtSingleton.Instance.GetApiResponse(result, result ? ApiResponseStatus.Success : ApiResponseStatus.Failed, "");
            return Ok(apiResponse);
        }
    }
}
