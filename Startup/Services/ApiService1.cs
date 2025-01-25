using RateLimiter.Services;
using RateLimiter.Services.Core;


namespace Startup.Services
{
    /// <summary>
    /// A service that demonstrates the usage of the <see cref="RateLimiter{TArg, TResult}"/> to enforce rate limits
    /// for asynchronous operations that do not return a value.
    /// </summary>
    public class ApiService1 : MethodLockService<string, Task>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiService1"/> class.
        /// </summary>
        /// <param name="rateLimiter">
        /// An instance of <see cref="RateLimiter{TArg, TResult}"/> configured with the desired rate limits
        /// to throttle API calls.
        /// </param>
        public ApiService1(RateLimiter<string, Task> rateLimiter) : base(rateLimiter) { }

        /// <summary>
        /// Executes an API call while adhering to the rate limits defined in the provided <see cref="RateLimiter{TArg, TResult}"/>.
        /// </summary>
        /// <param name="data">The data or request payload to pass to the API call.</param>
        public Task PerformAction(string data)
        {
            return Perform(data);
        }
    }
}
