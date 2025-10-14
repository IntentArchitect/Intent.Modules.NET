using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Aws.Sqs.Templates.SqsMessageDispatcherInterface;
using Intent.Modules.Aws.Sqs.Templates.SqsSubscriptionOptions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Aws.Sqs.Templates.SqsMessageDispatcher
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class SqsMessageDispatcherTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Aws.Sqs.SqsMessageDispatcher";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SqsMessageDispatcherTemplate(IOutputTarget outputTarget, object? model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.AmazonLambdaSqsEvents(OutputTarget));
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Text.Json")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Amazon.Lambda.SQSEvents")
                .AddUsing("Microsoft.Extensions.Options")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"SqsMessageDispatcher", @class =>
                {
                    @class.AddField("Dictionary<string, DispatchHandler>", "_handlers", field => field.PrivateReadOnly());

                    @class.ImplementsInterface(this.GetTypeName(SqsMessageDispatcherInterfaceTemplate.TemplateId));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"IOptions<{this.GetTypeName(SqsSubscriptionOptionsTemplate.TemplateId)}>", "options");

                        ctor.AddStatement("_handlers = options.Value.Entries.ToDictionary(k => k.MessageType.FullName!, v => v.HandlerAsync);");
                    });

                    @class.AddMethod("async Task", "DispatchAsync", method =>
                    {
                        method.AddParameter("IServiceProvider", "scopedServiceProvider");
                        method.AddParameter("SQSEvent.SQSMessage", "sqsMessage");
                        method.AddParameter("CancellationToken", "cancellationToken");

                        method.AddStatement("""
                            var messageTypeName = sqsMessage.MessageAttributes
                                .TryGetValue("MessageType", out var messageTypeAttr)
                                ? messageTypeAttr.StringValue
                                : throw new Exception("MessageType attribute is missing");
                            """, stmt => stmt.SeparatedFromPrevious());
                        
                        method.AddIfStatement("_handlers.TryGetValue(messageTypeName, out var handlerAsync)",
                            block => { block.AddStatement("await handlerAsync(scopedServiceProvider, sqsMessage, cancellationToken);"); });
                    });

                    @class.AddMethod("Task", "InvokeDispatchHandler", method =>
                    {
                        method.Async().Static();
                        method.AddGenericParameter("TMessage", out var tMessage);
                        method.AddGenericParameter("THandler", out var tHandler);
                        method.AddParameter("IServiceProvider", "serviceProvider");
                        method.AddParameter("SQSEvent.SQSMessage", "sqsMessage");
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddGenericTypeConstraint(tMessage, c => c.AddType("class"));
                        method.AddGenericTypeConstraint(tHandler, c => c.AddType($"{this.GetIntegrationEventHandlerInterfaceName()}<{tMessage}>"));

                        method.AddStatement($"var messageObj = JsonSerializer.Deserialize<{tMessage}>(sqsMessage.Body)!;");
                        method.AddStatement($"var handler = serviceProvider.GetRequiredService<{tHandler}>();");
                        method.AddStatement("await handler.HandleAsync(messageObj, cancellationToken);");
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
