using System.Threading.Tasks;

namespace Pixeval.Objects.Caching
{
    public interface IWeakCacheProvider<T, in THash> where T : class
    {
        void Attach(ref T key, THash associateWith);

        void Detach(THash associateWith);

        Task<(bool, T)> TryGet(THash key);

        void Clear();

        protected static int HashKey(THash key)
        {
            return key == null ? 0 : key.GetHashCode();
        }
    }
}