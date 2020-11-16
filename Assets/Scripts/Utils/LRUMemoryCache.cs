using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LRUMemoryCache<TItem>
{
    private const int cacheSizeLimit = 100;
    private Dictionary<object, TItem> _cache;
    private Dictionary<object, DateTime> _cacheTimes;

    public LRUMemoryCache()
    {
        _cache = new Dictionary<object, TItem>();
        _cacheTimes = new Dictionary<object, DateTime>();
    }

    public async Task<TItem> GetOrCreate(object key, Func<Task<TItem>> createItem)
    {
        TItem cacheEntry;
        if(!_cache.TryGetValue(key, out cacheEntry))
        {
            // Key not in cache, so get data
            cacheEntry = await createItem();

            if (_cache.Count >= cacheSizeLimit)
            {
                // Delete the oldest entry
                _cache.Remove(GetOldestKey());
            }

            _cache.Add(key, cacheEntry);
        }

        return cacheEntry;
    }

    private object GetOldestKey()
    {
        if(_cache.Count > 0)
        {
            object oldestKey = _cache[0];

            DateTime oldestTime = new DateTime(DateTime.MinValue.Ticks); // Set time to the oldest date possible

            foreach (KeyValuePair<object, DateTime> entry in _cacheTimes)
            {
                if (entry.Value.CompareTo(oldestTime) < 0)
                {
                    oldestTime = entry.Value;
                    oldestKey = entry.Key;
                }
            }

            return oldestKey;
        }

        return null;
    }
}
