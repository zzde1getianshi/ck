using System;

namespace Pixeval.Core
{
    public class EnumeratingSchedule
    {
        private static object _currentItr;

        public static void StartNewInstance<T>(IPixivAsyncEnumerable<T> itr)
        {
            var iterator = _currentItr as IPixivAsyncEnumerable<T>;
            iterator?.Cancel();
            GC.Collect();
            AppContext.DefaultCacheProvider.Clear();
            _currentItr = itr;
        }

        public static IPixivAsyncEnumerable<T> GetCurrentEnumerator<T>()
        {
            return _currentItr as IPixivAsyncEnumerable<T>;
        }

        public static void CancelCurrent()
        {
            var iterator = _currentItr as ICancellable;
            iterator?.Cancel();
            GC.Collect();
            AppContext.DefaultCacheProvider.Clear();
        }
    }
}