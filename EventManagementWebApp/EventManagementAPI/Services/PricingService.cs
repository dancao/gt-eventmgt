using EventManagementAPI.Commons;
using EventManagementAPI.Data;
using EventManagementAPI.Domain.Entities;
using EventManagementAPI.Repositories.Interfaces;
using EventManagementAPI.Services.Interfaces;
using EventManagementAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Polly.CircuitBreaker;
using Polly.Registry;

namespace EventManagementAPI.Services
{
    public class PricingService : IPricingService
    {
        private readonly IPricingRepository _pricingRepository;
        private readonly AppDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly ResiliencePipelineProvider<string> _pipelineProvider;
        private const string PricingTierIsNotExisted = "Pricing Tier is not existed.";

        public PricingService(IPricingRepository pricingRepository, AppDbContext context, IMemoryCache cache,
            ResiliencePipelineProvider<string> provider)
        {
            _pricingRepository = pricingRepository;
            _dbContext = context;
            _memoryCache = cache;
            _pipelineProvider = provider;
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

        /// <summary>
        /// Apply Circuit Breaker pattern with Memory Cache, in Prod, we can use it with Redis/Distributed Cache
        /// </summary>
        /// <param name="includeDeactive"></param>
        /// <returns></returns>
        public async Task<List<PricingTierDto>> GetPricingTiersAsync(bool includeDeactive = false)
        {
            string cacheKey = $"all_pricing_tiers_{includeDeactive}";

            if (_memoryCache.TryGetValue(cacheKey, out List<PricingTierDto>? cachedData))
            {
                return cachedData!;
            }

            var pipeline = _pipelineProvider.GetPipeline(Constants.DbCircuitBreakerKey);

            try
            {
                return await pipeline.ExecuteAsync(async token =>
                {
                    var items = await _pricingRepository.GetAllAsync(includeDeactive);
                    var data = items.Select(MapPricingTier).ToList();
                    var options = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                        SlidingExpiration = TimeSpan.FromMinutes(2),
                        Priority = CacheItemPriority.Normal
                    };
                    _memoryCache.Set(cacheKey, data, options);
                    return data;
                });
            }
            catch (BrokenCircuitException)
            {
                // Fallback: If DB is down, try to return "Expired" cache if it exists 
                return _memoryCache.Get<List<PricingTierDto>>(cacheKey) ?? new List<PricingTierDto>();
            }
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
