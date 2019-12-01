using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using Utf8Json;

namespace DistributedCachingSampleWithRedis.Core.Caching
{
    public class RedisItem
    {
        public string Key;
        public Type Type;
        public byte[] Value;
        public DateTime ExpiresIn;
    }

    public interface IDistributedCacher : ICacher
    {
        ISubscriber Subscriber { get; }
        string RedisChannel { get; }
    }

    public class DistributedCacher : IDistributedCacher
    {
        private readonly IDistributedCache cacher;
        private readonly TimeSpan defaultTimeSpan = TimeSpan.FromHours(1);
        private readonly IConfiguration config;
        private readonly string _redisChannel = "cacheRefresh";
        private ISubscriber _subscriber;
        public DistributedCacher(IDistributedCache cacher, IConfiguration config)
        {
            this.config = config;
            this.cacher = cacher;
        }

        public ISubscriber Subscriber
        {
            get
            {
                if (_subscriber != null)
                    return _subscriber;
                var redis = ConnectionMultiplexer.Connect(config.GetValue("DistributedCache:Configuration", "localhost"));
                _subscriber = redis.GetSubscriber();
                return _subscriber;
            }
        }

        public string RedisChannel => this._redisChannel;

        public bool ClearCache(string key)
        {
            this.cacher.Remove(key);
            PublishMessage(new RedisItem() { Key = key, Value = null }); //// tell subscriber to remove value
            return true;
        }


        public T Get<T>(string id)
        {
            if (string.IsNullOrEmpty(id)) return default;

            var result = this.cacher.Get(id);
            if (Equals(result, default(T)))
                return default;
            return JsonSerializer.Deserialize<T>(result);
        }

        public bool Save<T>(T obj, string id, TimeSpan? expiresIn = null)
        {
            if (obj == default || string.IsNullOrEmpty(id)) return false;

            var value = JsonSerializer.Serialize<T>(obj);
            this.cacher.Set(id, value);
            PublishMessage(new RedisItem() { Key = id, Value = value, Type = typeof(T) });
            return true;
        }

        private void PublishMessage(RedisItem item)
        {
            if (this.Subscriber != null)
                this.Subscriber.Publish(RedisChannel, JsonSerializer.Serialize(item));
        }

    }
}
