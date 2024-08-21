### Version 1.0.3

- Improvement: Updated module NuGet packages infrastructure.

### Version 1.0.2

- Improvement: Updated NuGet packages to latest stables.

### Version 1.0.1

- Improvement: Changed the Connection String name from `REDIS_CONNECTION_STRING` to `RedisOmConnectionString`.
- Fixed: `RedisConnectionProvider` now uses the `ConnectionMultiplexer` to connect to Redis instead of the connection string directly. This makes the connection string info more consistent with things like Health Checks.

> ⚠️ **NOTE**
> 
> This update will need for the connection string for Redis to no longer have the `redis://` scheme. It should look like this: `localhost:6379`.

### Version 1.0.0

- New Feature: Model document Entities to be persisted in a Redis Stack instance using the [Redis OM (Object Mapper)](https://redis.io/docs/connect/clients/om-clients/stack-dotnet/) library.
