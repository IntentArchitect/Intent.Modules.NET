using Intent.Modules.Application.DependencyInjection.Templates.DependencyInjection;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Interop.AspNetCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class MediatRDependencyInjectionDecorator : DependencyInjectionDecorator
    {
        public const string DecoratorId = "Application.MediatR.Interop.AspNetCore.MediatRDependencyInjectionDecorator";

        private readonly DependencyInjectionTemplate _template;

        public MediatRDependencyInjectionDecorator(DependencyInjectionTemplate template)
        {
            _template = template;
            _template.AddNugetDependency("MediatR.Extensions.Microsoft.DependencyInjection", "9.0.*");
        }

        public override string ServiceRegistration()
        {
            return "services.AddMediatR(Assembly.GetExecutingAssembly());";
        }
    }
}