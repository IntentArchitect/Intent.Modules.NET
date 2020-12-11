using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System.Collections.Generic;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.Selfhost.Templates.IdentityConfig
{
    [IntentManaged(Mode.Merge)]
    public abstract class IdentityConfigDecorator : ITemplateDecorator
    {
        public int Priority { get; protected set; } = 0;

        public abstract EntityCollection GetClients();
    }

    public class EntityCollection : List<Entity>
    {
    }

    public class Entity : Dictionary<string, string>
    {
    }
}