using Microsoft.Extensions.Caching.Memory;
using System;

namespace ContentManagement.Common.WebToolkit
{
    // https://github.com/tpodolak/Blog/tree/master/AspNetCoreMemoryCacheIsPopulatedMultipleTimes
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public T GetOrAdd<T>(string cacheKey, Func<T> factory, DateTime absoluteExpiration)
        {
            // locks get and set internally but call to factory method is not locked
            return _memoryCache.GetOrCreate(cacheKey, entry => factory());
        }
    }

    public interface ICacheService
    {
        T GetOrAdd<T>(string cacheKey, Func<T> factory, DateTime absoluteExpiration);
    }

    public class LockedFactoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public LockedFactoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public T GetOrAdd<T>(string cacheKey, Func<T> factory, DateTime absoluteExpiration)
        {
            // locks get and set internally
            if (_memoryCache.TryGetValue<T>(cacheKey, out var result))
            {
                return result;
            }

            lock (TypeLock<T>.Lock)
            {
                if (_memoryCache.TryGetValue(cacheKey, out result))
                {
                    return result;
                }

                result = factory();
                _memoryCache.Set(cacheKey, result, absoluteExpiration);

                return result;
            }
        }

        private static class TypeLock<T>
        {
            public static object Lock { get; } = new object();
        }
    }
}
