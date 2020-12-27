using System.Collections.Generic;
using Intent.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Ignore)]

namespace Intent.Modules.AspNetCore.Templates.Startup
{
    public abstract class StartupDecorator : ITemplateDecorator
    {
        public virtual string ConfigureServices() => @"";
        public virtual string Configuration() => @"";
        public virtual string Methods() => @"";

        public virtual int Priority { get; set; } = 0;
    }
}