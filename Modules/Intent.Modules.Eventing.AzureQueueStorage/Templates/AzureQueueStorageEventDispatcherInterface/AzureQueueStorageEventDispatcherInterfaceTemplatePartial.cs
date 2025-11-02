using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageEnvelope;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageEventDispatcherInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureQueueStorageEventDispatcherInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.AzureQueueStorage.AzureQueueStorageEventDispatcherInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureQueueStorageEventDispatcherInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface($"IAzureQueueStorageEventDispatcher", @interface =>
                {
                    @interface.AddMethod(UseType("System.Threading.Tasks.Task"), "DispatchAsync", mth =>
                    {
                        AddTemplateDependency(AzureQueueStorageEnvelopeTemplate.TemplateId);

                        mth.AddParameter(UseType("System.IServiceProvider"), "serviceProvider")
                        .AddParameter(this.GetAzureQueueStorageEnvelopeName(), "message")
                        .AddParameter(UseType("System.Text.Json.JsonSerializerOptions"), "serializerOptions")
                        .AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");
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