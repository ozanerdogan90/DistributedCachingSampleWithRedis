using System;

namespace DistributedCachingSampleWithRedis.Core.Caching
{
    public interface ICacher
    {
        bool Save<T>(T obj, string id, TimeSpan? expiresIn = null);

        T Get<T>(string id);

        bool ClearCache(string key);

    }
}
