using System;
using System.Collections.Generic;
using System.Xml;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageConsumer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureQueueStorageConsumerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.AzureQueueStorage.AzureQueueStorageConsumer";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureQueueStorageConsumerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"AzureQueueStorageConsumer", @class =>
                {
                    AddNugetDependency(NugetPackages.AzureStorageQueues(outputTarget));

                    @class.AddGenericParameter("T")
                        .AddGenericTypeConstraint("T", con => con.AddType("class"));
                    @class.ImplementsInterface(this.GetAzureQueueStorageConsumerInterfaceName());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"{UseType($"Microsoft.Extensions.Logging.ILogger<{@class.Name}<T>>")}", "logger", @param => param.IntroduceReadonlyField())
                            .AddParameter($"{UseType($"Microsoft.Extensions.Options.IOptions<{this.GetAzureQueueStorageOptionsName()}>")}", "options")
                            .AddParameter($"IServiceProvider", "serviceProvider", @param => param.IntroduceReadonlyField());

                        ctor.AddAssignmentStatement("_serializerOptions", new CSharpObjectInitializerBlock("new JsonSerializerOptions")
                            .WithSemicolon()
                            .AddInitStatement("PropertyNamingPolicy", "JsonNamingPolicy.CamelCase")
                            .AddInitStatement("PropertyNameCaseInsensitive", "true"));

                        ctor.AddIfStatement("!options.Value.Entries.Any(e => e.MessageType.FullName == typeof(T).FullName!)", @if =>
                        {
                            @if.AddStatement("throw new Exception($\"The message type '{typeof(T).FullName}' is not registered.\");");
                        });

                        var firstInvoc = new CSharpInvocationStatement("options.Value.Entries.First")
                            .AddArgument(new CSharpLambdaBlock("e")
                            .WithExpressionBody("e.MessageType.FullName == typeof(T).FullName!"));

                        ctor.AddObjectInitStatement("_messageOptions", firstInvoc);
                    });

                    @class.AddField("QueueStorageEntry", "_messageOptions", @field => field.PrivateReadOnly());
                    @class.AddField(UseType("System.Text.Json.JsonSerializerOptions"), "_serializerOptions", @field => field.PrivateReadOnly());

                    @class.AddMethod(UseType("System.Threading.Tasks.Task"), "ConsumeAsync", mth =>
                    {
                        mth.Async();
                        mth.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");

                        mth.AddInvocationStatement("_logger.LogInformation", invoc =>
                        {
                            invoc.AddArgument("$\"Subscribing to Queue: {_messageOptions.QueueName}\"");
                        });

                        mth.AddTryBlock(@try =>
                        {
                            var newClientInvoc = new CSharpInvocationStatement($"new {UseType("Azure.Storage.Queues.QueueClient")}")
                                .AddArgument("_messageOptions.Endpoint")
                                .AddArgument("_messageOptions.QueueName");

                            @try.AddObjectInitStatement("var queueClient", newClientInvoc);

                            @try.AddIfStatement("_messageOptions.CreateQueue", @if =>
                            {
                                @if.AddInvocationStatement("await queueClient.CreateIfNotExistsAsync", invoc =>
                                {
                                    invoc.AddArgument("cancellationToken: cancellationToken");
                                });
                            });

                            @try.AddWhileStatement("!cancellationToken.IsCancellationRequested", @while =>
                            {
                                @while.AddIfStatement("!await queueClient.ExistsAsync(cancellationToken)", @if =>
                                {
                                    @if.AddInvocationStatement("_logger.LogError", invoc =>
                                    {
                                        invoc.AddArgument("\"Queue '{QueueName}' does not exist. Unable to consume.\"")
                                            .AddArgument("_messageOptions.QueueName");
                                    })
                                    .AddInvocationStatement("await Task.Delay", invoc =>
                                    {
                                        invoc.AddArgument("500")
                                            .AddArgument("cancellationToken");
                                    })
                                    .AddStatement("continue;");
                                });

                                @while.AddObjectInitStatement($"{UseType("Azure.Storage.Queues.Models.QueueProperties")} properties",
                                    new CSharpInvocationStatement("await queueClient.GetPropertiesAsync").AddArgument("cancellationToken"));

                                @while.AddIfStatement("properties.ApproximateMessagesCountLong <= 0", @if =>
                                {
                                    @if.AddInvocationStatement("await Task.Delay", invoc =>
                                    {
                                        invoc.AddArgument("500")
                                            .AddArgument("cancellationToken");
                                    })
                                    .AddStatement("continue;");
                                });

                                @while.AddObjectInitStatement($"{UseType("Azure.Storage.Queues.Models.QueueMessage")}[] messages",
                                    new CSharpInvocationStatement("await queueClient.ReceiveMessagesAsync")
                                        .AddArgument("maxMessages: _messageOptions.MaxMessages"));

                                @while.AddIfStatement("messages.Length == 0", @if =>
                                {
                                    @if.AddInvocationStatement("await Task.Delay", invoc =>
                                    {
                                        invoc.AddArgument("500")
                                            .AddArgument("cancellationToken");
                                    })
                                    .AddStatement("continue;");
                                });

                                AddUsing("Microsoft.Extensions.DependencyInjection");
                                @while.AddForEachStatement("message", "messages", @foreach =>
                                {
                                    @foreach.AddUsingBlock("var scope = _serviceProvider.CreateScope()", @using =>
                                    {
                                        @using.AddObjectInitStatement("var dispatcher",
                                            new CSharpInvocationStatement($"scope.ServiceProvider.GetRequiredService<{this.GetAzureQueueStorageEventDispatcherInterfaceName()}<T>>"));
                                        @using.AddObjectInitStatement("var deSerializedMessage",
                                            new CSharpInvocationStatement($"message.Body.ToObjectFromJson<T>").AddArgument("_serializerOptions"));

                                        @using.AddIfStatement("deSerializedMessage is null", @if =>
                                        {
                                            @if.AddInvocationStatement("_logger.LogWarning", invoc =>
                                            {
                                                invoc.AddArgument("\"Skipping message '{Id}'. Null deserialization\"")
                                                    .AddArgument("message.MessageId");
                                            })
                                            .AddStatement("continue;");
                                        });

                                        @using.AddTryBlock(innerTry =>
                                        {
                                            innerTry.AddInvocationStatement("await dispatcher.Dispatch", invoc =>
                                            {
                                                invoc.AddArgument("deSerializedMessage")
                                                    .AddArgument("cancellationToken");
                                            });

                                            innerTry.AddInvocationStatement("await queueClient.DeleteMessageAsync", invoc =>
                                            {
                                                invoc.AddArgument("message.MessageId")
                                                    .AddArgument("message.PopReceipt")
                                                    .AddArgument("cancellationToken");
                                            });

                                        }).AddCatchBlock("Exception", "handlerEx", @catch =>
                                        {
                                            @catch.AddStatement("_logger.LogError(handlerEx, \"Error dispatching message '{Id}' from queue '{Queue}'\", message.MessageId, _messageOptions.QueueName);");
                                        });
                                    });
                                });
                            });

                        }).AddCatchBlock("Exception", "exception", @catch =>
                        {
                            @catch.AddStatement("_logger.LogError(exception, $\"Error consuming for {_messageOptions.MessageType.FullName}\");");
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
    }
}