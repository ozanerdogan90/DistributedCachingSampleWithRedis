using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace DistributedCachingSampleWithRedis.Core.Caching
{
    public interface IMemoryCacher : ICacher { }

    public class MemoryCacher : IMemoryCacher
    {
        private readonly IMemoryCache cacher;
        private readonly TimeSpan defaultTimeSpan = TimeSpan.FromHours(1);
        public MemoryCacher(IMemoryCache cache)
        {
            cacher = cache;
        }

        public bool ClearCache(string key)
        {
            this.cacher.Remove(key);
            return true;
        }
        public T Get<T>(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;
            if (this.cacher.TryGetValue<T>(id, out T value))
            {
                return value;
            }

            return default;
        }

        public bool Save<T>(T obj, string id, TimeSpan? expiresIn = null)
        {
            if (obj == null) return false;
            this.cacher.Set(id, obj, expiresIn.HasValue ? expiresIn.Value : defaultTimeSpan);
            return true;
        }

    }
}
