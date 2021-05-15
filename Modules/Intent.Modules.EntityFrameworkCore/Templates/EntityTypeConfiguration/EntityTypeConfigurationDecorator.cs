using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration
{
    [IntentManaged(Mode.Merge)]
    public abstract class EntityTypeConfigurationDecorator : ITemplateDecorator
    {
        public int Priority { get; protected set; } = 0;

        public virtual string AfterAttributes()
        {
            return null;
        }
    }
}