using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SEO.Core.Model;

namespace SEO.Core.Process.Processing
{
    public class RedisCache : IRedisCache
    {
        private readonly IDistributedCache _redisDB;
        private RedisCacheSettings _redisCacheSettings;

        public RedisCache(IDistributedCache redisDB, IOptions<RedisCacheSettings> redisCacheSettings)
        {
            _redisCacheSettings = redisCacheSettings.Value;
            _redisDB = redisDB;
        }

        public async Task<T> Get<T>(string key)
        {
            try
            {
                var value = await _redisDB.GetAsync(string.Format(_redisCacheSettings.RedisFormat, _redisCacheSettings.Category, key));
                if (value != null)
                {
                    var cachedMessage = Encoding.UTF8.GetString(value);
                    return JsonConvert.DeserializeObject<T>(cachedMessage);
                }
                else
                {
                    return default(T);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task Set(string key, object value, int expirationInSeconds)
        {
            var distributedOptions = new DistributedCacheEntryOptions(); // create options object
            distributedOptions.SetSlidingExpiration(TimeSpan.FromSeconds(_redisCacheSettings.Timeout));
            await _redisDB.SetStringAsync(string.Format(_redisCacheSettings.RedisFormat, _redisCacheSettings.Category, key), JsonConvert.SerializeObject(value), distributedOptions);
        }
    }
}
