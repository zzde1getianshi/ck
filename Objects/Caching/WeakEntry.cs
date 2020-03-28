using System;
using System.Runtime.InteropServices;

#nullable enable

namespace Pixeval.Objects.Caching
{
    public class WeakEntry<T> : IEquatable<WeakEntry<T>>, IDisposable where T : class
    {
        private readonly int hashCode;

        private GCHandle gcHandle;

        public WeakEntry(T target)
        {
            hashCode = target.GetHashCode();
            gcHandle = GCHandle.Alloc(target, GCHandleType.Weak);
        }

        public bool IsAlive => gcHandle.Target != null;

        public T? Target => gcHandle.Target as T;

        public void Dispose()
        {
            gcHandle.Free();
            GC.SuppressFinalize(this);
        }

        ~WeakEntry() => Dispose();

        public override bool Equals(object? obj)
        {
            if (obj is WeakEntry<T> weakEntry)
                return Equals(weakEntry);
            return false;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

        public bool Equals(WeakEntry<T>? other)
        {
            return other != null && ReferenceEquals(other.Target, Target);
        }
    }
}