using RateLimiter.Services.Core;

namespace RateLimiter.Services.BaseRateLimitService
{
    /// <summary>
    /// A base class for implementing services that utilize the <see cref="RateLimiter{TArg, TResult}"/> 
    /// to enforce rate limits on operations.
    /// </summary>
    /// <typeparam name="TArg">The type of the argument passed to the action being rate-limited.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by the action being rate-limited.</typeparam>
    public abstract class BaseRateLimitService<TArg, TResult> : IBaseRateLimitService<TArg, TResult>
    {
        /// <summary>
        /// The <see cref="RateLimiter{TArg, TResult}"/> instance used to enforce rate limits.
        /// </summary>
        protected RateLimiter<TArg, TResult> RateLimiter;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRateLimitService{TArg, TResult}"/> class
        /// with a specified <see cref="RateLimiter{TArg, TResult}"/>.
        /// </summary>
        /// <param name="rateLimiter">The rate limiter used to enforce rate limits on operations.</param>
        protected BaseRateLimitService(RateLimiter<TArg, TResult> rateLimiter)
        {
            RateLimiter = rateLimiter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRateLimitService{TArg, TResult}"/> class without
        /// a pre-configured rate limiter. The rate limiter must be initialized separately.
        /// </summary>
        protected BaseRateLimitService()
        {
        }

        /// <summary>
        /// Executes the rate-limited operation with the specified argument.
        /// This method must be implemented by derived classes to define the behavior of the operation.
        /// </summary>
        /// <param name="argument">The argument to pass to the operation.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous execution of the operation.</returns>
        public abstract Task<TResult> Perform(TArg argument);
    }
}
