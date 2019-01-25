using System;

namespace MemoryCacheT.Ex
{
    /// <summary>
    /// Base class for cache items.
    /// </summary>
    /// <typeparam name="TValue">Type of value in cache item.</typeparam>
    public abstract class CacheItem<TValue> : ICacheItem<TValue>
    {
        protected readonly IDateTimeProvider _dateTimeProvider;
        protected readonly TValue _cacheItemValue;
        protected readonly int _notificationTime;

        internal CacheItem(IDateTimeProvider dateTimeProvider, TValue value, int notificationTime)
        {
            _dateTimeProvider = dateTimeProvider;
            _cacheItemValue = value;
            _notificationTime = notificationTime;

            if ((notificationTime != int.MinValue) && (notificationTime < 1))
            {
                throw new ArgumentException("Notification time must be at least 1 second");
            }
        }

        public abstract ICacheItem<TValue> CreateNewCacheItem(TValue value);

        public abstract TValue Value { get; }

        public abstract TValue PeekValue { get; }

        public abstract bool IsExpired { get; }

        public abstract bool IsAboutToExpire { get; }

        public void Expire()
        {
            if (OnExpire != null)
            {
                OnExpire(_cacheItemValue, _dateTimeProvider.UtcNow);
            }
        }

        public void AboutToExpire()
        {
            if (OnAboutToExpire != null)
            {
                OnAboutToExpire(_cacheItemValue, _dateTimeProvider.UtcNow);
            }
        }

        public void Remove()
        {
            if (OnRemove != null)
            {
                OnRemove(_cacheItemValue, _dateTimeProvider.UtcNow);
            }
        }

        public Action<TValue, DateTime> OnExpire { get; set; }

        public Action<TValue, DateTime> OnAboutToExpire { get; set; }

        public Action<TValue, DateTime> OnRemove { get; set; }

    }
}