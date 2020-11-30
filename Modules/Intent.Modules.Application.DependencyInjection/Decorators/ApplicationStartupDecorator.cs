using System.Collections.Generic;
using Intent.Modules.Application.DependencyInjection.Templates.DependencyInjection;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Application.DependencyInjection.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ApplicationStartupDecorator : StartupDecorator
    {
        public const string DecoratorId = "Application.DependencyInjection.ApplicationStartupDecorator";

        private readonly StartupTemplate _template;

        public ApplicationStartupDecorator(StartupTemplate template)
        {
            _template = template;
        }

        public override string ConfigureServices()
        {
            return "services.AddApplication();";
        }

        public override IEnumerable<string> RequiredNamespaces()
        {
            return new[] { _template.GetTemplate<IClassProvider>(DependencyInjectionTemplate.TemplateId).Namespace };
        }
    }
}