using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Ignore)]

namespace Intent.Modules.EntityFrameworkCore.Templates.DbContext
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public abstract class DbContextDecoratorBase : ITemplateDecorator
    {
        public virtual string GetBaseClass() { return null; }

        public virtual string GetBaseInterfaces() { return null; }

        public virtual IEnumerable<string> GetPrivateFields() => null;

        public virtual IEnumerable<string> GetConstructorParameters() => null;

        public virtual IEnumerable<string> GetConstructorInitializations() => null;

        public virtual string BeforeCallToSaveChangesAsync() => null;

        public virtual string AfterCallToSaveChangesAsync() => null;

        public virtual IEnumerable<string> GetMethods() { return new List<string>(); }

        public int Priority { get; set; } = 0;
    }
}