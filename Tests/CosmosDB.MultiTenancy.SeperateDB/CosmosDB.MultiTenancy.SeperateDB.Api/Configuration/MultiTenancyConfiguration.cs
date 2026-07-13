using System;
using CosmosDB.MultiTenancy.SeperateDB.Infrastructure.MultiTenant;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.Stores.InMemoryStore;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyConfiguration", Version = "1.0")]

namespace CosmosDB.MultiTenancy.SeperateDB.Api.Configuration
{
    public static class MultiTenancyConfiguration
    {
        public static IServiceCollection ConfigureMultiTenancy(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMultiTenant<TenantExtendedInfo>()
                .WithInMemoryStore(SetupInMemoryStore) // See https://www.finbuckle.com/MultiTenant/Docs/v6.12.0/Stores#in-memory-store
                .WithRouteStrategy("__tenant__"); // example https://www.example.com/tenantidentifier/home/). See https://www.finbuckle.com/MultiTenant/Docs/v6.12.0/Strategies#route-strategy
            return services;
        }

        public static void UseMultiTenancy(this IApplicationBuilder app)
        {
            app.UseMultiTenant();
            InitializeStore(app.ApplicationServices);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        private static void SetupInMemoryStore(InMemoryStoreOptions<TenantExtendedInfo> options)
        {
            // configure in memory store:
            options.IsCaseSensitive = false;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public static void InitializeStore(IServiceProvider sp)
        {
            var scopeServices = sp.CreateScope().ServiceProvider;
            var store = scopeServices.GetRequiredService<IMultiTenantStore<TenantExtendedInfo>>();

            store.TryAddAsync(new TenantExtendedInfo() { Id = "sample-tenant-1", Identifier = "tenant1", Name = "Tenant 1", CosmosDbConnection = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;Database=DbTenant1;" }).Wait();
            store.TryAddAsync(new TenantExtendedInfo() { Id = "sample-tenant-2", Identifier = "tenant2", Name = "Tenant 2", CosmosDbConnection = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;Database=DbTenant2;" }).Wait();
        }
    }
}