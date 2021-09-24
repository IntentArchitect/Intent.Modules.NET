using System.Collections.Generic;
using Intent.Modules.Application.DependencyInjection.Templates.DependencyInjection;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;
using Intent.Engine;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Application.DependencyInjection.MediatR.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class MediatRDependencyInjectionDecorator : DependencyInjectionDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Application.DependencyInjection.MediatR.MediatRDependencyInjectionDecorator";

        private readonly DependencyInjectionTemplate _template;
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public MediatRDependencyInjectionDecorator(DependencyInjectionTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _template.AddNugetDependency("MediatR.Extensions.Microsoft.DependencyInjection", "9.0.*");
        }

        public override string ServiceRegistration()
        {
            return "services.AddMediatR(Assembly.GetExecutingAssembly());";
        }

        public IEnumerable<string> DeclareUsings()
        {
            yield return "MediatR";
        }
    }
}