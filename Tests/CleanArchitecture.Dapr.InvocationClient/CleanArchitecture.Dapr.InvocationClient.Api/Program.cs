using CleanArchitecture.Dapr.InvocationClient.Api.Configuration;
using CleanArchitecture.Dapr.InvocationClient.Api.Filters;
using CleanArchitecture.Dapr.InvocationClient.Application;
using CleanArchitecture.Dapr.InvocationClient.Infrastructure;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Program", Version = "1.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDaprSidekick(builder.Configuration);

            builder.Services.AddControllers(
                opt =>
                {
                    opt.Filters.Add<ExceptionFilter>();
                })
            .AddDapr();
            builder.Services.AddApplication(builder.Configuration);
            builder.Services.ConfigureApplicationSecurity(builder.Configuration);
            builder.Services.ConfigureProblemDetails();
            builder.Services.ConfigureApiVersioning();
            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.ConfigureSwagger(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseExceptionHandler();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.UseSwashbuckle(builder.Configuration);

            app.Run();
        }
    }
}