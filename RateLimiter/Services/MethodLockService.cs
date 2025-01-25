using RateLimiter.Services.BaseRateLimitService;
using RateLimiter.Services.Core;

namespace RateLimiter.Services
{
    /// <summary>
    /// A base service class for applying rate limits to specific methods or operations.
    /// This service provides a simple mechanism for executing rate-limited operations.
    /// </summary>
    /// <typeparam name="TArg">The type of the argument passed to the operation.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by the operation.</typeparam>
    public abstract class MethodLockService<TArg, TResult> : BaseRateLimitService<TArg, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodLockService{TArg, TResult}"/> class
        /// with a specified <see cref="RateLimiter{TArg, TResult}"/>.
        /// </summary>
        /// <param name="rateLimiter">The <see cref="RateLimiter{TArg, TResult}"/> instance to enforce rate limits on the operation.</param>
        protected MethodLockService(RateLimiter<TArg, TResult> rateLimiter) : base(rateLimiter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodLockService{TArg, TResult}"/> class
        /// without a pre-configured rate limiter. The rate limiter must be initialized separately.
        /// </summary>
        protected MethodLockService() : base()
        {
        }

        /// <summary>
        /// Executes a rate-limited operation with the specified argument.
        /// </summary>
        /// <param name="argument">The argument to pass to the operation.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous execution of the operation,
        /// with the result being the output of the rate-limited operation.
        /// </returns>
        public override async Task<TResult> Perform(TArg argument)
        {
            return await RateLimiter.Perform(argument);
        }
    }
}
