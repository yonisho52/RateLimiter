namespace RateLimiter.Services.BaseRateLimitService
{
    /// <summary>
    /// Represents the base interface for services that enforce rate limits on operations.
    /// </summary>
    /// <typeparam name="TArg">The type of the argument passed to the rate-limited operation.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by the rate-limited operation.</typeparam>
    public interface IBaseRateLimitService<TArg, TResult>
    {
        /// <summary>
        /// Executes a rate-limited operation with the specified argument.
        /// </summary>
        /// <param name="argument">The argument to pass to the operation.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous execution of the operation.
        /// The result contains the output of the rate-limited operation.
        /// </returns>
        Task<TResult> Perform(TArg argument);
    }
}
