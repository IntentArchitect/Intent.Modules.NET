### Version 1.0.11

- Improvement: Updated NuGet package versions.

### Version 1.0.10

- Improvement: Updated NuGet package versions.
- Fixed: Issue with unit of work not being correctly injected into the constructor

### Version 1.0.9

- Improvement: Updated NuGet package versions.
- Improvement: Added stereotype descriptions in preperation for Intent Architect 4.5. 

### Version 1.0.8

- Improvement: Updated NuGet package versions.

### Version 1.0.7

- Improvement: Updated NuGet package versions.

### Version 1.0.6

- Improvement: Updated NuGet package versions.

### Version 1.0.5

- Improvement: Updated NuGet package versions.

### Version 1.0.4

- Improvement: Small updated to module internal code

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
