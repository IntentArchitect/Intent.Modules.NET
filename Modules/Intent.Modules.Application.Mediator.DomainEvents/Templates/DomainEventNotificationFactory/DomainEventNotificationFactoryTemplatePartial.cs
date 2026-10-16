using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Application.Mediator.DomainEvents.Templates.TypedDomainEventNotification;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.DomainEvents.Templates.DomainEventNotificationFactory
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DomainEventNotificationFactoryTemplate : CSharpTemplateBase<IList<DomainEventModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Mediator.DomainEvents.DomainEventNotificationFactory";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DomainEventNotificationFactoryTemplate(IOutputTarget outputTarget, IList<DomainEventModel> model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MediatorAbstractions(outputTarget));
            AddTypeSource(TemplateRoles.Domain.Events);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass("DomainEventNotificationFactory", @class =>
                {
                    @class.Static();
                    @class.WithComments(new[]
                    {
                        "/// <summary>",
                        "/// Maps a domain event to its Mediator notification without reflection: Mediator's source",
                        "/// generator cannot see open generic notification types such as DomainEventNotification&lt;T&gt;.",
                        "/// Every domain event has a case here. An unmapped event throws rather than being dropped.",
                        "/// </summary>",
                    });

                    @class.AddMethod(UseType("Mediator.INotification"), "Create", method =>
                    {
                        method.Static();
                        method.AddParameter(GetTypeName(TemplateRoles.Domain.Common.EventBase), "domainEvent");
                        method.AddSwitchStatement("domainEvent", @switch =>
                        {
                            // A derived event must precede its base event, otherwise the base case swallows it.
                            foreach (var domainEvent in Model.OrderByDescending(GetInheritanceDepth).ThenBy(x => x.Name, StringComparer.Ordinal))
                            {
                                @switch.AddCase($"{GetTypeName(TemplateRoles.Domain.Events, domainEvent)} @event", block => block
                                    .AddStatement($"return new {GetTypeName(TypedDomainEventNotificationTemplate.TemplateId, domainEvent)}(@event);"));
                            }

                            @switch.AddDefault(block => block
                                .AddStatement($"throw new {UseType("System.InvalidOperationException")}($\"No notification is mapped for domain event {{domainEvent.GetType().Name}}. Add one to {{nameof(DomainEventNotificationFactory)}}.\");"));
                        });
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        private static int GetInheritanceDepth(DomainEventModel domainEvent)
        {
            var depth = 0;
            var current = domainEvent;
            // The bound also guards against a cyclic model.
            while (depth < 50 && current.Generalizations().SingleOrDefault()?.TypeReference?.Element?.AsDomainEventModel() is { } parent)
            {
                depth++;
                current = parent;
            }

            return depth;
        }
    }
}
