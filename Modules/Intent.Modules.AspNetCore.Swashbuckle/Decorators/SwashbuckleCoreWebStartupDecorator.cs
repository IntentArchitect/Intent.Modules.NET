using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.Swashbuckle.Decorators
{
    public class SwashbuckleCoreWebStartupDecorator : StartupDecorator, IDeclareUsings
    {
        public const string Identifier = "Intent.AspNetCore.Swashbuckle.StartupDecorator";
        private readonly StartupTemplate _template;

        public SwashbuckleCoreWebStartupDecorator(StartupTemplate template)
        {
            _template = template;
            _template.AddNugetDependency(NugetPackages.SwashbuckleAspNetCore);
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
            services.AddSwaggerGen(c =>
            {{
                c.SwaggerDoc(""v1"", new OpenApiInfo {{ Title = ""{_template.OutputTarget.Application.Name} API"", Version = ""v1"" }});
            }});
        }}

        private void InitializeSwagger(IApplicationBuilder app)
        {{
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {{
                c.SwaggerEndpoint(""/swagger/v1/swagger.json"", ""{_template.OutputTarget.Application.Name} API V1"");
            }});
        }}";
        }

        public IEnumerable<string> DeclareUsings()
        {
            yield return "Microsoft.OpenApi.Models";
        }
    }
}
