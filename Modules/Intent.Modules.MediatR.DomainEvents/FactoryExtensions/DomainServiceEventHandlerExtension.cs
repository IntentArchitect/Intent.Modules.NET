using System.Linq;
using System.Reflection;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.DomainEvents.Templates.DomainEvent;
using Intent.Modules.MediatR.DomainEvents.Templates;
using Intent.Modules.MediatR.DomainEvents.Templates.DomainEventNotification;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.MediatR.DomainEvents.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DomainServiceEventHandlerExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.MediatR.DomainEvents.DomainServiceEventHandlerExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var domainEvents = application.MetadataManager.Domain(application).GetDomainEventModels();
            foreach (var domainEvent in domainEvents)
            {
                foreach (var targetHandler in domainEvent.AssociatedClasses().Select(x => x.Element).Distinct().ToList())
                {
                    var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnModel(TemplateRoles.Domain.DomainServices.Implementation, targetHandler));
                    template?.CSharpFile.OnBuild(file =>
                    {
                        file.AddUsing("System");
                        file.AddUsing("System.Threading");
                        file.AddUsing("System.Threading.Tasks");

                        var @class = file.Classes.First();
                        @class.ImplementsInterface($"{template.UseType("MediatR.INotificationHandler")}<{template.GetTypeName(DomainEventNotificationTemplate.TemplateId)}<{template.GetTypeName(DomainEventTemplate.TemplateId, domainEvent)}>>");
                        @class.AddMethod("Task", "Handle", method =>
                        {
                            method.Async();
                            method.AddAttribute(CSharpIntentManagedAttribute.IgnoreBody());
                            method.AddParameter($"{template.GetDomainEventNotificationName()}<{template.GetTypeName(DomainEventTemplate.TemplateId, domainEvent)}>", "notification");
                            method.AddParameter($"CancellationToken", "cancellationToken");
                            method.AddStatement($"// TODO: Implement {method.Name} {@class.Name}) functionality");
                            method.AddStatement("throw new NotImplementedException(\"Implement your handler logic here...\");");
                        });
                    });
                }
            }
        }
    }
}