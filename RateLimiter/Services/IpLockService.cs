using RateLimiter.Services.BaseRateLimitService;
using RateLimiter.Services.Core;


namespace RateLimiter.Services
{
    /// *****EXAMPLE FOR EXTENTIONS*****

    /// <summary>
    /// A base service for applying per-IP rate limits to actions.
    /// This service creates and manages individual <see cref="RateLimiter{TArg, TResult}"/> instances for each IP,
    /// ensuring that rate limits are applied independently for each IP.
    /// </summary>
    /// <typeparam name="TArg">The type of the argument passed to the action.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by the action.</typeparam>
    public abstract class IpLockService<TArg, TResult> : BaseRateLimitService<TArg, TResult>
    {
        private readonly Dictionary<string, RateLimiter<TArg, TResult>> _ipRateLimiters = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="IpLockService{TArg, TResult}"/> class.
        /// </summary>
        /// <param name="rateLimiter">A <see cref="RateLimiter{TArg, TResult}"/> instance containing the shared rate limits and action logic.</param>
        protected IpLockService(RateLimiter<TArg, TResult> rateLimiter) : base(rateLimiter)
        {
        }

        /// <summary>
        /// Retrieves or creates a <see cref="RateLimiter{TArg, TResult}"/> for the specified IP address.
        /// If a rate limiter does not already exist for the IP, a new one is created with the same rate limits and action logic as the base rate limiter.
        /// </summary>
        /// <param name="ip">The IP address to retrieve or create a rate limiter for.</param>
        /// <returns>A <see cref="RateLimiter{TArg, TResult}"/> instance specific to the IP address.</returns>
        protected RateLimiter<TArg, TResult> GetIpRateLimiter(string ip)
        {
            lock (_ipRateLimiters)
            {
                if (!_ipRateLimiters.ContainsKey(ip))
                {
                    _ipRateLimiters[ip] = new RateLimiter<TArg, TResult>(RateLimiter.RateLimits, RateLimiter.Action);
                }

                return _ipRateLimiters[ip];
            }
        }

        /// <summary>
        /// Executes the action while enforcing per-IP rate limits.
        /// </summary>
        /// <param name="argument">The argument to pass to the action, which contains IP-specific information.</param>
        /// <returns>The result of the asynchronous action.</returns>
        public override async Task<TResult> Perform(TArg argument)
        {
            string ip = ExtractIp(argument);
            var ipRateLimiter = GetIpRateLimiter(ip);
            return await ipRateLimiter.Perform(argument);
        }

        /// <summary>
        /// Extracts the IP address from the given argument.
        /// This method is intended to be overridden or replaced with logic to extract the IP address based on the application's requirements.
        /// </summary>
        /// <param name="argument">The argument containing IP-specific information.</param>
        /// <returns>The extracted IP address as a string.</returns>
        private string ExtractIp(TArg argument)
        {
            return "192.168.1.1";
        }
    }
}
