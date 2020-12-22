using System.Collections.Generic;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Infrastructure.DependencyInjection.Templates.DependencyInjection;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Infrastructure.DependencyInjection.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ApplicationStartupDecorator : StartupDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Infrastructure.DependencyInjection.ApplicationStartupDecorator";

        private readonly StartupTemplate _template;

        public ApplicationStartupDecorator(StartupTemplate template)
        {
            _template = template;
        }

        public override string ConfigureServices()
        {
            return "services.AddInfrastructure(Configuration);";
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[] { _template.GetTemplate<IClassProvider>(DependencyInjectionTemplate.TemplateId).Namespace };
        }
    }
}