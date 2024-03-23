# Intent.Modules.EntityFrameworkCore.SecondLevelCaching

This module adds second level caching to Entity Framework Core through use of the [EFCoreSecondLevelCacheInterceptor](https://github.com/VahidN/EFCoreSecondLevelCacheInterceptor#readme) library, refer to EFCoreSecondLevelCacheInterceptor's README for full information.

A [custom cache provider](https://github.com/VahidN/EFCoreSecondLevelCacheInterceptor?tab=readme-ov-file#using-a-custom-cache-provider) is generated using `IDistributedCache` as the underlying cache provider.
