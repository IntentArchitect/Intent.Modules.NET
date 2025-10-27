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

namespace Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageOptions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureQueueStorageOptionsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.AzureQueueStorage.AzureQueueStorageOptions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureQueueStorageOptionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"AzureQueueStorageOptions", @class =>
                {
                    @class.AddProperty("string?", "DefaultEndpoint");

                    @class.AddProperty($"{UseType("System.Collections.Generic.Dictionary")}<string, QueueDefinition>", "Queues", prop =>
                    {
                        prop.WithInitialValue("new Dictionary<string, QueueDefinition>()");
                    });

                    @class.AddProperty($"{UseType("System.Collections.Generic.Dictionary")}<string, string>", "PublishMap", prop =>
                    {
                        prop.WithInitialValue("new Dictionary<string, string>()");
                    });

                }).AddClass("QueueDefinition", @class =>
                {
                    @class.AddProperty("string?", "QueueName");
                    @class.AddProperty("string?", "Endpoint");
                    @class.AddProperty("bool", "CreateQueue");
                    @class.AddProperty("int", "MaxMessages", prop => prop.WithInitialValue("10"));
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