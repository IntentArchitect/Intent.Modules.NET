using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Ignore)]

namespace Intent.Modules.AspNetCore.Templates.Startup
{
    public abstract class StartupDecorator : ITemplateDecorator
    {
        public virtual string ConfigureServices() => @"";
        public virtual string Configuration() => @"";
        public virtual string EndPointMappings() => @"";

        // Cannot convert normal strings under the hood to use the
        // builder pattern, hence why we need this obsolete warning.
        [Obsolete("Underlying Builder Pattern should be used instead. This method is no longer processed.")]
        public virtual string Methods() => @"";

        public virtual int Priority { get; set; } = 0;
    }
}