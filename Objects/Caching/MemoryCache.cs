using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pixeval.Objects.Caching
{
    public class MemoryCache<T, THash> : IWeakCacheProvider<T, THash>, IEnumerable<KeyValuePair<int, WeakEntry<T>>> where T : class
    {
        public static readonly MemoryCache<T, THash> Shared = new MemoryCache<T, THash>();
        
        private readonly ConcurrentDictionary<int, WeakEntry<T>> cache = new ConcurrentDictionary<int, WeakEntry<T>>();
        
        public void Attach(ref T key, THash associateWith)
        {
            if (associateWith == null || key == null) return;
            var weakRef = new WeakEntry<T>(key);
            key = null;
            cache.TryAdd(IWeakCacheProvider<T, THash>.HashKey(associateWith), weakRef);
        }

        public void Detach(THash associateWith)
        {
            cache.TryRemove(IWeakCacheProvider<T, THash>.HashKey(associateWith), out _);
        }

        public Task<(bool, T)> TryGet([NotNull] THash key)
        {
            return cache.TryGetValue(IWeakCacheProvider<T, THash>.HashKey(key), out var weakRef) && weakRef.Target is { } target
                ? Task.FromResult((true, target))
                : Task.FromResult((false, (T) null));
        }

        public void Clear()
        {
            lock (cache) cache.Clear();
        }
        
        public IEnumerator<KeyValuePair<int, WeakEntry<T>>> GetEnumerator()
        {
            return cache.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}