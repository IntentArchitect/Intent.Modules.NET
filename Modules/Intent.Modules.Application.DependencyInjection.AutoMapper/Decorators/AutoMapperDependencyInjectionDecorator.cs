using System.Collections.Generic;
using Intent.Modules.Application.DependencyInjection.Templates.DependencyInjection;
using Intent.Modules.Common;
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

        private readonly DependencyInjectionTemplate _template;

        public AutoMapperDependencyInjectionDecorator(DependencyInjectionTemplate template)
        {
            _template = template;
            _template.AddNugetDependency("AutoMapper.Extensions.Microsoft.DependencyInjection", "7.0.*");
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