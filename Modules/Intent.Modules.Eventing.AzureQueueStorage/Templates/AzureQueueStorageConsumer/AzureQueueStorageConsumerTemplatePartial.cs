using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.AzureQueueStorage.Settings;
using Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageOptions;
using Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageSubscriptionOptions;
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
                    AddTemplateDependency(AzureQueueStorageOptionsTemplate.TemplateId);
                    AddTemplateDependency(AzureQueueStorageSubscriptionOptionsTemplate.TemplateId);

                    @class.ImplementsInterface(this.GetAzureQueueStorageConsumerInterfaceName());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"{UseType($"Microsoft.Extensions.Logging.ILogger<{@class.Name}>")}", "logger", @param => param.IntroduceReadonlyField())
                            .AddParameter($"{UseType("System.IServiceProvider")}", "serviceProvider", @param => param.IntroduceReadonlyField())
                            .AddParameter($"{UseType($"Microsoft.Extensions.Options.IOptions<{this.GetAzureQueueStorageOptionsName()}>")}", "options", @param => param.IntroduceReadonlyField())
                            .AddParameter($"{UseType($"Microsoft.Extensions.Options.IOptions<{this.GetAzureQueueStorageSubscriptionOptionsName()}>")}", "subscriptionOptions")
                            .AddParameter($"{UseType($"QueueDefinition")}", "queueDefinition", @param => param.IntroduceReadonlyField());

                        ctor.AddAssignmentStatement("_serializerOptions", new CSharpObjectInitializerBlock("new JsonSerializerOptions")
                            .WithSemicolon()
                            .AddInitStatement("PropertyNamingPolicy", "JsonNamingPolicy.CamelCase")
                            .AddInitStatement("PropertyNameCaseInsensitive", "true"));

                        AddUsing("System.Linq");
                        //_handlers = subscriptionOptions.Value.Entries.ToDictionary(k => k.MessageType.FullName!, v => v.HandlerAsync);
                        ctor.AddObjectInitStatement("_handlers", new CSharpInvocationStatement("subscriptionOptions.Value.Entries.ToDictionary")
                            .AddArgument("k => k.MessageType.FullName!").AddArgument("v => v.HandlerAsync"));
                    });

                    @class.AddField($"{UseType("System.Collections.Generic.Dictionary")}<string, DispatchHandler>", "_handlers", @field => field.PrivateReadOnly());
                    @class.AddField(UseType("System.Text.Json.JsonSerializerOptions"), "_serializerOptions", @field => field.PrivateReadOnly());

                    @class.AddMethod(UseType("System.Threading.Tasks.Task"), "ConsumeAsync", mth =>
                    {
                        mth.Async();
                        mth.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");

                        mth.AddInvocationStatement("_logger.LogInformation", invoc =>
                        {
                            invoc.AddArgument("$\"Subscribing to Queue: {_queueDefinition.QueueName}\"");
                        });

                        mth.AddTryBlock(@try =>
                        {
                            @try.AddObjectInitStatement("var endpoint", "!string.IsNullOrWhiteSpace(_queueDefinition.Endpoint) ? _queueDefinition.Endpoint : _options.Value.DefaultEndpoint;");

                            var newClientInvoc = new CSharpInvocationStatement($"new {UseType("Azure.Storage.Queues.QueueClient")}")
                                .AddArgument("_queueDefinition.Endpoint")
                                .AddArgument("_queueDefinition.QueueName");

                            @try.AddObjectInitStatement("var queueClient", newClientInvoc);

                            @try.AddIfStatement("_queueDefinition.CreateQueue", @if =>
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
                                            .AddArgument("_queueDefinition.QueueName");
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
                                        .AddArgument("maxMessages: _queueDefinition.MaxMessages"));

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
                                        @using.AddTryBlock(innerTry =>
                                        {
                                            if (ExecutionContext.Settings.GetAzureQueueStorageSettings().MessageEncoding().AsEnum() == AzureQueueStorageSettings.MessageEncodingOptionsEnum.None)
                                            {
                                                innerTry.AddObjectInitStatement("var envelope",
                                                    new CSharpInvocationStatement($"message.Body.ToObjectFromJson<{this.GetAzureQueueStorageEnvelopeName()}>").AddArgument("_serializerOptions"));
                                            }

                                            if (ExecutionContext.Settings.GetAzureQueueStorageSettings().MessageEncoding().AsEnum() == AzureQueueStorageSettings.MessageEncodingOptionsEnum.Base64)
                                            {
                                                AddUsing("System.Text");

                                                innerTry.AddObjectInitStatement("var base64Message", "message.Body.ToString();");
                                                innerTry.AddObjectInitStatement("var jsonMessage", "Encoding.UTF8.GetString(Convert.FromBase64String(base64Message));");

                                                innerTry.AddObjectInitStatement("var envelope", new CSharpInvocationStatement($"{UseType("System.Text.Json.JsonSerializer")}.Deserialize<{this.GetAzureQueueStorageEnvelopeName()}>")
                                                        .AddArgument("jsonMessage").AddArgument("_serializerOptions"));
                                            }

                                            innerTry.AddIfStatement("envelope is null", @if =>
                                            {
                                                @if.AddInvocationStatement("_logger.LogWarning", invoc =>
                                                {
                                                    invoc.AddArgument("\"Skipping message '{Id}'. Null deserialization\"")
                                                        .AddArgument("message.MessageId");
                                                })
                                                .AddStatement("continue;");
                                            });

                                            innerTry.AddObjectInitStatement("var messageTypeName", "envelope.MessageType;");

                                            innerTry.AddIfStatement("_handlers.TryGetValue(messageTypeName, out var handlerAsync)", @if =>
                                            {
                                                @if.AddInvocationStatement("await handlerAsync", invoc =>
                                                {
                                                    invoc.AddArgument("scope.ServiceProvider")
                                                        .AddArgument("envelope")
                                                        .AddArgument("_serializerOptions")
                                                        .AddArgument("cancellationToken");
                                                });

                                                @if.AddInvocationStatement("await queueClient.DeleteMessageAsync", invoc =>
                                                {
                                                    invoc.AddArgument("message.MessageId")
                                                        .AddArgument("message.PopReceipt")
                                                        .AddArgument("cancellationToken");
                                                });
                                            });
                                        }).AddCatchBlock("Exception", "handlerEx", @catch =>
                                        {
                                            @catch.AddStatement("_logger.LogError(handlerEx, \"Error dispatching message '{Id}' from queue '{Queue}'\", message.MessageId, _queueDefinition.QueueName);");
                                        });
                                    });
                                });
                            });

                        }).AddCatchBlock("Exception", "exception", @catch =>
                        {
                            @catch.AddStatement("_logger.LogError(exception, $\"Error consuming for {_queueDefinition.QueueName}\");");
                        });
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            var appStartup = ExecutionContext.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);

            return base.CanRunTemplate() && this.GetSubscribedMessageCount() > 0 && appStartup != null;
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