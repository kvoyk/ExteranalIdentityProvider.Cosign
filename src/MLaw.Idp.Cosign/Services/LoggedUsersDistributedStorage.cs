using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using MLaw.Idp.Cosign.Models;
using Newtonsoft.Json;

namespace MLaw.Idp.Cosign.Services
{
    public class LoggedUsersDistributedStorage : ILoggedUsersStorage
    {
        private readonly IDistributedCache _distributedCache;
        private readonly CacheKeyCreator _cacheKeyCreator;

        public LoggedUsersDistributedStorage(IDistributedCache distributedCache, CacheKeyCreator cacheKeyCreator)
        {
            _distributedCache = distributedCache;
            _cacheKeyCreator = cacheKeyCreator;
        }

        public async Task<string> SaveLoginAsync(LoggedinUserModel loggedinUserModel)
        {

            string cacheKey = _cacheKeyCreator.CreateKey();

            string json = JsonConvert.SerializeObject(loggedinUserModel);
            byte[] val = Encoding.UTF8.GetBytes(json);
            var distributedCacheEntryOptions = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(300));
            await _distributedCache.SetAsync(cacheKey, val, distributedCacheEntryOptions);

            return cacheKey;
        }
        
        public async Task<LoggedinUserModel> GetLoginAsync(string cacheKey)
        {

            byte[] value = await _distributedCache.GetAsync(cacheKey);
            if (value == null)
            {
                return null;
            }
            string val = Encoding.UTF8.GetString(value);
            LoggedinUserModel loggedinUserModel = JsonConvert.DeserializeObject<LoggedinUserModel>(val);


            return loggedinUserModel;

        }
    }
}
