using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.AspNetCore.DependencyInjection.Templates.ApplicationDependencyInjection
{
    [IntentManaged(Mode.Merge)]
    public abstract class ApplicationDependencyInjectionDecorator : ITemplateDecorator
    {
        public int Priority { get; protected set; } = 0;

        public virtual string ServiceRegistration()
        {
            return null;
        }
    }
}