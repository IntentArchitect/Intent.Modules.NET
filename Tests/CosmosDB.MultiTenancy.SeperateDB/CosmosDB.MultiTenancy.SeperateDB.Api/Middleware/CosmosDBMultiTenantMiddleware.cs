using System;
using System.Threading.Tasks;
using CosmosDB.MultiTenancy.SeperateDB.Infrastructure.Persistence;
using Finbuckle.MultiTenant;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBMultiTenantMiddleware", Version = "1.0")]

namespace CosmosDB.MultiTenancy.SeperateDB.Api.Middleware
{
    public class CosmosDBMultiTenantMiddleware
    {
        private readonly CosmosDBMultiTenantClientProvider _clientProvider;
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public CosmosDBMultiTenantMiddleware(RequestDelegate next,
            IServiceProvider serviceProvider,
            ICosmosClientProvider clientProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
            _clientProvider = (CosmosDBMultiTenantClientProvider)clientProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var tenant = scope.ServiceProvider.GetService<TenantInfo>();
                var cosmosClientOptionsProvider = scope.ServiceProvider.GetRequiredService<ICosmosClientOptionsProvider>();

                using (_clientProvider.SetLocalState(tenant, cosmosClientOptionsProvider))
                {
                    await _next(context);
                }

            }
        }
    }

    public static class CosmosDBMultiTenantMiddlewareExtensions
    {
        public static IApplicationBuilder UseCosmosMultiTenantMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CosmosDBMultiTenantMiddleware>();
        }
    }
}