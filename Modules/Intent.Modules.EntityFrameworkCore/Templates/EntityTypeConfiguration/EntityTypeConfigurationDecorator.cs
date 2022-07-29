using System;
using System.Collections.Generic;
using System.Linq;
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

        [Obsolete($"Use instead {nameof(BeforeAttributeStatements)}")]
        public virtual string BeforeAttributes() => null;

        [Obsolete($"Use instead {nameof(AfterAttributeStatements)}")]
        public virtual string AfterAttributes() => null;

        public virtual IEnumerable<string> GetClassMembers() => Enumerable.Empty<string>();
        public virtual IEnumerable<string> GetConstructorParameters() => Enumerable.Empty<string>();
        public virtual IEnumerable<string> GetConstructorBodyStatements() => Enumerable.Empty<string>();
        public virtual IEnumerable<string> BeforeAttributeStatements() => Enumerable.Empty<string>();
        public virtual IEnumerable<string> AfterAttributeStatements() => Enumerable.Empty<string>();
    }
}