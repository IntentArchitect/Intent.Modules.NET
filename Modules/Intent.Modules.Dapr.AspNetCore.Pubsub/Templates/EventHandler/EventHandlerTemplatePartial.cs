using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.EventHandler
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class EventHandlerTemplate : CSharpTemplateBase<MessageModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.Pubsub.EventHandler";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EventHandlerTemplate(IOutputTarget outputTarget, MessageModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.MediatR);

            CSharpFile = new CSharpFile(
                    @namespace: this.GetNamespace(),
                    relativeLocation: this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("MediatR")
                .IntentManagedFully()
                .AddClass($"{Model.Name.RemoveSuffix("Event", "Message")}EventHandler", @class => @class
                    .ImplementsInterface($"IRequestHandler<{this.GetIntegrationEventMessageName()}>")
                    .AddAttribute("[IntentManaged(Mode.Merge, Signature = Mode.Fully)]")
                    .AddConstructor(constructor => constructor
                        .AddAttribute("[IntentManaged(Mode.Ignore)]")
                    )
                    .AddMethod("Task<Unit>", "Handle", method => method
                        .Async()
                        .AddAttribute("[IntentManaged(Mode.Fully, Body = Mode.Ignore)]")
                        .AddParameter(this.GetIntegrationEventMessageName(), "@event")
                        .AddParameter("CancellationToken", "cancellationToken")
                        .AddStatement("throw new NotImplementedException();")
                    )
                );
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
    }
}