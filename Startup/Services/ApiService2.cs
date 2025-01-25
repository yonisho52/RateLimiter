using RateLimiter.Model;
using RateLimiter.Services;
using RateLimiter.Services.Core;


namespace Startup.Services
{
    /// <summary>
    /// A service that demonstrates the usage of the <see cref="RateLimiter{TArg, TResult}"/> to enforce rate limits
    /// for API calls that return a string response.
    /// </summary>
    public class ApiService2 : MethodLockService<string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiService2"/> class with pre-configured rate limits.
        /// </summary>
        /// <remarks>
        /// This service is configured with the following rate limits:
        /// - 5 requests per 1 second.
        /// - 20 requests per 1 minute.
        /// </remarks>
        public ApiService2() : base()
        {
            List<RateLimit> rateLimits = new List<RateLimit>()
            {
                new RateLimit(5, TimeSpan.FromSeconds(1)),
                new RateLimit(20, TimeSpan.FromMinutes(1))
            };

            this.RateLimiter = new RateLimiter<string, string>(rateLimits, CallApi);
        }

        /// <summary>
        /// Executes an API call while simulating a delay, returning a string response with the data and timestamp.
        /// </summary>
        /// <param name="data">The data or request payload to be processed by the API.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the asynchronous execution of the API call.
        /// The result is a string containing the data and the timestamp of the call.
        /// </returns>
        private async Task<string> CallApi(string data)
        {
            await Task.Delay(200); // Simulate API delay
            return $"CallApi response: {data} at {DateTime.UtcNow}";
        }

        /// <summary>
        /// Executes the rate-limited API call using the <see cref="RateLimiter{TArg, TResult}"/>.
        /// </summary>
        /// <param name="data">The data or request payload to pass to the API call.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous execution of the API call,
        /// with the result being the API response as a string.
        /// </returns>
        public Task<string> PerformCallApi(string data)
        {
            return Perform(data);
        }
    }
}
