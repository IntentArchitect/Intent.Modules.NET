using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.AspNetCore.DependencyInjection.Templates.InfrastructureDependencyInjection
{
    [IntentManaged(Mode.Merge)]
    public abstract class InfrastructureDependencyInjectionDecorator : ITemplateDecorator
    {
        public int Priority { get; protected set; } = 0;

        public virtual string ServiceRegistration()
        {
            return null;
        }
    }
}