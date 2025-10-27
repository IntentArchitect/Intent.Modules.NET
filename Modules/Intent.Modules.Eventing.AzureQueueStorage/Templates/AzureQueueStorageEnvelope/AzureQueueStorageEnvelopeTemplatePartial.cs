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

namespace Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageEnvelope
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureQueueStorageEnvelopeTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.AzureQueueStorage.AzureQueueStorageEnvelope";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureQueueStorageEnvelopeTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"AzureQueueStorageEnvelope", @class =>
                {
                    @class.AddProperty("string", "MessageType");
                    @class.AddProperty("object", "Payload");

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("object", "payload");
                        ctor.AddAssignmentStatement("MessageType", new CSharpStatement("payload.GetType().FullName;"));
                        ctor.AddAssignmentStatement("Payload", new CSharpStatement("payload;"));
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