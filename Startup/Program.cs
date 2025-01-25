using RateLimiter.Model;
using RateLimiter.Services.Core;
using Startup.Services;
using System.Reflection.Metadata;

namespace Startup
{
    /// <summary>
    /// Demonstrates the usage of the RateLimiter with two different API service implementations:
    /// - <see cref="ApiService1"/>: Handles non-returning tasks.
    /// - <see cref="ApiService2"/>: Handles tasks that return a string response.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The entry point of the application.
        /// Executes one of the two available options to demonstrate rate-limited API operations.
        /// </summary>
        /// <param name="args">Command-line arguments (not used).</param>
        async static Task Main(string[] args)
        {
            // Option1 demonstrates using ApiService1 with non-returning tasks.
            await Option1();

            // Option2 demonstrates using ApiService2 with string-returning API calls.
            await Option2();
        }

        /// <summary>
        /// Demonstrates the usage of <see cref="ApiService1"/> with non-returning tasks.
        /// </summary>
        /// <remarks>
        /// - Configures the <see cref="RateLimiter{TArg, TResult}"/> with the following rate limits:
        ///   - 10 requests per 10 seconds.
        ///   - 20 requests per minute.
        /// - Simulates 25 API calls, printing a log for each executed action.
        /// </remarks>
        private async static Task Option1()
        {
            // Step 1: Define rate limits
            var rateLimits = new List<RateLimit>
            {
                new RateLimit(10, TimeSpan.FromSeconds(10)),  // 5 requests per second
                new RateLimit(20, TimeSpan.FromMinutes(1)) // 10 requests per minute
            };

            // Step 2: Create a RateLimiter for non-returning tasks
            var rateLimiter = new RateLimiter<string, Task>(
                rateLimits,
                async (data) =>
                {
                    // Simulate an API call or action
                    await Task.Delay(200); // Simulate work
                    Console.WriteLine($"Performed action with: {data} at {DateTime.UtcNow}");
                    return Task.CompletedTask;
                }
            );

            // Step 3: Create the ApiService1 instance
            var apiService1 = new ApiService1(rateLimiter);

            // Step 3: Perform API calls
            var tasks = Enumerable.Range(1, 25)
                .Select(i => apiService1.PerformAction($"Request {i}"))
            .ToList();

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Demonstrates the usage of <see cref="ApiService2"/> with string-returning API calls.
        /// </summary>
        /// <remarks>
        /// - Configures <see cref="ApiService2"/> with the following rate limits:
        ///   - 5 requests per second.
        ///   - 20 requests per minute.
        /// - Simulates 10 API calls, printing the response for each executed action.
        /// </remarks>
        private async static Task Option2()
        {
            ApiService2 apiService = new ApiService2();

            // Simulate multiple API calls (distinct requests)
            var tasks = Enumerable.Range(0, 10)
                .Select(async i =>
                {
                    var result = await apiService.PerformCallApi($"Request {i}");
                    Console.WriteLine(result);
                    return result;
                })
                .ToList(); // Ensure tasks are materialized properly

            await Task.WhenAll(tasks);

        }
    }
}