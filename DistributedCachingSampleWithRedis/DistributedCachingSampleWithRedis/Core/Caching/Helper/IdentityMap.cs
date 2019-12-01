
namespace DistributedCachingSampleWithRedis.Core.Caching
{
    public static class IdentityMap
    {
        public static string CreateKey<T>()
        {
            string key = typeof(T).FullName.Replace(".", "_");
            return key;
        }

        public static string CreateKey<T>(string id)
        {
            string key = typeof(T).FullName.Replace(".", "_");
            key = key + "_" + id;
            return key;
        }
    }
}
