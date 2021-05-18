using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Templates.CommandHandler
{
    [IntentManaged(Mode.Merge)]
    public abstract class CommandHandlerDecorator : ITemplateDecorator
    {
        public int Priority { get; protected set; } = 0;

        public virtual IEnumerable<RequiredService> GetRequiredServices() => new List<RequiredService>();

        public virtual string GetImplementation() => null;
    }
}