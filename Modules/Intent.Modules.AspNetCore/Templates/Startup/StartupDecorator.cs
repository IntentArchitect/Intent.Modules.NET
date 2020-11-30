using System.Collections.Generic;
using Intent.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Ignore)]

namespace Intent.Modules.AspNetCore.Templates.Startup
{
    public abstract class StartupDecorator : ITemplateDecorator
    {
        public virtual string ConfigureServices() => @"";
        public virtual string Configuration() => @"";
        public virtual string Methods() => @"";
        public virtual IEnumerable<string> RequiredNamespaces() => new string[0];

        public virtual int Priority { get; set; } = 0;
    }
}