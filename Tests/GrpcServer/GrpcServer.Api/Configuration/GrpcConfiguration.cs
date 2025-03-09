using GrpcServer.Api.Interceptors;
using GrpcServer.Api.Services;
using GrpcServer.Api.Services.TypeTestingServices;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.GrpcConfiguration", Version = "1.0")]

namespace GrpcServer.Api.Configuration
{
    public static class GrpcConfiguration
    {
        public static IServiceCollection ConfigureGrpc(this IServiceCollection services)
        {
            services.AddGrpc(
                options =>
                {
                    options.Interceptors.Add<GrpcExceptionInterceptor>();
                });
            services.AddGrpcReflection();
            return services;
        }

        public static IEndpointRouteBuilder MapGrpcServices(this WebApplication app)
        {
            app.MapGrpcService<ProductsService>();
            app.MapGrpcService<TagsService>();
            app.MapGrpcService<ForBinary>();
            app.MapGrpcService<ForBool>();
            app.MapGrpcService<ForByte>();
            app.MapGrpcService<ForChar>();
            app.MapGrpcService<ForDateOnly>();
            app.MapGrpcService<ForDateTime>();
            app.MapGrpcService<ForDateTimeOffset>();
            app.MapGrpcService<ForDecimal>();
            app.MapGrpcService<ForDictionary>();
            app.MapGrpcService<ForDouble>();
            app.MapGrpcService<ForEnum>();
            app.MapGrpcService<ForFloat>();
            app.MapGrpcService<ForGuid>();
            app.MapGrpcService<ForInt>();
            app.MapGrpcService<ForLong>();
            app.MapGrpcService<ForObject>();
            app.MapGrpcService<ForPagedResult>();
            app.MapGrpcService<ForShort>();
            app.MapGrpcService<ForString>();
            app.MapGrpcService<ForTimeSpan>();

            if (app.Environment.IsDevelopment())
            {
                app.MapGrpcReflectionService();
            }

            return app;
        }
    }
}