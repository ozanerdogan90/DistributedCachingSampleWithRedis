using Microsoft.Extensions.Configuration;
using System;
using Utf8Json;

namespace DistributedCachingSampleWithRedis.Core.Caching
{
    public interface ICacheManager
    {
        void Set<T>(string id, T value, TimeSpan? expireCacheIn = null);
        T GetOrCreate<T>(string id, Func<T> createFn = null);
    }

    public class CacheManager : ICacheManager
    {
        private readonly IMemoryCacher memoryCacher;
        private readonly IDistributedCacher distributedCacher;
        private readonly bool isDistributedCacheActive;
        public CacheManager(IMemoryCacher memoryCacher, IDistributedCacher distributedCacher, IConfiguration config)
        {
            this.memoryCacher = memoryCacher;
            this.distributedCacher = distributedCacher;
            this.isDistributedCacheActive = config.GetValue("DistributedCache:IsActive", false);
            BindDistributedCacheSubscription();
        }

        public void Set<T>(string id, T value, TimeSpan? expireCacheIn = null)
        {
            var key = IdentityMap.CreateKey<T>(id);
            if (isDistributedCacheActive)
                distributedCacher.Save<T>(value, key, expireCacheIn);
            memoryCacher.Save<T>(value, key, expireCacheIn);
        }

        public T GetOrCreate<T>(string id, Func<T> createFn = null)
        {
            var key = IdentityMap.CreateKey<T>(id);
            var value = memoryCacher.Get<T>(key);
            if (!Equals(value, default(T)))
            {
                return value;
            }

            if (isDistributedCacheActive)
            {
                var distValue = distributedCacher.Get<T>(key);
                if (!Equals(distValue, default(T)))
                {
                    this.memoryCacher.Save<T>(distValue, key);
                    value = distValue;
                }

            }

            if (Equals(value, default(T)))
            {
                if (createFn == null)
                    return default;
                value = createFn();
                if (isDistributedCacheActive)
                    distributedCacher.Save<T>(value, key);
                memoryCacher.Save<T>(value, key);
            }

            return value;
        }
        private void BindDistributedCacheSubscription()
        {
            if (!isDistributedCacheActive || this.distributedCacher.Subscriber == null)
                return;

            this.distributedCacher.Subscriber.Subscribe(distributedCacher.RedisChannel, (channel, message) =>
            {
                byte[] mssg = message;
                var messageItem = JsonSerializer.Deserialize<RedisItem>(mssg);
                if (messageItem.Value == null)
                {
                    this.memoryCacher.ClearCache(messageItem.Key);
                }
                else
                {
                    var newValue = JsonSerializer.Deserialize<object>(messageItem.Value);
                    SaveMessage(messageItem.Type, newValue, messageItem.Key);
                }

            });
        }

        private void SaveMessage<T>(T type, T value, string id) where T : class
        {
            this.memoryCacher.Save<T>(value, id);
        }
    }
}
