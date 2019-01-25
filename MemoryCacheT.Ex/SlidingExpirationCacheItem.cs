using System;

namespace MemoryCacheT.Ex
{
    /// <summary>
    /// A cache item that expires if it has not been accessed in a given span of time.
    /// </summary>
    /// <typeparam name="TValue">Type of value in cache item.</typeparam>
    public class SlidingExpirationCacheItem<TValue> : CacheItem<TValue>
    {
        private readonly TimeSpan _cacheInterval;
        private DateTime? _lastAccessDateTime;

        internal SlidingExpirationCacheItem(IDateTimeProvider dateTimeProvider, TValue value, TimeSpan cacheInterval, int notificationTime = int.MinValue)
            : base(dateTimeProvider, value, notificationTime)
        {
            _cacheInterval = cacheInterval;
            _lastAccessDateTime = dateTimeProvider.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the SlidingExpirationCacheItem&lt;TValue&gt; class.
        /// </summary>
        /// <param name="value">Data for the cache item.</param>
        /// <param name="cacheInterval">Interval that will be used to determine if cache item has expired.</param>
        public SlidingExpirationCacheItem(TValue value, TimeSpan cacheInterval, int notificationTime = int.MinValue)
            : this(DateTimeProvider.Instance, value, cacheInterval, notificationTime) { }

        public override ICacheItem<TValue> CreateNewCacheItem(TValue value)
        {
            return new SlidingExpirationCacheItem<TValue>(value, _cacheInterval, _notificationTime)
                {
                    OnExpire = OnExpire,
                    OnAboutToExpire = OnAboutToExpire,
                    OnRemove = OnRemove
                };
        }

        public override TValue Value
        {
            get
            {
                _lastAccessDateTime = _dateTimeProvider.UtcNow;
                return _cacheItemValue;
            }
        }

        public override bool IsExpired
        {
            get { return _dateTimeProvider.UtcNow >= (_lastAccessDateTime + _cacheInterval); }
        }

        public override bool IsAboutToExpire
        {
            get { return _dateTimeProvider.UtcNow.AddSeconds(_notificationTime) >= (_lastAccessDateTime + _cacheInterval); }
        }
    }
}