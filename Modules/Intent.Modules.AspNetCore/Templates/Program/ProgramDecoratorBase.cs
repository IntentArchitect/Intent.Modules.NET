using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Templates.Program
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public abstract class ProgramDecoratorBase : ITemplateDecorator
    {
        public int Priority { get; protected set; } = 0;
    }
}