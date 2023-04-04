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
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.DomainEvents.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DomainEventsDecoratorExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.DomainEvents.DomainEventsDecoratorExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => -10;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var entityStateTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Domain.Entity.Primary));
            foreach (var template in entityStateTemplates)
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.FirstOrDefault();
                    if (@class.TryGetMetadata<ClassModel>("model", out var model) &&
                        model.IsAggregateRoot() && model.ParentClass == null)
                    {
                        @class.ImplementsInterface(template.GetTypeName(HasDomainEventInterfaceTemplate.TemplateId));
                        @class.AddProperty($"{template.UseType("System.Collections.Generic.List")}<{template.GetTypeName(DomainEventBaseTemplate.TemplateId)}>", "DomainEvents", property =>
                        {
                            property.WithInitialValue($"new {property.Type}()");
                            property.AddMetadata("non-persistent", true);
                        });
                    }
                });
            }

            var entityInterfaceTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Domain.Entity.Interface));
            foreach (var template in entityInterfaceTemplates.Where(x => x.CSharpFile.Interfaces.Any()))
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var @interface = file.Interfaces.First();
                    if (@interface.TryGetMetadata<ClassModel>("model", out var model) &&
                        model.IsAggregateRoot() && model.ParentClass == null)
                    {
                        @interface.ExtendsInterface(template.GetTypeName(HasDomainEventInterfaceTemplate.TemplateId));
                    }
                });
            }
        }
    }
}