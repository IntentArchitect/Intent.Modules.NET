using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AspNetCore.Swashbuckle.Templates.SwashbuckleConfiguration;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.AspNetCore.Swashbuckle.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class SwashbuckleCoreWebStartupDecorator : StartupDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AspNetCore.Swashbuckle.SwashbuckleCoreWebStartupDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly StartupTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public SwashbuckleCoreWebStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _template.AddTemplateDependency(SwashbuckleConfigurationTemplate.TemplateId);
        }

        public override int Priority => 100;

        public override string ConfigureServices()
        {
            return @"services.ConfigureSwagger(Configuration);";
        }

        public override string Configuration()
        {
            return @"
            app.UseSwagger();
            app.UseSwaggerUI();";
        }
    }
}
