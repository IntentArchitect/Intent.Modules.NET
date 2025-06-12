using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureEventGrid.Templates.InboundCloudEventBehavior
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class InboundCloudEventBehaviorTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.AzureEventGrid.InboundCloudEventBehavior";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public InboundCloudEventBehaviorTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Azure.Messaging")
                .AddClass($"InboundCloudEventBehavior", @class =>
                {
                    @class.ImplementsInterface(this.GetAzureEventGridConsumerBehaviorInterfaceName());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(this.GetEventContextName(), "eventContext", param => param.IntroduceReadonlyField());
                    });

                    @class.AddMethod($"Task<CloudEvent>", "HandleAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("CloudEvent", "cloudEvent");
                        method.AddParameter("CloudEventBehaviorDelegate", "next");
                        method.AddOptionalCancellationTokenParameter();

                        method.AddForEachStatement("extensionAttribute", "cloudEvent.ExtensionAttributes", f =>
                        {
                            f.AddStatement("_eventContext.AdditionalData.Add(extensionAttribute.Key, extensionAttribute.Value);");
                        });

                        method.AddStatement("return await next(cloudEvent, cancellationToken);");
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
    }
}