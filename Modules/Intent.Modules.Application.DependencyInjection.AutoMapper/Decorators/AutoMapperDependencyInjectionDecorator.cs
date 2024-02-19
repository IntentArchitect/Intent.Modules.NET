using System.Collections.Generic;
using System.Threading;
using Intent.Engine;
using Intent.Modules.Application.DependencyInjection.Templates.DependencyInjection;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Application.DependencyInjection.AutoMapper.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class AutoMapperDependencyInjectionDecorator : DependencyInjectionDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Application.DependencyInjection.AutoMapper.AutoMapperDependencyInjectionDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly DependencyInjectionTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public AutoMapperDependencyInjectionDecorator(DependencyInjectionTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _template.AddNugetDependency("AutoMapper", "13.0.1");

            application.EventDispatcher.Publish(new RemoveNugetPackageEvent("AutoMapper.Extensions.Microsoft.DependencyInjection", template.OutputTarget));
        }

        public override string ServiceRegistration()
        {
            return "services.AddAutoMapper(Assembly.GetExecutingAssembly());";
        }

        public IEnumerable<string> DeclareUsings()
        {
            yield return "AutoMapper";
        }
    }
}