# AdCreative.ai Integration Case

## Description of the Problem

Resources

https://medium.com/@uderbentoglu/parallel-programming-with-semaphoreslim-in-net-407b6a6ee673

https://medium.com/@deniztasdogen/locking-async-methods-in-c-fd840eb2322e

https://cheoalfredo.medium.com/distributed-locks-with-redis-net-2b1b50355deb

## Required Solution

### 1- Single Server Scenario

**a: Solution Description:**
ConcurrentDictionary with SemaphoreSlim: The locks dictionary is a ConcurrentDictionary where each item content (string) is associated with a SemaphoreSlim. This dictionary ensures thread-safe access to the semaphores.

SemaphoreSlim: Each semaphore in the dictionary allows only one thread to enter its critical section at a time (new SemaphoreSlim(1, 1)). This ensures that threads processing items with the same content are executed sequentially.

**b: Implementation:**
- Implemented soluiton in Service Layer

**c: Demonstration in Program.cs:**
- Implemented Demonstration in Program.cs

### 2 - Distributed System Scenario

**a: Solution Description:**
- Solution in DistributedItemIntegrationService.cs and demonstration in Program.cs

**b: Weaknesses:**
- Dependency on Redis and single point of failure.
- If the system crashes while holding locks, it could lead to data inconsistencies or loss
- In scenarios where multiple services contend for the same locks, there is a risk of deadlocks if proper precautions are not taken