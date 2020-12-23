using System.Collections.Generic;
using Intent.Modules.Application.DependencyInjection.Templates.DependencyInjection;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

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

        public FluentValidationDependencyInjectionDecorator(DependencyInjectionTemplate template)
        {
            _template = template;
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