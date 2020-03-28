using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Pixeval.Objects.Caching
{
    public class FileCache<T, THash> : IWeakCacheProvider<T, THash>, IEnumerable<KeyValuePair<THash, string>> where T : class
    {
        private readonly Func<T, Stream> cachingPolicy;
        private readonly Func<Stream, T> restorePolicy;
        private readonly string initDirectory;
        private readonly ConcurrentDictionary<THash, string> fileMapping = new ConcurrentDictionary<THash, string>();

        public FileCache(string initDirectory, Func<T, Stream> cachingPolicy, Func<Stream, T> restorePolicy)
        {
            this.cachingPolicy = cachingPolicy;
            this.restorePolicy = restorePolicy;
            this.initDirectory = initDirectory;

            Directory.CreateDirectory(initDirectory);
        }
        
        public void Attach(ref T key, THash associateWith)
        {
            if (associateWith == null || key == null) return;
            var path = Path.Combine(initDirectory, IWeakCacheProvider<T, THash>.HashKey(associateWith) + ".tmp");
            if (!File.Exists(path))
            {
                var s = cachingPolicy(key);
                key = null;
                Task.Run(() =>
                {
                    fileMapping.TryAdd(associateWith, path);
                    WriteFile(path, s);
                });
            }
        }

        private static async void WriteFile(string path, Stream src)
        {
            await using var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            src.Position = 0L;
            await src.CopyToAsync(fileStream);
        }

        public void Detach(THash associateWith)
        {
            using var sem = new SemaphoreSlim(1);
            var path = Path.Combine(initDirectory, IWeakCacheProvider<T, THash>.HashKey(associateWith) + ".");
            if (File.Exists(path))
            {
                fileMapping.TryRemove(associateWith, out _);
                File.Delete(path);
            }
        }

        public async Task<(bool, T)> TryGet([NotNull] THash key)
        {
            if (fileMapping.TryGetValue(key, out var file) && File.Exists(file))
            {
                await using var fileStream = File.OpenRead(file);
                fileStream.Position = 0L;
                Stream memoStream = new MemoryStream();
                await fileStream.CopyToAsync(memoStream);
                return (true, restorePolicy(memoStream));
            }

            return (false, null);
        }
        
        public void Clear()
        {
            using var sem = new SemaphoreSlim(1);
            foreach (var file in Directory.GetFiles(initDirectory))
            {
                File.Delete(file);
            }
        }

        public IEnumerator<KeyValuePair<THash, string>> GetEnumerator()
        {
            return fileMapping.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}