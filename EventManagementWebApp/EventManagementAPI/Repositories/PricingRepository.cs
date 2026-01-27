using EventManagementAPI.Data;
using EventManagementAPI.Domain.Entities;
using EventManagementAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventManagementAPI.Repositories
{
    public class PricingRepository: IPricingRepository
    {
        private readonly AppDbContext _context;

        public PricingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddPricingTierAsync(PricingTier pricingTier)
        {
            await _context.PricingTiers.AddAsync(pricingTier);
        }

        public async Task DeleteAsync(PricingTier pricingTier)
        {
            _context.PricingTiers.Remove(pricingTier);
        }

        public async Task<List<PricingTier>> GetAllAsync(bool includeDeactive = false)
        {
            return await _context.PricingTiers.Where(p => includeDeactive || p.IsActive).ToListAsync();
        }

        public async Task<PricingTier?> GetPricingTierByIdAsync(long id)
        {
            return await _context.PricingTiers.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(PricingTier pricingTier)
        {
            _context.PricingTiers.Update(pricingTier);
        }
    }
}
