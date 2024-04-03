namespace Intent.Modules.UnitOfWork.Persistence.Shared;

internal static class TemplateIds
{
    public const string DaprStateStoreUnitOfWorkInterface = "Intent.Dapr.AspNetCore.StateManagement.DaprStateStoreUnitOfWorkInterface";
    public const string CosmosDBUnitOfWorkInterface = "Intent.CosmosDB.CosmosDBUnitOfWorkInterface";
    public const string MongoDbUnitOfWorkInterface = "Intent.MongoDb.MongoDbUnitOfWorkInterface";
    public const string TableStorageUnitOfWorkInterface = "Intent.Azure.TableStorage.TableStorageUnitOfWorkInterface";
    public const string RedisOmUnitOfWorkInterface = "Intent.Redis.Om.Repositories.Templates.RedisOmUnitOfWorkInterface";
    public const string DistributedCacheWithUnitOfWorkInterface = "Intent.AspNetCore.DistributedCaching.DistributedCacheWithUnitOfWorkInterface";
}