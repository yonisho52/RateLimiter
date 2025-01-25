using RateLimiter.Services.Core;
using Startup.Services;

namespace RateLimiter.Tests
{
    public class ApiService2Tests
    {
        /// <summary>
        /// Tests the behavior of the <see cref="ApiService2"/> class to ensure:
        /// - All API actions are executed as expected (10 requests).
        /// - Responses are returned in the expected format, including the request data and timestamp.
        /// - Rate limits are enforced correctly:
        ///   - No more than 5 requests are processed in a single second.
        /// - All timestamps are recorded and validated against the configured rate limits.
        /// </summary>
        /// <remarks>
        /// This test simulates 10 API calls using the <see cref="ApiService2"/> service, which is backed by a
        /// <see cref="RateLimiter{TArg, TResult}"/> with the following rate limits:
        /// - 5 requests per second.
        /// Each response is validated for its format and timestamp correctness. The timestamps are further grouped
        /// and analyzed to ensure adherence to the rate limits.
        /// </remarks>
        [Fact]
        public async Task ApiService2_PerformsApiCallsCorrectly_WithTimestamps()
        {
            ApiService2 apiService2 = new ApiService2();
            var timestamps = new List<DateTime>();

            // simulate 10 API calls
            var tasks = Enumerable.Range(0, 10)
                .Select(async i =>
                {
                    string response = await apiService2.PerformCallApi($"Request {i}");

                    // Extract and store the timestamp from the response
                    string timestampString = response.Split(" at ")[1];
                    if (DateTime.TryParse(timestampString, out var timestamp))
                    {
                        lock (timestamps)
                        {
                            timestamps.Add(timestamp);
                        }
                    }
                    return response;
                })
                .ToList();

            await Task.WhenAll(tasks);

            // validate timestamps
            Assert.Equal(10, timestamps.Count); // Ensure all timestamps are recorded

            // group timestamps by seconds to validate rate limiting (5 requests/second)
            var groupedBySecond = timestamps
                .GroupBy(t => t.ToString("HH:mm:ss"))
                .Select(g => g.Count())
                .ToList();

            // verify that no more than 5 requests occurred in any single second
            Assert.All(groupedBySecond, count => Assert.InRange(count, 0, 5));
        }

    }
}
