using System;
using System.Threading.Tasks;

namespace WinRtCacheManager.Interfaces
{

    /// <summary>
    /// 
    /// </summary>
    public interface ICacheManager
    {

        /// <summary>
        /// Gets the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <returns></returns>
        Task<T> GetCache<T>(string token);
        /// <summary>
        /// Adds the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="value">The value.</param>
        /// <param name="expirationTimeSpan">The expiration time span.</param>
        /// <returns></returns>
        Task AddCache<T>(string token, T value, TimeSpan expirationTimeSpan);
        /// <summary>
        /// Adds the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cahce">The cahce.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        Task AddCache<T>(string token, T value);
        /// <summary>
        /// Adds the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cahce">The cahce.</param>
        /// <param name="value">The value.</param>
        /// <param name="renewCache">if set to <c>true</c> [renew cache].</param>
        /// <returns></returns>
        Task AddCache<T>(string token, T value, bool renewCache);

        Task DeleteCache(string token);
    }
}
