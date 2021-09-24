using System.Collections.Generic;
using Intent.Modules.Application.DependencyInjection.Templates.DependencyInjection;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Engine;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Application.DependencyInjection.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ApplicationStartupDecorator : StartupDecorator, IDeclareUsings
    {
        public const string DecoratorId = "Application.DependencyInjection.ApplicationStartupDecorator";

        private readonly StartupTemplate _template;
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge, Signature = Mode.Fully, Body = Mode.Fully)]
        public ApplicationStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public override string ConfigureServices()
        {
            return "services.AddApplication();";
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[] { _template.GetTemplate<IClassProvider>(DependencyInjectionTemplate.TemplateId).Namespace };
        }
    }
}