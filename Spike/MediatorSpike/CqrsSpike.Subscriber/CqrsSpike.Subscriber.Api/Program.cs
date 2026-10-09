using CqrsSpike.Subscriber.Api.Configuration;
using CqrsSpike.Subscriber.Api.FastEndpoints;
using CqrsSpike.Subscriber.Application;
using CqrsSpike.Subscriber.Infrastructure;
using FastEndpoints;

namespace CqrsSpike.Subscriber.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddApplication(builder.Configuration);
            builder.Services.ConfigureApplicationSecurity(builder.Configuration);
            builder.Services.ConfigureProblemDetails();
            builder.Services.ConfigureApiVersioning();
            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.AddFastEndpoints(opt =>
            {
                opt.Assemblies = [typeof(Program).Assembly];
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapFastEndpoints(c => c.Endpoints.Configurator = ep =>
            {
                ep.PostProcessors(0, new ExceptionProcessor());
            });

            app.Run();
        }
    }
}