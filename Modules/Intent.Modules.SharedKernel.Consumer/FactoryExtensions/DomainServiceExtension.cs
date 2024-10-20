using Intent.Engine;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.SharedKernel.Consumer.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DomainServiceExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.SharedKernel.Consumer.DomainServiceExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            base.OnAfterTemplateRegistrations(application);
            var sharedKernel = TemplateHelper.GetSharedKernel();

            UpdateDomainServiceContract(application, sharedKernel);
            UpdateDomainService(application, sharedKernel);
            UpdateDomainEventHandlers(application, sharedKernel);
        }

        private void UpdateDomainEventHandlers(IApplication application, SharedKernel sharedKernel)
        {
            var handlerTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Intent.MediatR.DomainEvents.DomainEventHandler");
            foreach (var handlerTemplate in handlerTemplates)
            {
                var model = (handlerTemplate as IntentTemplateBase<DomainEventHandlerModel>)?.Model;
                if (model is null)
                {
                    continue;
                }

                List<DomainEventHandlerAssociationTargetEndModel> fixes = [];
                foreach (var handledDomainEvents in model.HandledDomainEvents())
                {
                    if (handledDomainEvents.TypeReference.Element.Package.Name.StartsWith(sharedKernel.ApplicationName))
                    {
                        fixes.Add(handledDomainEvents);
                    }
                }

                if (fixes.Any())
                {
                    handlerTemplate.CSharpFile.OnBuild(file =>
                    {
                        var @class = file.Classes.First();
                        @class.Interfaces[0] = @class.Interfaces[0].Replace("INotificationHandler<DomainEventNotification<", $"INotificationHandler<{sharedKernel.ApplicationName}.Application.Common.Models.DomainEventNotification<");
                        foreach (var toFix in fixes)
                        {
                            var method = @class.GetReferenceForModel(toFix) as CSharpClassMethod;
                            method.Parameters[0].WithType($"{sharedKernel.ApplicationName}.Application.Common.Models.{method.Parameters[0].Type}");
                        }

                    }, 10000).AfterBuild(file => 
                    {
                        if (model.HandledDomainEvents().Count == fixes.Count)
                        {
                            file.Template.RemoveUsing($"{application.Name.ToCSharpIdentifier()}.Application.Common.Models");
                            file.Template.RemoveUsing($"{application.Name.ToCSharpIdentifier()}.Domain.Events");
                        }
                    }, 100);
                }
            }
        }

        private void UpdateDomainService(IApplication application, SharedKernel sharedKernel)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.MediatR.DomainEvents.DomainEventService");
            if (template is null)
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                @class.ImplementsInterface($"{sharedKernel.ApplicationName}.Application.Common.Interfaces.IDomainEventService");
                @class.AddMethod("Task", "Publish", method => 
                {
                    method
                        .Async()
                        .AddParameter($"{sharedKernel.ApplicationName}.Domain.Common.DomainEvent", "domainEvent")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    method.AddStatement("_logger.LogInformation(\"Publishing domain event. Event - {event}\", domainEvent.GetType().Name);");
                    method.AddStatement("await _mediator.Publish(GetNotificationCorrespondingToDomainEvent(domainEvent), cancellationToken);");
                });

                @class.AddMethod("INotification", "GetNotificationCorrespondingToDomainEvent", method =>
                {
                    method
                        .Private()
                        .AddParameter($"{sharedKernel.ApplicationName}.Domain.Common.DomainEvent", "domainEvent");
                    method.AddStatement(@$"var result = Activator.CreateInstance(
                typeof({sharedKernel.ApplicationName}.Application.Common.Models.DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent);");
                    method.AddStatement(@"if (result == null)
                throw new Exception($""Unable to create DomainEventNotification<{domainEvent.GetType().Name}>"");");
                    method.AddStatement("return (INotification)result;");
                });
            });
        }

        private void UpdateDomainServiceContract(IApplication application, SharedKernel sharedKernel)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.DomainEvents.DomainEventServiceInterface");
            if (template is null)
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Interfaces.First();
                @class.ExtendsInterface($"{sharedKernel.ApplicationName}.Application.Common.Interfaces.IDomainEventService");

            });
        }
    }
}