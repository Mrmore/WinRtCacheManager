using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinRtCacheManager.Common
{
    public static class Helper
    {
        public static async Task<TResult> GetData<TParameter, TResult>(this CacheManager cacheManager, string token, Func<TParameter, TResult> action, TParameter parameter)
        {

            var cacheResult = await cacheManager.GetCache<TResult>(token);
            if (cacheResult == null)
            {
                cacheResult = action(parameter);
                await cacheManager.AddCache(token, cacheResult);
            }

            return cacheResult;
        }
    }
}
