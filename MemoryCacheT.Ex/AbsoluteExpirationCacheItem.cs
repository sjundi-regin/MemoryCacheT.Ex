using System;

namespace MemoryCacheT.Ex
{
    /// <summary>
    /// A cache item that expires after a certain duration or specified time in the future.
    /// </summary>
    /// <typeparam name="TValue">Type of value in cache item.</typeparam>
    public class AbsoluteExpirationCacheItem<TValue> : CacheItem<TValue>
    {
        private readonly DateTime _expirationDateTime;

        internal AbsoluteExpirationCacheItem(IDateTimeProvider dateTimeProvider, TValue value, DateTime expirationDateTime, int notificationTime = int.MinValue)
            : base(dateTimeProvider, value, notificationTime)
        {
            DateTime utcExpirationDateTime = expirationDateTime.ToUniversalTime();
            if (utcExpirationDateTime < _dateTimeProvider.UtcNow)
            {
                throw new ArgumentException("Expiration time must be greater than current time");
            }

            _expirationDateTime = utcExpirationDateTime;
        }

        internal AbsoluteExpirationCacheItem(IDateTimeProvider dateTimeProvider, TValue value, TimeSpan cacheInterval, int notificationTime = int.MinValue)
            : base(dateTimeProvider, value, notificationTime)
        {
            _expirationDateTime = _dateTimeProvider.UtcNow + cacheInterval;
        }

        /// <summary>
        /// Initializes a new instance of the AbsoluteExpirationCacheItem&lt;TValue&gt; class.
        /// </summary>
        /// <param name="value">Data for the cache item.</param>
        /// <param name="expirationDate">Expiration time for the cache item.</param>
        /// <exception cref="ArgumentException">expirationDate is a time in the past.</exception>
        public AbsoluteExpirationCacheItem(TValue value, DateTime expirationDate, int notificationTime = int.MinValue)
            : this(DateTimeProvider.Instance, value, expirationDate, notificationTime) { }

        /// <summary>
        /// Initializes a new instance of the AbsoluteExpirationCacheItem&lt;TValue&gt; class.
        /// </summary>
        /// <param name="value">Data for the cache item.</param>
        /// <param name="cacheInterval">Interval before the cache item will expire, beginning from now.</param>
        public AbsoluteExpirationCacheItem(TValue value, TimeSpan cacheInterval, int notificationTime = int.MinValue)
            : this(DateTimeProvider.Instance, value, cacheInterval, notificationTime) { }

        public override ICacheItem<TValue> CreateNewCacheItem(TValue value)
        {
            return new AbsoluteExpirationCacheItem<TValue>(_dateTimeProvider, value, _expirationDateTime, _notificationTime)
                {
                    OnExpire = OnExpire,
                    OnAboutToExpire = OnAboutToExpire,
                    OnRemove = OnRemove
                };
        }

        public override TValue Value
        {
            get { return _cacheItemValue; }
        }

        public override bool IsExpired
        {
            get { return _dateTimeProvider.UtcNow >= _expirationDateTime; }
        }

        public override bool IsAboutToExpire
        {
            get { return _dateTimeProvider.UtcNow >= _expirationDateTime; } // TODO: Implement time-logic
        }
    }
}