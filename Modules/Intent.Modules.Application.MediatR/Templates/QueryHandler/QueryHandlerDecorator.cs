using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Templates.QueryHandler
{
    [IntentManaged(Mode.Merge)]
    public abstract class QueryHandlerDecorator : ITemplateDecorator
    {
        public int Priority { get; protected set; } = 0;
    }
}