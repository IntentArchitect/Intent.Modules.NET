using System.Collections.Generic;
using Intent.Modules.Application.DependencyInjection.Templates.DependencyInjection;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;
using Intent.Engine;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Application.DependencyInjection.FluentValidation.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class FluentValidationDependencyInjectionDecorator : DependencyInjectionDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Application.DependencyInjection.FluentValidation.FluentValidationDependencyInjectionDecorator";

        private readonly DependencyInjectionTemplate _template;
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public FluentValidationDependencyInjectionDecorator(DependencyInjectionTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _template.AddNugetDependency("FluentValidation.DependencyInjectionExtensions", "9.3.0");
        }

        public override string ServiceRegistration()
        {
            return "services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());";
        }

        public IEnumerable<string> DeclareUsings()
        {
            yield return "FluentValidation";
        }
    }
}