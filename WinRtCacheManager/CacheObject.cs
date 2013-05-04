using System.Collections.Generic;

namespace WinRtCacheManager
{
    public class CacheObject
    {

        private List<CacheKey> _expirations = new List<CacheKey>();
        private Dictionary<string, string> _cache = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the expirations.
        /// </summary>
        /// <value>
        /// The expirations.
        /// </value>
        public List<CacheKey> Expirations
        {
            get { return _expirations; }
            set { _expirations = value; }
        }

        /// <summary>
        /// Gets or sets the cache.
        /// </summary>
        /// <value>
        /// The cache.
        /// </value>
        public Dictionary<string, string> Cache
        {
            get { return _cache; }
            set { _cache = value; }
        }

        
    }
}