using EventManagementAPI.Data;
using EventManagementAPI.Domain.Entities;
using EventManagementAPI.Repositories.Interfaces;
using EventManagementAPI.Services.Interfaces;
using EventManagementAPI.ViewModels;

namespace EventManagementAPI.Services
{
    public class PricingService : IPricingService
    {
        private readonly IPricingRepository _pricingRepository;
        private readonly AppDbContext _dbContext;
        private const string PricingTierIsNotExisted = "Pricing Tier is not existed.";

        public PricingService(IPricingRepository pricingRepository, AppDbContext context)
        {
            _pricingRepository = pricingRepository;
            _dbContext = context;
        }

        public async Task AddPricingTierAsync(PricingTierDto pricingTierDto)
        {
            var pricingTier = new PricingTier()
            {
                Name = pricingTierDto.Name,
                Description = pricingTierDto.Description,
                Price = pricingTierDto.Price,
                IsActive = true
            };
            await _pricingRepository.AddPricingTierAsync(pricingTier);
            await _dbContext.SaveChangesAsync();

            pricingTierDto.Id = pricingTier.Id;
        }

        public async Task<bool> DeletePricingTierAsync(int id)
        {
            var pricingItem = await _pricingRepository.GetPricingTierByIdAsync(id);
            if (pricingItem == null) throw new ArgumentException(PricingTierIsNotExisted);

            await _pricingRepository.DeleteAsync(pricingItem);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<PricingTierDto> GetPricingTierByIdAsync(long id)
        {
            var pricingItem = await _pricingRepository.GetPricingTierByIdAsync(id);
            if (pricingItem == null) throw new ArgumentException(PricingTierIsNotExisted);

            return MapPricingTier(pricingItem);
        }

        public async Task<List<PricingTierDto>> GetPricingTiersAsync(bool includeDeactive = false)
        {
            var items = await _pricingRepository.GetAllAsync(includeDeactive);
            return items.Select(MapPricingTier).ToList();
        }

        public async Task<bool> UpdatePricingTierAsync(PricingTierDto pricingTierDto)
        {
            var pricingItem = await _pricingRepository.GetPricingTierByIdAsync(pricingTierDto.Id);
            if (pricingItem == null) throw new ArgumentException(PricingTierIsNotExisted);

            pricingItem.Name = pricingTierDto.Name;
            pricingItem.Description = pricingTierDto.Description;
            pricingItem.Price = pricingTierDto.Price;
            pricingItem.IsActive = pricingTierDto.IsActive;

            await _pricingRepository.UpdateAsync(pricingItem);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private PricingTierDto MapPricingTier(PricingTier priceItem)
        {
            return new PricingTierDto()
            {
                Id = priceItem.Id,
                Name = priceItem.Name,
                Description = priceItem.Description,
                Price = priceItem.Price,
                IsActive = priceItem.IsActive
            };
        }
    }
}
