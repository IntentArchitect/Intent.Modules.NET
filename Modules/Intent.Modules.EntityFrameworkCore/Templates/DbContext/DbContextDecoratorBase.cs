using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;
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
        public virtual void DecorateDbContext(CSharpClass @class)
        {
        }

        [Obsolete("Use CSharpFile builder pattern to decorate")]
        public virtual string GetBaseClass() { return null; }

        [Obsolete("Use CSharpFile builder pattern to decorate")]
        public virtual string GetBaseInterfaces() { return null; }

        [Obsolete("Use CSharpFile builder pattern to decorate")]
        public virtual IEnumerable<string> GetPrivateFields() => null;

        [Obsolete("Use CSharpFile builder pattern to decorate")]
        public virtual IEnumerable<string> GetConstructorParameters() => null;

        [Obsolete("Use CSharpFile builder pattern to decorate")]
        public virtual IEnumerable<string> GetConstructorInitializations() => null;

        [Obsolete("Use CSharpFile builder pattern to decorate")]
        public virtual string BeforeCallToSaveChangesAsync() => null;

        [Obsolete("Use CSharpFile builder pattern to decorate")]
        public virtual string AfterCallToSaveChangesAsync() => null;

        [Obsolete("Use CSharpFile builder pattern to decorate")]
        public virtual IEnumerable<string> GetMethods() => Enumerable.Empty<string>();

        [Obsolete("Use CSharpFile builder pattern to decorate")]
        public virtual void OnBeforeTemplateExecution() { }

        [Obsolete("Use CSharpFile builder pattern to decorate")]
        public virtual IEnumerable<string> GetOnModelCreatingStatements() => Enumerable.Empty<string>();

        [Obsolete("Use CSharpFile builder pattern to decorate")]
        public virtual IEnumerable<string> GetTypeConfigurationParameters(EntityTypeConfigurationCreatedEvent @event) => Enumerable.Empty<string>();

        public int Priority { get; set; } = 0;
    }
}