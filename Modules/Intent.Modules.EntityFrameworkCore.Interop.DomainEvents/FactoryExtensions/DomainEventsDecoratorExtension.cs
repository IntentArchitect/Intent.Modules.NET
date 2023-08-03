using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.DomainEvents.Templates.DomainEvent;
using Intent.Modules.DomainEvents.Templates.DomainEventBase;
using Intent.Modules.DomainEvents.Templates.DomainEventServiceInterface;
using Intent.Modules.DomainEvents.Templates.HasDomainEventInterface;
using Intent.Modules.EntityFrameworkCore.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Interop.DomainEvents.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DomainEventsDecoratorExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.Interop.DomainEvents.DomainEventsDecoratorExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => -10;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var designTimeDbContextFactoryTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Infrastructure.Data.DesignTimeDbContextFactory"));
            foreach (var template in designTimeDbContextFactoryTemplates)
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    var method = @class.FindMethod("CreateDbContext");
                    var returnStatement = (CSharpInvocationStatement)method.Statements.LastOrDefault(p => p.HasMetadata("return-statement"));
                    returnStatement?.AddArgument("null", arg => arg.AddMetadata("domain-event", true));
                });
            }
        }
    }
}