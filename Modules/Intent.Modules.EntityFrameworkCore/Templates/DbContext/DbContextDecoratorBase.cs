using System.Collections.Generic;
using System.Linq;
using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;
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

        public virtual IEnumerable<string> GetMethods() => Enumerable.Empty<string>();

        public virtual void OnBeforeTemplateExecution() { }

        public virtual IEnumerable<string> GetOnModelCreatingStatements() => Enumerable.Empty<string>();

        public virtual IEnumerable<string> GetTypeConfigurationParameters(EntityTypeConfigurationCreatedEvent @event) => Enumerable.Empty<string>();

        public int Priority { get; set; } = 0;
    }
}