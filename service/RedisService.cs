using EasyCaching.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Redis
{
    public class RedisService
    {
        private IEasyCachingProvider cachingProvider;
        private IEasyCachingProviderFactory cachingProviderFactory;

        public RedisService(IEasyCachingProviderFactory cachingProviderFactory)
        {
            this.cachingProviderFactory = cachingProviderFactory;
            this.cachingProvider = this.cachingProviderFactory.GetCachingProvider("redis1");
        }

        public void setItem(string key,byte[] value)
        {
            this.cachingProvider.Set(key, value, TimeSpan.FromDays(10));
        }

        public byte[] getItem(string key)
        {
            if (cachingProvider.Exists(key))
            {
                var item = this.cachingProvider.Get<byte[]>(key).Value;
                return item;
            }
            return null;
        }
    }
}
