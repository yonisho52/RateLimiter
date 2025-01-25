using RateLimiter.Model;
using RateLimiter.Services.Core;
using Startup.Services;

namespace RateLimiter.Tests
{
    public class ApiService1Tests
    {
        /// <summary>
        /// Tests the behavior of the <see cref="ApiService1"/> class to ensure:
        /// - All API actions are executed as expected (15 requests).
        /// - Rate limits are enforced correctly:
        ///   - No more than 5 requests are processed per second.
        ///   - No more than 10 requests are processed per minute.
        /// - Each request's data and timestamp are captured and validated.
        /// </summary>
        /// <remarks>
        /// This test simulates 15 API calls using the <see cref="ApiService1"/> service, which is backed by a 
        /// <see cref="RateLimiter{TArg, TResult}"/> configured with the following rate limits:
        /// - 5 requests per second.
        /// - 10 requests per minute.
        /// Each response is validated to ensure:
        /// - The correct number of actions were performed.
        /// - The responses are associated with the correct request data.
        /// - The timestamps adhere to the specified rate limits.
        /// </remarks>
        [Fact]
        public async Task ApiService1_PerformsAllActions_WithTimestamps()
        {
            // Arrange
            List<RateLimit> rateLimits = new List<RateLimit>
            {
                new RateLimit(5, TimeSpan.FromSeconds(1)),  // 5 requests per second
                new RateLimit(10, TimeSpan.FromMinutes(1)) // 10 requests per minute
            };

            List<(string Data, DateTime Timestamp)> actionsPerformed = new List<(string Data, DateTime Timestamp)>();
            var lockObject = new object(); // Lock for thread safety

            RateLimiter<string, Task> rateLimiter = new RateLimiter<string, Task>(
                rateLimits,
                async (data) =>
                {
                    lock (lockObject)
                    {
                        actionsPerformed.Add((data, DateTime.UtcNow)); // Capture data and timestamp
                    }
                    await Task.Delay(50); // Simulate work
                    return Task.CompletedTask;
                }
            );

            ApiService1 apiService1 = new ApiService1(rateLimiter);

            var tasks = Enumerable.Range(1, 15)
                .Select(i => apiService1.PerformAction($"Request {i}"))
                .ToList();

            await Task.WhenAll(tasks);

            // validate all 15 actions were performed
            Assert.Equal(15, actionsPerformed.Count);
            Assert.Contains(actionsPerformed, action => action.Data == "Request 1");
            Assert.Contains(actionsPerformed, action => action.Data == "Request 15");

            // validate timestamps
            List<DateTime> timestamps = actionsPerformed.Select(a => a.Timestamp).OrderBy(t => t).ToList();

            // group timestamps by seconds to validate rate limiting
            List<int> groupedBySecond = timestamps
                .GroupBy(t => t.ToString("HH:mm:ss"))
                .Select(g => g.Count())
                .ToList();

            // ensure no more than 5 actions occurred in any single second
            Assert.All(groupedBySecond, count => Assert.InRange(count, 0, 5));

            // ensure that the actions respect the overall rate limit of 10 actions per minute
            List<int> groupedByMinute = timestamps
                .GroupBy(t => t.ToString("HH:mm"))
                .Select(g => g.Count())
                .ToList();

            Assert.All(groupedByMinute, count => Assert.InRange(count, 0, 10));
        }
    }
}