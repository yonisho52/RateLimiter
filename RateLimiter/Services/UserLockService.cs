using RateLimiter.Services.BaseRateLimitService;
using RateLimiter.Services.Core;

namespace RateLimiter.Services
{
    /// *****EXAMPLE FOR EXTENTIONS*****

    /// <summary>
    /// A base service for applying per-user rate limits to actions.
    /// This service creates and manages individual <see cref="RateLimiter{TArg, TResult}"/> instances for each user,
    /// ensuring that rate limits are applied independently for each user.
    /// </summary>
    /// <typeparam name="TArg">The type of the argument passed to the action.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by the action.</typeparam>
    public abstract class UserLockService<TArg, TResult> : BaseRateLimitService<TArg, TResult>
    {
        private readonly Dictionary<string, RateLimiter<TArg, TResult>> _userRateLimiters = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="UserLockService{TArg, TResult}"/> class.
        /// </summary>
        /// <param name="rateLimiter">A <see cref="RateLimiter{TArg, TResult}"/> instance containing the shared rate limits and action logic.</param>
        protected UserLockService(RateLimiter<TArg, TResult> rateLimiter) : base(rateLimiter)
        {
        }

        /// <summary>
        /// Retrieves or creates a <see cref="RateLimiter{TArg, TResult}"/> for the specified user.
        /// If a rate limiter does not already exist for the user, a new one is created with the same rate limits and action logic as the base rate limiter.
        /// </summary>
        /// <param name="userId">The unique identifier for the user.</param>
        /// <returns>A <see cref="RateLimiter{TArg, TResult}"/> instance specific to the user.</returns>
        protected RateLimiter<TArg, TResult> GetUserRateLimiter(string userId)
        {
            lock (_userRateLimiters)
            {
                if (!_userRateLimiters.ContainsKey(userId))
                {
                    _userRateLimiters[userId] = new RateLimiter<TArg, TResult>(RateLimiter.RateLimits, RateLimiter.Action);
                }

                return _userRateLimiters[userId];
            }
        }

        /// <summary>
        /// Executes the action while enforcing per-user rate limits.
        /// </summary>
        /// <param name="argument">The argument to pass to the action, which contains user-specific information.</param>
        /// <returns>The result of the asynchronous action.</returns>
        public override async Task<TResult> Perform(TArg argument)
        {
            string userId = ExtractUserId(argument); // Logic to extract user ID
            var userRateLimiter = GetUserRateLimiter(userId);
            return await userRateLimiter.Perform(argument);
        }

        /// <summary>
        /// Extracts the user ID from the given argument.
        /// This method is intended to be overridden or replaced with logic to extract the user ID based on the application's requirements.
        /// </summary>
        /// <param name="argument">The argument containing user-specific information.</param>
        /// <returns>The extracted user ID as a string.</returns>
        /// <exception cref="ArgumentException">Thrown if the user ID cannot be extracted from the argument.</exception>
        private string ExtractUserId(TArg argument)
        {
            // Replace this with actual user ID extraction logic
            return "user123";
        }
    }
}
