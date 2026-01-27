using EventManagementAPI.ViewModels;

namespace EventManagementAPI.Services.Interfaces
{
    public interface IPricingService
    {
        Task AddPricingTierAsync(PricingTierDto pricingTierDto);
        Task<PricingTierDto> GetPricingTierByIdAsync(long id);
        Task<bool> UpdatePricingTierAsync(PricingTierDto pricingTierDto);
        Task<bool> DeletePricingTierAsync(int id);
        Task<List<PricingTierDto>> GetPricingTiersAsync(bool includeDeactive = false);
    }
}
