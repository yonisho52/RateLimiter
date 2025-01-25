using System;

namespace RateLimiter.Model
{
    /// <summary>
    /// Represents a rate limit configuration that specifies the maximum number of allowed actions 
    /// within a given time window.
    /// </summary>
    public class RateLimit
    {
        /// <summary>
        /// Gets the maximum number of actions allowed within the time window.
        /// </summary>
        public int MaxCount { get; }

        /// <summary>
        /// Gets the time window during which the rate limit applies.
        /// </summary>
        public TimeSpan TimeWindow { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimit"/> class.
        /// </summary>
        /// <param name="maxCount">The maximum number of actions allowed within the time window.</param>
        /// <param name="timeWindow">The duration of the time window for the rate limit.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="maxCount"/> is less than or equal to zero,
        /// or if <paramref name="timeWindow"/> is less than or equal to zero.
        /// </exception>
        public RateLimit(int maxCount, TimeSpan timeWindow)
        {
            if (maxCount <= 0) throw new ArgumentOutOfRangeException(nameof(maxCount), "MaxCount must be greater than zero.");
            if (timeWindow <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(timeWindow), "TimeWindow must be greater than zero.");

            MaxCount = maxCount;
            TimeWindow = timeWindow;
        }
    }
}
