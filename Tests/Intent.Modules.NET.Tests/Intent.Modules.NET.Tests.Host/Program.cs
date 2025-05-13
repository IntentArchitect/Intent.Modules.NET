using Intent.Modules.NET.Tests.Host.Configuration;
using Intent.Modules.NET.Tests.Host.Eventing;
using Intent.Modules.NET.Tests.Host.Filters;
using Intent.Modules.NET.Tests.Infrastructure.Core.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Program", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers(
                opt =>
                {
                    opt.Filters.Add<ExceptionFilter>();
                });
            var moduleInstallers = ModuleInstallerFactory.GetModuleInstallers();
            builder.Services.AddApplication(builder.Configuration);
            moduleInstallers.ConfigureContainer(builder.Services, builder.Configuration);
            builder.Services.ConfigureApplicationSecurity(builder.Configuration);
            builder.Services.ConfigureProblemDetails();
            builder.Services.ConfigureApiVersioning();
            builder.Services.AddMassTransitConfiguration(builder.Configuration, moduleInstallers);
            builder.Services.ConfigureSwagger(builder.Configuration, moduleInstallers);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.UseSwashbuckle(builder.Configuration);

            app.Run();
        }
    }
}