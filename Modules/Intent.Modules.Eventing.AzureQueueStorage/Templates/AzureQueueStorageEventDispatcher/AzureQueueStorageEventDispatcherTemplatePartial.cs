using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.UnitOfWork.Shared;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageEventDispatcher
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureQueueStorageEventDispatcherTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.AzureQueueStorage.AzureQueueStorageEventDispatcher";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureQueueStorageEventDispatcherTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"AzureQueueStorageEventDispatcher", @class =>
                {
                    @class.ImplementsInterface($"{this.GetAzureQueueStorageEventDispatcherInterfaceName()}");

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"{UseType("Microsoft.Extensions.Options.IOptions")}<{this.GetAzureQueueStorageSubscriptionOptionsName()}>", "options");

                        AddUsing("System.Linq");
                        ctor.AddObjectInitStatement("_handlers", new CSharpInvocationStatement($"options.Value.Entries.ToDictionary")
                            .AddArgument("k => k.MessageType.FullName!").AddArgument("v => v.HandlerAsync"));
                    });

                    @class.AddField($"{UseType("System.Collections.Generic.Dictionary")}<string, DispatchHandler>", "_handlers", @field =>
                    {
                        field.PrivateReadOnly();
                    });

                    @class.AddMethod(UseType("System.Threading.Tasks.Task"), "DispatchAsync", method =>
                    {
                        method.Async();

                        method.AddParameter(UseType("System.IServiceProvider"), "serviceProvider");
                        method.AddParameter(this.GetAzureQueueStorageEnvelopeName(), "message");
                        method.AddParameter(UseType("System.Text.Json.JsonSerializerOptions"), "serializerOptions");
                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");

                        method.AddObjectInitStatement("var messageTypeName", "message.MessageType;");

                        method.AddIfStatement("_handlers.TryGetValue(messageTypeName, out var handlerAsync)", @if =>
                        {
                            @if.AddInvocationStatement("await handlerAsync", invoc =>
                            {
                                invoc.AddArgument("serviceProvider")
                                    .AddArgument("message")
                                    .AddArgument("serializerOptions")
                                    .AddArgument("cancellationToken");
                            });
                        });
                    });

                    @class.AddMethod(UseType("System.Threading.Tasks.Task"), "InvokeDispatchHandler", method =>
                    {
                        method.Static().Async();

                        method.AddGenericParameter("TMessage", out var TMessage).AddGenericTypeConstraint(TMessage, gen => gen.AddType("class"));
                        method.AddGenericParameter("THandler", out var THandler).AddGenericTypeConstraint(THandler, gen => gen.AddType($"{this.GetIntegrationEventHandlerInterfaceName()}<TMessage>"));

                        method.AddParameter(UseType("System.IServiceProvider"), "serviceProvider");
                        method.AddParameter(this.GetAzureQueueStorageEnvelopeName(), "message");
                        method.AddParameter(UseType("System.Text.Json.JsonSerializerOptions"), "serializerOptions");
                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");

                        AddUsing("Microsoft.Extensions.DependencyInjection");

                        method.AddObjectInitStatement("var messageObj", "((JsonElement)message.Payload).Deserialize<TMessage>(serializerOptions);");
                        method.AddObjectInitStatement("var handler", new CSharpInvocationStatement("serviceProvider.GetRequiredService<THandler>"));
                        method.AddInvocationStatement("await handler.HandleAsync", invoc => invoc.AddArgument("messageObj").AddArgument("cancellationToken"));
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