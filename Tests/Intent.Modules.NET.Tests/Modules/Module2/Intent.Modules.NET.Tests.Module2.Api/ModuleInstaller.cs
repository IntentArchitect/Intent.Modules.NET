using Intent.Modules.NET.Tests.Infrastructure.Core.Interfaces;
using Intent.Modules.NET.Tests.Module2.Infrastructure.Configuration;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModularMonolith.Module.ModuleInstallerTemplate", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module2.Api
{
    public class ModuleInstaller : IModuleInstaller
    {
        public void ConfigureContainer(IServiceCollection services, IConfiguration configuration)
        {
            Application.DependencyInjection.AddApplication(services, configuration);
            Infrastructure.DependencyInjection.AddInfrastructure(services, configuration);
            var builder = services.AddControllers();
            builder.PartManager.ApplicationParts.Add(new AssemblyPart(typeof(ModuleInstaller).Assembly));
        }

        public void ConfigureSwagger(SwaggerGenOptions options)
        {
            AddCommentFile(options, Path.Combine(AppContext.BaseDirectory, $"{typeof(Intent.Modules.NET.Tests.Module2.Api.ModuleInstaller).Assembly.GetName().Name}.xml"));
            AddCommentFile(options, Path.Combine(AppContext.BaseDirectory, $"{typeof(Intent.Modules.NET.Tests.Module2.Application.DependencyInjection).Assembly.GetName().Name}.Contracts.xml"));
        }

        public void ConfigureIntegrationEventConsumers(IRegistrationConfigurator cfg)
        {
            MassTransitConfiguration.AddConsumers(cfg);
        }

        private static void AddCommentFile(SwaggerGenOptions options, string filename)
        {
            string? docFile = Path.Combine(AppContext.BaseDirectory, filename);

            if (File.Exists(docFile))
            {
                options.IncludeXmlComments(docFile);
            }
        }
    }
}