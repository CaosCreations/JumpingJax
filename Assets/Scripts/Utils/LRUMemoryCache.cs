using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LRUMemoryCache<TItem>
{
    private const int cacheSizeLimit = 100;
    private ConcurrentDictionary<object, TItem> _cache;
    private ConcurrentDictionary<object, DateTime> _cacheTimes;

    public LRUMemoryCache()
    {
        _cache = new ConcurrentDictionary<object, TItem>();
        _cacheTimes = new ConcurrentDictionary<object, DateTime>();
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
                object oldestKey = GetOldestKey();

                if (oldestKey == null)
                {
                    Debug.LogError($"Could not get oldest key in cache");
                }
                else
                {
                    TItem valueRemoved;
                    if (!_cache.TryRemove(oldestKey, out valueRemoved))
                    {
                        Debug.LogError($"Could not remove key {key} from dictionary");
                    }

                    DateTime timeRemoved;
                    _cacheTimes.TryRemove(oldestKey, out timeRemoved);
                }
            }

            if(!_cache.TryAdd(key, cacheEntry))
            {
                Debug.LogError($"Could not add key {key} to dictionary");
            }

            _cacheTimes.TryAdd(key, DateTime.Now);
        }

        return cacheEntry;
    }

    private object GetOldestKey()
    {
        object oldestKey = null;

        DateTime oldestTime = new DateTime(DateTime.MinValue.Ticks); // Set time to the oldest date possible

        foreach (KeyValuePair<object, DateTime> entry in _cacheTimes)
        {
            if (entry.Value.CompareTo(oldestTime) > 0)
            {
                oldestTime = entry.Value;
                oldestKey = entry.Key;
            }
        }

        return oldestKey;
    }
}
