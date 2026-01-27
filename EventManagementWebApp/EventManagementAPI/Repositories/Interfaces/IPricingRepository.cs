using EventManagementAPI.Domain.Entities;

namespace EventManagementAPI.Repositories.Interfaces
{
    public interface IPricingRepository
    {
        Task<PricingTier?> GetPricingTierByIdAsync(long id);
        Task<List<PricingTier>> GetAllAsync(bool includeDeactive = false);
        Task AddPricingTierAsync(PricingTier pricingTier);
        Task UpdateAsync(PricingTier pricingTier);
        Task DeleteAsync(PricingTier pricingTier);
    }
}
