#region Namespaces
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;
using WinRtCacheManager.Interfaces;
#endregion
namespace WinRtCacheManager
{
    /// <summary>
    /// 
    /// </summary>
    public class CacheManager : ICacheManager
    {
        #region Implementation of ICacheManager

        private readonly string _name;

        /// <summary>
        /// Gets the expiration time span.
        /// </summary>
        /// <value>
        /// The expiration time span.
        /// </value>
        public TimeSpan ExpirationTimeSpan { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheManager" /> class.
        /// </summary>
        public CacheManager()
        {
            _name = Package.Current.Id.Name;
            ExpirationTimeSpan = new TimeSpan(0, 0, 15);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheManager" /> class.
        /// </summary>
        /// <param name="expirationTimeSpan">The expiration time span.</param>
        public CacheManager(TimeSpan expirationTimeSpan)
        {
            _name = Package.Current.Id.Name;
            ExpirationTimeSpan = expirationTimeSpan;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheManager" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public CacheManager(string name)
        {
            _name = name;
            ExpirationTimeSpan = new TimeSpan(0, 0, 15);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheManager" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="expirationTimeSpan">The expiration time span.</param>
        public CacheManager(string name, TimeSpan expirationTimeSpan)
        {
            //var currentAssembly = this.GetType().GetTypeInfo().Assembly;
            //// we filter the defined classes according to the interfaces they implement
            //var iDisposableAssemblies = currentAssembly.DefinedTypes.Where(type => type.ImplementedInterfaces.Any(inter => inter == typeof(ICacheManager))).ToList();
            _name = name;
            ExpirationTimeSpan = expirationTimeSpan;

        }

        

        /// <summary>
        /// Creates the or get cache file.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private async Task<StorageFile> CreateOrGetCacheFile(string name)
        {
            StorageFile cacheFile = null;
            var storageFolder = ApplicationData.Current.LocalFolder;
            var fileExits = false;
            try
            {
                cacheFile = await storageFolder.GetFileAsync(name + ".dat").AsTask().ConfigureAwait(false);
                fileExits = true;
            }
            catch (FileNotFoundException)
            {

            }

            if (!fileExits)
            {

                cacheFile = await storageFolder.CreateFileAsync(name + ".dat", CreationCollisionOption.ReplaceExisting).AsTask().ConfigureAwait(false);
                await SaveCacheObjec(new CacheObject()).ConfigureAwait(false);
            }

            return cacheFile;

        }



        /// <summary>
        /// Adds the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The cache.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public Task AddCache<T>(string token, T value)
        {
            return AddCache(token, value, ExpirationTimeSpan);
        }

        /// <summary>
        /// Adds the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The cache.</param>
        /// <param name="value">The value.</param>
        /// <param name="renewCache">if set to <c>true</c> [renew cache].</param>
        /// <returns></returns>
        public Task AddCache<T>(string token, T value, bool renewCache)
        {
            return AddCache(token, value, ExpirationTimeSpan, renewCache);
        }

        public async Task DeleteCache(string token)
        {
            var cacheObject = await GetCacheObjec().ConfigureAwait(false);
            if (cacheObject != null)
            {
                if (cacheObject.Expirations.Any(e => e.Key == token))
                {
                    var indexToRemove = cacheObject.Expirations.FindIndex(e => e.Key == token);
                    cacheObject.Expirations.RemoveAt(indexToRemove);
                }
                if (cacheObject.Cache.Keys.Contains(token))
                {
                    cacheObject.Cache.Remove(token);
                }
            }
            SaveCacheObjec(cacheObject);
        }

        /// <summary>
        /// Adds the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The cache.</param>
        /// <param name="value">The value.</param>
        /// <param name="expirationTimeSpan">The expiration time span.</param>
        /// <returns></returns>
        public Task AddCache<T>(string token, T value, TimeSpan expirationTimeSpan)
        {
            return AddCache(token, value, expirationTimeSpan, true);
        }

        /// <summary>
        /// Adds the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The cache.</param>
        /// <param name="value">The value.</param>
        /// <param name="expirationTimeSpan">The expiration time span.</param>
        /// <param name="renewCache">if set to <c>true</c> [renew cache].</param>
        /// <returns></returns>
        private async Task AddCache<T>(string token, T value, TimeSpan expirationTimeSpan, bool renewCache)
        {
            ExpirationTimeSpan = expirationTimeSpan;
            var cacheObject = await GetCacheObjec().ConfigureAwait(false);
            if (cacheObject != null)
            {
                if (renewCache)
                {
                    var expirations = cacheObject.Expirations;

                    if (expirations.Any(e => e.Key == token))
                    {
                        var exp = expirations.FirstOrDefault(e => e.Key == token);
                        exp.ExpirationDateTime = DateTime.Now.Add(ExpirationTimeSpan);
                    }
                    else
                    {
                        expirations.Add(new CacheKey { Key = token, ExpirationDateTime = DateTime.Now });
                    }
                }

                if (!cacheObject.Cache.Keys.Contains(token))
                {
                    var obj = Newtonsoft.Json.JsonConvert.SerializeObject(value);
                    cacheObject.Cache.Add(token, obj);
                }
                else
                {
                    var obj = Newtonsoft.Json.JsonConvert.SerializeObject(value);
                    cacheObject.Cache[token] = obj;
                }


                SaveCacheObjec(cacheObject);
            }
        }



        /// <summary>
        /// Gets the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The cache.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public async Task<T> GetCache<T>(string token)
        {
            var ret = default(T);

            var cacheObject = await GetCacheObjec().ConfigureAwait(false);
            if (cacheObject != null)
            {
                var expirations = cacheObject.Expirations;
                if (expirations.Any(e => e.Key == token))
                {
                    var exp = expirations.FirstOrDefault(e => e.Key == token);
                    if (exp.ExpirationDateTime.Add(ExpirationTimeSpan) < DateTime.Now)
                    {
                        expirations.Remove(exp);

                    }
                    else
                    {
                        var json = cacheObject.Cache[token];
                        ret = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
                    }

                }
            }
            await SaveCacheObjec(cacheObject).ConfigureAwait(false);

            return ret;
        }

        /// <summary>
        /// Gets the cache objec.
        /// </summary>
        /// <returns></returns>
        private async Task<CacheObject> GetCacheObjec()
        {
            var cacheFile = await CreateOrGetCacheFile(_name).ConfigureAwait(false);
            string cacheObjectJson;
            var buffer = await FileIO.ReadBufferAsync(cacheFile);
            using (var dataReader = DataReader.FromBuffer(buffer))
            {
                cacheObjectJson = dataReader.ReadString(buffer.Length);
            }

            return Newtonsoft.Json.JsonConvert.DeserializeObject<CacheObject>(cacheObjectJson);

        }

        /// <summary>
        /// Saves the cache objec.
        /// </summary>
        /// <param name="cacheObject">The cache object.</param>
        /// <returns></returns>
        private async Task SaveCacheObjec(CacheObject cacheObject)
        {
            var cacheFile = await CreateOrGetCacheFile(_name).ConfigureAwait(false);
            var buffer = GetBufferFromString(Newtonsoft.Json.JsonConvert.SerializeObject(cacheObject));
            await FileIO.WriteBufferAsync(cacheFile, buffer).AsTask().ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the buffer from string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        private static IBuffer GetBufferFromString(String str)
        {
            using (var memoryStream = new InMemoryRandomAccessStream())
            {
                using (var dataWriter = new DataWriter(memoryStream))
                {
                    dataWriter.WriteString(str);
                    return dataWriter.DetachBuffer();
                }
            }
        }



        #endregion
    }
}
