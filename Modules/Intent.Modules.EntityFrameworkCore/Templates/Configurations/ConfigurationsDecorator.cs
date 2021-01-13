using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates.Configurations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public abstract class ConfigurationsDecorator : ITemplateDecorator
    {
        public int Priority { get; protected set; } = 0;

        public virtual string BeforeAttributes() => null;

        public virtual string AfterAttributes() => null;
    }
}