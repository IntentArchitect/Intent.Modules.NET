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
                    @class.AddField($"{UseType("System.Collections.Generic.List")}<QueueStorageEntry>", "_entries", @field =>
                    {
                        @field
                            .PrivateReadOnly()
                            .WithAssignment(new CSharpStatement("new List<QueueStorageEntry>()"));
                    }).AddProperty($"{UseType("System.Collections.Generic.IReadOnlyList")}<QueueStorageEntry>", "Entries", @prop =>
                    {
                        @prop.WithoutSetter().Getter.WithExpressionImplementation("_entries");
                    });

                    @class.AddMethod("void", "AddQueue", mth =>
                    {
                        mth.AddGenericParameter("TMessage");
                        mth.AddParameter("string", "endpoint")
                            .AddParameter("string", "queueName")
                            .AddParameter("bool", "createQueue")
                            .AddParameter("int", "maxMessages");

                        mth.AddInvocationStatement($"{UseType("System.ArgumentNullException")}.ThrowIfNull", invoc => invoc.AddArgument("queueName"));
                        mth.AddInvocationStatement($"{UseType("System.ArgumentNullException")}.ThrowIfNull", invoc => invoc.AddArgument("endpoint"));

                        mth.AddIfStatement("maxMessages <=0 || maxMessages > 32", @if =>
                        {
                            @if.AddAssignmentStatement("maxMessages", new CSharpStatement("10;"));
                        });

                        var newEntryStatement = new CSharpInvocationStatement("new QueueStorageEntry")
                            .AddArgument("typeof(TMessage)")
                            .AddArgument("endpoint")
                            .AddArgument("queueName")
                            .AddArgument("createQueue")
                            .AddArgument("maxMessages").WithoutSemicolon();

                        mth.AddInvocationStatement("_entries.Add", invoc => invoc.AddArgument(newEntryStatement));
                    });
                }).AddRecord("QueueStorageEntry", @record =>
                {
                    record.AddPrimaryConstructor(ctor =>
                     {
                         ctor
                             .AddParameter("Type", "MessageType")
                             .AddParameter("string", "Endpoint")
                             .AddParameter("string", "QueueName")
                             .AddParameter("bool", "CreateQueue")
                             .AddParameter("int", "MaxMessages");
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