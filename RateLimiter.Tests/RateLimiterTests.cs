using RateLimiter.Model;
using RateLimiter.Services.Core;

namespace RateLimiter.Tests
{
    public class RateLimiterTests
    {
        /// <summary>
        /// Tests the behavior of the <see cref="RateLimiter{TArg, TResult}"/> class to ensure:
        /// - Rate limits are correctly enforced for a single rate limit configuration.
        /// - Requests are throttled to adhere to the specified limit of 3 requests per second.
        /// - All requests are processed, and their execution timestamps are captured and validated.
        /// </summary>
        /// <remarks>
        /// This test configures the <see cref="RateLimiter{TArg, TResult}"/> with a rate limit of 3 requests per second.
        /// It simulates 6 API calls and verifies:
        /// - That all 6 requests are executed successfully.
        /// - That the requests are grouped and processed within the configured limit of 3 requests per second.
        /// - That the timestamps for each request comply with the rate-limiting rules.
        /// </remarks>
        /// <returns></returns>
        [Fact]
        public async Task RateLimiter_EnforcesRateLimits()
        {
            List<RateLimit> rateLimits = new List<RateLimit>
            {
                new RateLimit(3, TimeSpan.FromSeconds(1)) // 3 requests per second
            };

            List<DateTime> timestamps = new List<DateTime>();
            RateLimiter<string, Task> rateLimiter = new RateLimiter<string, Task>(
                rateLimits,
                async (data) =>
                {
                    timestamps.Add(DateTime.UtcNow);
                    await Task.Delay(50); 
                    return Task.CompletedTask;
                }
            );

            var tasks = Enumerable.Range(1, 6)
                .Select(i => rateLimiter.Perform($"Request {i}"))
                .ToList();

            await Task.WhenAll(tasks);

            Assert.Equal(6, timestamps.Count);

            // verify timestamps to ensure rate limits were enforced
            List<int> groupedBySecond = timestamps
                .GroupBy(t => t.Second)
                .Select(g => g.Count()).ToList();

            Assert.All(groupedBySecond, count => Assert.InRange(count, 0, 3));
        }
    }
}