using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureEventGrid.Templates.AzureEventGridMessageDispatcher
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureEventGridMessageDispatcherTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.AzureEventGrid.AzureEventGridMessageDispatcher";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureEventGridMessageDispatcherTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.AzureMessagingEventGrid(outputTarget));
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Azure.Messaging")
                .AddUsing("Microsoft.Extensions.Options")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"AzureEventGridMessageDispatcher", @class =>
                {
                    @class.AddField("Dictionary<string, AzureEventGridDispatchHandler>", "_handlers", field => field.PrivateReadOnly());

                    @class.ImplementsInterface(this.GetAzureEventGridMessageDispatcherInterfaceName());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"IOptions<{this.GetAzureEventGridSubscriptionOptionsName()}>", "options");

                        ctor.AddStatement("_handlers = options.Value.Entries.ToDictionary(k => k.MessageType.FullName!, v => v.HandlerAsync);");
                    });

                    @class.AddMethod("async Task", "DispatchAsync", method =>
                    {
                        method.AddParameter("IServiceProvider", "scopedServiceProvider");
                        method.AddParameter("CloudEvent", "cloudEvent");
                        method.AddParameter("CancellationToken", "cancellationToken");

                        method.AddStatement("var messageTypeName = cloudEvent.Type;");
                        method.AddIfStatement("_handlers.TryGetValue(messageTypeName, out var handlerAsync)",
                            block => { block.AddStatement("await handlerAsync(scopedServiceProvider, cloudEvent, cancellationToken);"); });
                    });

                    @class.AddMethod("Task", "InvokeDispatchHandler", method =>
                    {
                        method.Async().Static();
                        method.AddGenericParameter("TMessage", out var tMessage);
                        method.AddGenericParameter("THandler", out var tHandler);
                        method.AddParameter("IServiceProvider", "serviceProvider");
                        method.AddParameter("CloudEvent", "cloudEvent");
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddGenericTypeConstraint(tMessage, c => c.AddType("class"));
                        method.AddGenericTypeConstraint(tHandler, c => c.AddType($"{this.GetIntegrationEventHandlerInterfaceName()}<{tMessage}>"));

                        method.AddStatement($"var pipeline = serviceProvider.GetRequiredService<{this.GetAzureEventGridConsumerPipelineName()}>();");
                        method.AddStatement(new CSharpAwaitExpression(new CSharpInvocationStatement("pipeline.ExecuteAsync")
                            .AddArgument("cloudEvent")
                            .AddArgument(new CSharpLambdaBlock("async (@event, token)")
                                .AddStatement($"var messageObj = @event.Data?.ToObjectFromJson<TMessage>()!;")
                                .AddStatement($"var handler = serviceProvider.GetRequiredService<{tHandler}>();")
                                .AddStatement("await handler.HandleAsync(messageObj, token);")
                                .AddReturn("@event")
                            )
                            .AddArgument("cancellationToken")
                        ));
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