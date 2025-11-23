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

namespace Intent.Modules.Eventing.Contracts.Templates.CompositeMessageBus
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CompositeMessageBusTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Contracts.CompositeMessageBus";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CompositeMessageBusTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass("CompositeMessageBus", @class =>
                {
                    @class.ImplementsInterface(this.GetBusInterfaceName());
                    @class.AddField(this.GetMessageBrokerResolverName(), "_resolver", field => field.PrivateReadOnly());
                    @class.AddField($"HashSet<{this.GetBusInterfaceName()}>", "_messageBusProviders", field => 
                        field.PrivateReadOnly().WithAssignment(new CSharpStatement("[]")));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(this.GetMessageBrokerResolverName(), "resolver");
                        ctor.AddStatement("_resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));");
                    });

                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("TMessage", out var tMessage);
                        method.AddGenericTypeConstraint(tMessage, c => c.AddType("class"));
                        method.AddParameter(tMessage, "message");
                        method.AddStatement("ValidateMessageType<TMessage>();");
                        method.AddStatement("InnerDispatch<TMessage>(provider => provider.Publish(message));");
                    });

                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("TMessage", out var tMessage);
                        method.AddGenericTypeConstraint(tMessage, c => c.AddType("class"));
                        method.AddParameter(tMessage, "message");
                        method.AddParameter("IDictionary<string, object>", "additionalData");
                        method.AddStatement("ValidateMessageType<TMessage>();");
                        method.AddStatement("InnerDispatch<TMessage>(provider => provider.Publish(message, additionalData));");
                    });

                    @class.AddMethod("void", "Send", method =>
                    {
                        method.AddGenericParameter("TMessage", out var tMessage);
                        method.AddGenericTypeConstraint(tMessage, c => c.AddType("class"));
                        method.AddParameter(tMessage, "message");
                        method.AddStatement("ValidateMessageType<TMessage>();");
                        method.AddStatement("InnerDispatch<TMessage>(provider => provider.Send(message));");
                    });

                    @class.AddMethod("void", "Send", method =>
                    {
                        method.AddGenericParameter("TMessage", out var tMessage);
                        method.AddGenericTypeConstraint(tMessage, c => c.AddType("class"));
                        method.AddParameter(tMessage, "message");
                        method.AddParameter("IDictionary<string, object>", "additionalData");
                        method.AddStatement("ValidateMessageType<TMessage>();");
                        method.AddStatement("InnerDispatch<TMessage>(provider => provider.Send(message, additionalData));");
                    });

                    @class.AddMethod("Task", "FlushAllAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        
                        method.AddIfStatement("_messageBusProviders.Count == 0", stmt =>
                        {
                            stmt.AddStatement("return;");
                        });

                        method.AddStatement(new CSharpAwaitExpression(new CSharpInvocationStatement("Task.WhenAll")
                            .AddArgument("_messageBusProviders.Select(provider => provider.FlushAllAsync(cancellationToken))")), stmt => stmt.SeparatedFromPrevious());
                        method.AddStatement("_messageBusProviders.Clear();");
                    });

                    @class.AddMethod("void", "InnerDispatch", method =>
                    {
                        method.Private();
                        method.AddGenericParameter("T");
                        method.AddParameter($"Action<{this.GetBusInterfaceName()}>", "action");
                        
                        method.AddStatement("var providers = _resolver.GetMessageBusProvidersForMessageType(typeof(T));");
                        method.AddForEachStatement("provider", "providers", loop =>
                        {
                            loop.AddStatement("_messageBusProviders.Add(provider);");
                            loop.AddStatement("action(provider);");
                        });
                    });

                    @class.AddMethod("void", "ValidateMessageType", method =>
                    {
                        method.Private();
                        method.AddGenericParameter("TMessage");
                        
                        method.AddStatement("var messageType = typeof(TMessage);");
                        method.AddIfStatement("!_resolver.IsMessageTypeRegistered(messageType)", stmt =>
                        {
                            stmt.AddStatement(@"throw new InvalidOperationException(
                    $""Message type '{messageType.FullName}' is not registered with any message broker provider. "" +
                    $""Ensure the message is configured in the appropriate provider's configuration (e.g., AzureEventGridPublisherOptions)."");");
                        });
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return this.RequiresCompositeMessageBus();
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
