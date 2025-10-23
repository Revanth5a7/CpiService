using Microsoft.Extensions.Caching.Memory;
using CpiService.Models;

namespace CpiService.Services
{
    public class CpiCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly BlsApiService _blsService;
        private readonly ILogger<CpiCacheService> _logger;

        public CpiCacheService(IMemoryCache cache, BlsApiService blsService, ILogger<CpiCacheService> logger)
        {
            _cache = cache;
            _blsService = blsService;
            _logger = logger;
        }

        public async Task<CpiResponse?> GetCpiAsync(int year, string month)
        {
            string cacheKey = $"{year}-{month}";
            if (_cache.TryGetValue(cacheKey, out CpiResponse cachedResponse))
            {
                _logger.LogInformation($"Cache hit for {cacheKey}");
                return cachedResponse;
            }

            var result = await _blsService.GetCpiDataAsync(year, month);
            if (result != null)
            {
                _cache.Set(cacheKey, result, TimeSpan.FromDays(1));
            }

            return result;
        }
    }
}
