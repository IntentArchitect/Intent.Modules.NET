using System;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Stores;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyConfiguration", Version = "1.0")]

namespace CosmosDBMultiTenancy.Api.Configuration
{
    public static class MultiTenancyConfiguration
    {
        public static IServiceCollection ConfigureMultiTenancy(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMultiTenant<TenantInfo>()
                .WithHttpRemoteStore(configuration["Finbuckle:MultiTenant:Stores:HttpRemoteEndpointTemplate"]!) // See https://www.finbuckle.com/MultiTenant/Docs/v6.12.0/Stores#http-remote-store
                .WithHeaderStrategy("X-Tenant-Identifier"); // See https://www.finbuckle.com/MultiTenant/Docs/v6.12.0/Strategies#header-strategy
            return services;
        }

        public static void UseMultiTenancy(this IApplicationBuilder app)
        {
            app.UseMultiTenant();
            InitializeStore(app.ApplicationServices);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public static void InitializeStore(IServiceProvider sp)
        {
            var scopeServices = sp.CreateScope().ServiceProvider;
            var store = scopeServices.GetRequiredService<IMultiTenantStore<TenantInfo>>();

            store.TryAddAsync(new TenantInfo() { Id = "sample-tenant-1", Identifier = "tenant1", Name = "Tenant 1", ConnectionString = "Tenant1Connection" }).Wait();
            store.TryAddAsync(new TenantInfo() { Id = "sample-tenant-2", Identifier = "tenant2", Name = "Tenant 2", ConnectionString = "Tenant2Connection" }).Wait();
        }
    }
}