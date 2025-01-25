RateLimiter

I chose to implement the rate limiter using the Sliding Window approach. Below are the pros and cons of this approach:

Pros:

* Fairer Request Handling:
  Requests are spread out more evenly, ensuring users don’t hit hard limits due to fixed time boundaries.

* Smooth Traffic Control:
  Prevents sudden bursts of traffic that could overload the system, making it more stable.

* Real-Time Adaptation:
  Dynamically adjusts based on recent activity, allowing for a more user-friendly and responsive system.
  
* Better for High-Concurrency Scenarios:
  When multiple users or systems make concurrent requests, Sliding Window ensures requests are distributed evenly, reducing contention and improving fairness (assumpting that the rate limmiter used for users).

Cons:

* Higher Complexity:
  Sliding Window requires tracking request timestamps, which increases memory usage and makes the implementation more complex compared to simpler approaches like Absolute Window.

* More Resource:
   Since the Sliding Window approach keeps track of individual request timestamps within a defined window, it can require additional processing power and memory. 
   This may become significant in high-traffic systems with many concurrent requests.

* Distributed Systems:
  Implementing Sliding Window in a distributed system (e.g., multiple instances of the same microservice) can be challenging. 
  Synchronizing request data accurately across nodes to maintain consistent rate limiting requires careful design, especially when using shared storage (like Redis or a database).

Solution stracture:
1. RateLimiter - Class Library - Core logic and services for implementing rate limiting (e.g., Sliding Window approach).
Key Components: RateLimiter.cs (logic), MethodLockService-based rate limiting.
2. RateLimiter.Tests - xUnit Test - Contains unit tests to verify the correctness of the RateLimiter logic and services.
- Tests for Sliding Window behavior.
- Service-specific functionality (e.g., ApiService1 and ApiService2).
3. Startup - Console App - Demonstrates how to use the RateLimiter in real-world scenarios.
- ApiService1 and ApiService2 showcasing rate-limited method execution.
- Program.cs with example implementations (Option1 and Option2).

I have added an extension to the example, allowing the RateLimiter to work with both UserID and IP.
This enhancement is based on the UserLockService and IpLockService implementations provided earlier.
 
Reference:
 https://www.youtube.com/watch?v=VzW41m4USGs
