using GrpcServer.Api.Services;

namespace GrpcServer.Api.Configuration
{
    public static class GrpcConfiguration
    {
        public static IServiceCollection ConfigureGrpc(this IServiceCollection services)
        {
            services.AddGrpc();
            services.AddGrpcReflection();

            return services;
        }

        public static IEndpointRouteBuilder MapGrpcServices(this WebApplication app)
        {
            app.MapGrpcService<ProductsGrpcService>();

            if (app.Environment.IsDevelopment())
            {
                app.MapGrpcReflectionService();
            }

            return app;
        }
    }
}
