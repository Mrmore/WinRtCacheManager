using System;

namespace WinRtCacheManager
{
    /// <summary>
    /// 
    /// </summary>
    public class CacheKey
    {


        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get;  set; }
        /// <summary>
        /// Gets or sets the expiration date time.
        /// </summary>
        /// <value>
        /// The expiration date time.
        /// </value>
        public DateTime ExpirationDateTime { get; set; }

    }

}