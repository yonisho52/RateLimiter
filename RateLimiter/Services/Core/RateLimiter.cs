using RateLimiter.Model;

namespace RateLimiter.Services.Core
{
    /// <summary>
    /// A dynamic rate limiter for enforcing multiple rate limits on a single asynchronous action.
    /// </summary>
    /// <typeparam name="TArg">The type of the argument passed to the action.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by the action.</typeparam>
    public class RateLimiter<TArg, TResult>
    {
        private readonly List<RateLimit> _rateLimits;
        public List<RateLimit> RateLimits { get { return _rateLimits; } }

        private readonly Func<TArg, Task<TResult>> _action;
        public Func<TArg, Task<TResult>> Action { get { return _action; } }

        private readonly Dictionary<RateLimit, Queue<DateTime>> _rateLimitQueues;
        private readonly object _lock = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimiter{TArg, TResult}"/> class.
        /// </summary>
        /// <param name="rateLimits">A list of rate limits to enforce. Each rate limit specifies the maximum number of actions allowed within a given time window.</param>
        /// <param name="action">The asynchronous action to be rate-limited.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
        public RateLimiter(List<RateLimit> rateLimits, Func<TArg, Task<TResult>> action)
        {
            _rateLimits = rateLimits;
            _action = action;
            _rateLimitQueues = _rateLimits.ToDictionary(ratelimit => ratelimit, newQueue => new Queue<DateTime>());
        }

        /// <summary>
        /// Executes the registered action while ensuring all rate limits are enforced.
        /// </summary>
        /// <param name="argument">The argument to pass to the action.</param>
        /// <returns>The result of the asynchronous action.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no rate limits are configured.</exception>
        public async Task<TResult> Perform(TArg argument)
        {
            if (_rateLimits == null || !_rateLimits.Any())
            {
                throw new InvalidOperationException("Rate limits must be provided and cannot be empty.");
            }

            // enforce rate limits sequentially
            foreach (var rateLimit in _rateLimits)
            {
                await EnforceRateLimit(rateLimit);
            }

            return await _action(argument);
        }

        /// <summary>
        /// Enforces the specified rate limit by delaying execution if necessary.
        /// </summary>
        /// <param name="rateLimit">The rate limit to enforce.</param>
        private async Task EnforceRateLimit(RateLimit rateLimit)
        {
            DateTime now = DateTime.UtcNow;

            while (true)
            {
                TimeSpan waitTime = TimeSpan.Zero;

                lock (_lock)
                {
                    Queue<DateTime> currentQueue = _rateLimitQueues[rateLimit];

                    // remove expired timestamps
                    while (currentQueue.Count > 0 && now - currentQueue.Peek() > rateLimit.TimeWindow)
                    {
                        currentQueue.Dequeue();
                    }

                    // check if the rate limit is exceeded
                    if (currentQueue.Count >= rateLimit.MaxCount)
                    {
                        waitTime = rateLimit.TimeWindow - (now - currentQueue.Peek());
                    }
                    else
                    {
                        currentQueue.Enqueue(now);
                        break; // exit the loop when no wait is needed
                    }
                }

                // perform async delay outside the lock
                if (waitTime > TimeSpan.Zero)
                {
                    await Task.Delay(waitTime);
                    now = DateTime.UtcNow; // update current time for the next iteration
                }
            }
        }
    }
}