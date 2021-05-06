using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;
using System.Collections.Generic;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.AspNetCore.Swashbuckle.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class SwashbuckleCoreWebStartupDecorator : StartupDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AspNetCore.Swashbuckle.SwashbuckleCoreWebStartupDecorator";

        private readonly StartupTemplate _template;
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public SwashbuckleCoreWebStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _template.AddNugetDependency(NugetPackages.SwashbuckleAspNetCore);
            _application = application;
        }

        public override int Priority => 100;

        public override string ConfigureServices()
        {
            return @"ConfigureSwagger(services);";
        }

        public override string Configuration()
        {
            return @"InitializeSwagger(app);";
        }

        public override string Methods()
        {
            return $@"        
        private void ConfigureSwagger(IServiceCollection services)
        {{
            services.AddSwaggerGen();
            services.Configure<SwaggerGenOptions>(Configuration.GetSection(""Swashbuckle:SwaggerGen""));
            services.Configure<SwaggerOptions>(Configuration.GetSection(""Swashbuckle:Swagger""));
            services.Configure<SwaggerUIOptions>(Configuration.GetSection(""Swashbuckle:SwaggerUI""));
        }}

        private void InitializeSwagger(IApplicationBuilder app)
        {{
            app.UseSwagger();
            app.UseSwaggerUI();
        }}";
        }

        public IEnumerable<string> DeclareUsings()
        {
            yield return "Microsoft.OpenApi.Models";
        }
    }
}
