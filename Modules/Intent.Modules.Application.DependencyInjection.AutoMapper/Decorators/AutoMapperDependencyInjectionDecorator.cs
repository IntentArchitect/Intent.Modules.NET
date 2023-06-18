using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Infrastructure.DependencyInjection.Templates.DependencyInjection;
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

        [IntentManaged(Mode.Fully)]
        private readonly DependencyInjectionTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public AutoMapperDependencyInjectionDecorator(DependencyInjectionTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _template.AddNugetDependency("AutoMapper.Extensions.Microsoft.DependencyInjection", "7.0.*");
        }

        public override string ServiceRegistration()
        {
            var applicationDependencyTypeName = _template.GetTypeName("Intent.Application.DependencyInjection.DependencyInjection");
            var assemblyTypeName = _template.UseType("System.Reflection.Assembly");

            // TODO JL: Look into GetTypeName system to work out why this needs ".Application" manually added
            return $"services.AddAutoMapper({assemblyTypeName}.GetExecutingAssembly(), typeof(Application.{applicationDependencyTypeName}).Assembly);";
        }

        public IEnumerable<string> DeclareUsings()
        {
            yield return "AutoMapper";
        }
    }
}