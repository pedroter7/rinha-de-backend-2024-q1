# Rinha de Backend 2024 - Q1

This is my solution to [zanfranceschi/rinha-de-backend-2024-q1](https://github.com/zanfranceschi/rinha-de-backend-2024-q1). It leverages MariaDb concurrency control mechanisms by relying on a pessimistic concurrency control strategy that uses row-based locking during write procedure execution. All the logic and concurrency control is abstracted by database procedures, thus one can easily switch the web API code/technology. The solution feats a ASP.NET 8 WebAPI using C#, [HAProxy](https://www.haproxy.org/) as loadbalancer, and [MariaDb](https://mariadb.org/) 11 as database.

## The architecture

```txt
                +----------+
                |          |
                |  Client  |
                |          |
                +----------+
                     |
                     | HTTP req
                     V
                +-----------+
                |           |
                |  HAProxy  |
                |           |
                +-----------+
                     |
         +-----------+-------------+
         |                         |
         V                         V
+------------------+      +------------------+
|                  |      |                  |
|  ASP.NET WebAPI  |      |  ASP.NET WebAPI  |
|                  |      |                  |
+------------------+      +------------------+
          |                        |
          +-----------+------------+
                      |
                      V
                +-----------+
                |           |
                |  MariaDB  |
                |           |
                +-----------+
```

## Testing, CI and running

Database procedures are tested using a .NET XUnit test project that leverages [TestContainers](https://testcontainers.com/) framework to spin-up a clean version of my MariaDb testing Docker image for each test. The WebAPI's core service is unit tested using XUnit as well.

A simple yet powerful continous integration pipeline was built and runs on GitHub Actions. Tests are run on pull requests to the master branch and the PR can only be completed when all tests pass. When new code reaches the main branch, GitHub actions build and publishes the necessary [Docker image tags to DockerHub](https://hub.docker.com/r/pedroter7/rinha-de-backend-2024-q1).

The whole solution can be run by using the Docker Compose file in the repo root.

## Conclusions after load tests

While reliable and fast, the concurrency control strategy employed in this solution tends to perform poorly when concurrency to write to a single resource is high. Given that, this pessimistic approach can be more suitable to applications where the probability of concurrency for a single resource is small, high latency for write requests is torelable, or the write rate is lower than the read rate. The good thing is that this strategy is easier to implement and verify than an optimistic one.
