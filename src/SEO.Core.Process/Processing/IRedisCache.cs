using System.Threading.Tasks;

namespace SEO.Core.Process.Processing
{
    public interface IRedisCache
    {
        Task<T> Get<T>(string key);
        Task Set(string key, object value, int expirationInSeconds);
    }
}