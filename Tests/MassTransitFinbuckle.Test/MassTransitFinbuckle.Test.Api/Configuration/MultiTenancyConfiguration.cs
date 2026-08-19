using System;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.Stores.InMemoryStore;
using Intent.RoslynWeaver.Attributes;
using MassTransitFinbuckle.Test.Infrastructure.Eventing;
using MassTransitFinbuckle.Test.Infrastructure.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyConfiguration", Version = "2.0")]

namespace MassTransitFinbuckle.Test.Api.Configuration
{
    public static class MultiTenancyConfiguration
    {
        public static IServiceCollection ConfigureMultiTenancy(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMultiTenant<TenantExtendedInfo>()
                .WithInMemoryStore(SetupInMemoryStore) // See https://www.finbuckle.com/MultiTenant/Docs/v6.12.0/Stores#in-memory-store
                .WithStrategy<FinbuckleMessageHeaderStrategy>(ServiceLifetime.Scoped)
                .WithHeaderStrategy("X-Tenant-Identifier"); // See https://www.finbuckle.com/MultiTenant/Docs/v6.12.0/Strategies#header-strategy
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

            store.TryAddAsync(new TenantExtendedInfo() { Id = "sample-tenant-1", Identifier = "tenant1", Name = "Tenant 1", ConnectionString = "Tenant1Connection" }).Wait();
            store.TryAddAsync(new TenantExtendedInfo() { Id = "sample-tenant-2", Identifier = "tenant2", Name = "Tenant 2", ConnectionString = "Tenant2Connection" }).Wait();
        }
    }
}
