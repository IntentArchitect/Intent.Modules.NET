using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.AzureQueueStorage.Settings;
using Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageOptions;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageEventBus
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureQueueStorageEventBusTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.AzureQueueStorage.AzureQueueStorageEventBus";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureQueueStorageEventBusTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.AzureStorageQueues(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"AzureQueueStorageEventBus", @class =>
                {
                    @class.ImplementsInterface(this.GetEventBusInterfaceName());

                    @class.AddField($"{UseType("System.Collections.Generic.List")}<MessageEntry>", "_messageQueue", @field =>
                    {
                        field.PrivateReadOnly();
                        @field.WithAssignment(new CSharpStatement("new List<MessageEntry>()"));
                    });
                    AddTemplateDependency(AzureQueueStorageOptionsTemplate.TemplateId);
                    @class.AddField($"{UseType("System.Collections.Generic.Dictionary")}<string, QueueStorageEntry>", "_lookup", @field => field.PrivateReadOnly());
                    @class.AddField($"{UseType("System.Text.Json.JsonSerializerOptions")}", "_serializerOptions", @field => field.PrivateReadOnly());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"{UseType("Microsoft.Extensions.Logging.ILogger")}<{@class.Name}>", "logger", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter($"{UseType("Microsoft.Extensions.Options.IOptions")}<{this.GetAzureQueueStorageOptionsName()}>", "options");


                        AddUsing("System.Linq");
                        ctor.AddObjectInitStatement("_lookup",
                            new CSharpInvocationStatement("options.Value.Entries.ToDictionary")
                                .AddArgument(new CSharpLambdaBlock("k").WithExpressionBody("k.MessageType.FullName!")));

                        ctor.AddObjectInitStatement("_serializerOptions",
                            new CSharpObjectInitializerBlock("new JsonSerializerOptions")
                                .AddInitStatement("PropertyNamingPolicy", "JsonNamingPolicy.CamelCase")
                                .AddInitStatement("WriteIndented", "false")
                                .AddInitStatement("PropertyNameCaseInsensitive", "true").WithSemicolon());
                    });

                    @class.AddMethod(UseType("System.Threading.Tasks.Task"), "FlushAllAsync", mth =>
                    {
                        mth.Async();
                        mth.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", @param => @param.WithDefaultValue("default"));

                        mth.AddIfStatement("_messageQueue.Count == 0", @if =>
                        {
                            @if.AddReturn("");
                        });

                        mth.AddObjectInitStatement("var groupedMessages", new CSharpInvocationStatement("_messageQueue.GroupBy")
                            .AddArgument(new CSharpLambdaBlock("entry")
                                .AddObjectInitStatement("var publisherEntry", "_lookup[entry.Type.FullName!];")
                                .AddReturn("(publisherEntry.Endpoint, publisherEntry.QueueName, publisherEntry.CreateQueue)")));

                        mth.AddForEachStatement("group", "groupedMessages", @foreach =>
                        {
                            @foreach.AddObjectInitStatement("var queueName", "group.Key.QueueName;");
                            @foreach.AddObjectInitStatement("var endpoint", "group.Key.Endpoint;");
                            @foreach.AddObjectInitStatement("var createQueue", "group.Key.CreateQueue;");

                            @foreach.AddObjectInitStatement("var queueClient", $"new {UseType("Azure.Storage.Queues.QueueClient")}(endpoint, queueName);");

                            @foreach.AddIfStatement("createQueue", @if =>
                            {
                                @if.AddInvocationStatement("await queueClient.CreateIfNotExistsAsync", invoc =>
                                {
                                    invoc.AddArgument("cancellationToken: cancellationToken");
                                });
                            });

                            @foreach.AddIfStatement("! await queueClient.ExistsAsync(cancellationToken)", @if =>
                            {
                                @if.AddInvocationStatement("_logger.LogError", invoc =>
                                {
                                    invoc.AddArgument("\"Queue '{QueueName}' does not exist. Unable to publish.\"");
                                    invoc.AddArgument("queueName");
                                });

                                @if.AddStatement("continue;");
                            });

                            @foreach.AddForEachStatement("entry", "group", entry =>
                            {
                                if (ExecutionContext.Settings.GetAzureQueueStorageSettings().MessageEncoding().AsEnum() == AzureQueueStorageSettings.MessageEncodingOptionsEnum.Base64)
                                {
                                    AddUsing("System.Text");
                                    AddUsing("System");
                                    entry.AddObjectInitStatement("var bytes", "Encoding.UTF8.GetBytes(entry.Message);");

                                    entry.AddInvocationStatement("await queueClient.SendMessageAsync", invoc =>
                                    {
                                        invoc.AddArgument("Convert.ToBase64String(bytes)")
                                            .AddArgument("cancellationToken");
                                    });
                                }

                                if (ExecutionContext.Settings.GetAzureQueueStorageSettings().MessageEncoding().AsEnum() == AzureQueueStorageSettings.MessageEncodingOptionsEnum.None)
                                {
                                    entry.AddInvocationStatement("await queueClient.SendMessageAsync", invoc =>
                                    {
                                        invoc.AddArgument("entry.Message")
                                            .AddArgument("cancellationToken");
                                    });
                                }


                            });
                        });
                    });

                    @class.AddMethod("void", "Publish", mth =>
                    {
                        mth.AddGenericParameter("T", out var genT).AddGenericTypeConstraint(genT, gen => gen.AddType("class"));
                        mth.AddParameter(genT, "message");

                        mth.AddInvocationStatement("ValidateMessage", invoc => invoc.AddArgument("message"));
                        mth.AddObjectInitStatement("var jsonMessage", $"{UseType("System.Text.Json.JsonSerializer")}.Serialize(message, _serializerOptions);");

                        mth.AddInvocationStatement("_messageQueue.Add", invoc =>
                        {
                            invoc.AddArgument(new CSharpInvocationStatement("new MessageEntry")
                                   .AddArgument("typeof(T)")
                                   .AddArgument("jsonMessage").WithoutSemicolon());
                        });
                    });

                    @class.AddMethod("void", "Send", mth =>
                    {
                        mth.AddGenericParameter("T", out var T);
                        mth.AddGenericTypeConstraint(T, c => c.AddType("class"));
                        mth.AddParameter(T, "message");

                        mth.AddStatement("ValidateMessage(message);");
                        mth.AddObjectInitStatement("var jsonMessage", $"{UseType("System.Text.Json.JsonSerializer")}.Serialize(message, _serializerOptions);");

                        mth.AddInvocationStatement("_messageQueue.Add", invoc =>
                        {
                            invoc.AddArgument(new CSharpInvocationStatement("new MessageEntry")
                                   .AddArgument("typeof(T)")
                                   .AddArgument("jsonMessage").WithoutSemicolon());
                        });
                    });

                    @class.AddMethod("void", "ValidateMessage", mth =>
                    {
                        mth.AddParameter("object", "message");

                        mth.AddIfStatement("!_lookup.TryGetValue(message.GetType().FullName!, out _)", @if =>
                        {
                            @if.AddStatement($"throw new {UseType("System.Exception")}($\"The message type '{{message.GetType().FullName}}' is not registered.\");");
                        });
                    });

                }).AddRecord("MessageEntry", record =>
                {
                    record
                        .AddPrimaryConstructor(ctor =>
                        {
                            ctor.AddParameter("Type", "Type")
                                .AddParameter("string", "Message");
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