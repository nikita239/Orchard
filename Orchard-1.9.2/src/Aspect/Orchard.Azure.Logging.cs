using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspectDotNet;
using Microsoft.ApplicationServer.Caching;
using NHibernate;
using NHibernate.Cache;
using NHibernate.Cfg.Loquacious;
using Orchard;
using Orchard.Azure.Services.Caching.Database;
using Orchard.Azure.Services.Caching.Output;
using Orchard.Azure.Services.Environment.Configuration;
using Orchard.Environment.Configuration;
using Orchard.Logging;
using Orchard.OutputCache.Models;

namespace Aspect
{
    public class Logging : AspectDotNet.Aspect
    {
        private const string DefaultRegion = "NHibernate";
        private static IInternalLogger _logger;

        private static string _region;
        private static string _regionAlphaNumeric1;
        private static string _regionAlphaNumeric2;
        public static ILogger Logger { get; set; }


        //конструктор не внедряется
        [AspectAction("%after %call *AzureCacheClient.ctor(DataCache, string, TimeSpan?) && args(..)")]
        public static void AzureCacheClient(DataCache cache, string region, TimeSpan? expirationTime)
        {
            _logger = LoggerProvider.LoggerFor(typeof(AzureCacheClient));
            _regionAlphaNumeric1 = new String(Array.FindAll(_region.ToCharArray(), Char.IsLetterOrDigit)) + _region.GetHashCode().ToString(CultureInfo.InvariantCulture);
            _region = region ?? DefaultRegion;
            if (_logger.IsDebugEnabled)
            {
                _logger.DebugFormat("Created an AzureCacheClient for region '{0}' (original region '{1}').", _regionAlphaNumeric1, _region);
            }
        }


        //метод не внедряется
        [AspectAction("%before %call *DataCache.Get(string, string) && %withincode(*AzureCacheClient.Get(object))")]
        public static void Get(object key)
        {
            if (_logger.IsDebugEnabled)
                _logger.DebugFormat("Get() invoked with key='{0}' in region '{1}'.", key, _regionAlphaNumeric1);
        }

        //метод внедряется
        //[AspectAction("%before %call *get_HasValue() && %withincode(*.Put)")]
        public static void Put(object key, object value)
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.DebugFormat("Remove() invoked with key='{0}' in region '{1}'.", key, _regionAlphaNumeric1);
            }
        }

        //метод не внедряется
        [AspectAction("%before %call *DataCache.Remove(string, string) && %withincode(*AzureCacheClient.Remove(object))")]
        public static void Remove(string key, string value)
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.DebugFormat("Remove() invoked with key='{0}' in region '{1}'.", key, _regionAlphaNumeric1);
            }
        }

        //метод внедряется
        // [AspectAction("%before %call *DataCache.ClearRegion(string) && %withincode(*AzureCacheClient.Clear())")]
        public static void Clear()
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.DebugFormat("Clear() invoked in region '{0}'.", _regionAlphaNumeric1);
            }
        }

        //метод внедряется
        // [AspectAction("%before %call *AzureCacheClient.Clear() && %withincode(*AzureCacheClient.Destroy())")]
        public static void Destroy()
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.DebugFormat("Destroy() invoked in region '{0}'.", _regionAlphaNumeric1);
            }
        }
        //метод не внедряется
        [AspectAction("%before %call *DataCache.Put(string, object, TimeSpan, string) && %withincode(*AzureOutputCacheStorageProvider.Set(string, CacheItem))")]
        public static void Set(string key, CacheItem cacheItem)
        {
            Logger.Debug("Set() invoked with key='{0}' in region '{1}'.", key, _regionAlphaNumeric2);
        }

        //метод не внедряется
        [AspectAction("%before %call *DataCache.Remove(string, string) && %withincode(*AzureOutputCacheStorageProvider.Remove(string))")]
        public static void Remove(string key)
        {
            Logger.Debug("Remove() invoked with key='{0}' in region '{1}'.", key, _regionAlphaNumeric2);
        }

        //метод внедряется 
        //[AspectAction("%before %call *DataCache.ClearRegion(string) && %withincode(*AzureOutputCacheStorageProvider.RemoveAll())")]  
        public static void RemoveAll()
        {
            Logger.Debug("RemoveAll() invoked in region '{0}'.", _regionAlphaNumeric2);
        }

    }

}
