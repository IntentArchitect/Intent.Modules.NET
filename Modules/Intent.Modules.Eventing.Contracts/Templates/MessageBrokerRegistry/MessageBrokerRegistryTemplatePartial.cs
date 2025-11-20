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

namespace Intent.Modules.Eventing.Contracts.Templates.MessageBrokerRegistry
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MessageBrokerRegistryTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Contracts.MessageBrokerRegistry";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MessageBrokerRegistryTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddClass("MessageBrokerRegistry", @class =>
                {
                    @class.AddField("Dictionary<Type, List<Type>>", "_messageTypeToProviderTypes", field => 
                        field.PrivateReadOnly().WithAssignment(new CSharpStatement("new()")));
                    @class.AddField("IReadOnlyDictionary<Type, IReadOnlyList<Type>>?", "_readOnlyCache", field => field.Private());

                    @class.AddMethod("void", "Register", method =>
                    {
                        method.AddGenericParameter("TMessage", out var tMessage);
                        method.AddGenericParameter("TMessageBus", out var tMessageBus);
                        method.AddGenericTypeConstraint(tMessage, c => c.AddType("class"));
                        method.AddGenericTypeConstraint(tMessageBus, c => c.AddType(this.GetMessageBusInterfaceName()));

                        method.AddIfStatement($"typeof(TMessageBus) == typeof({this.GetCompositeMessageBusName()})", stmt =>
                        {
                            stmt.AddStatement(@"throw new InvalidOperationException(""Cannot register CompositeMessageBus as a message bus provider."");");
                        });
                        method.AddStatement("Register(typeof(TMessage), typeof(TMessageBus));");
                    });

                    @class.AddMethod("void", "Register", method =>
                    {
                        method.Private();
                        method.AddParameter("Type", "messageType");
                        method.AddParameter("Type", "providerType");

                        method.AddIfStatement("!_messageTypeToProviderTypes.TryGetValue(messageType, out var providerTypes)", stmt =>
                        {
                            stmt.AddStatement("providerTypes = new List<Type>();");
                            stmt.AddStatement("_messageTypeToProviderTypes[messageType] = providerTypes;");
                        });

                        method.AddStatement("", stmt => stmt.SeparatedFromPrevious());
                        method.AddIfStatement("!providerTypes.Contains(providerType)", stmt =>
                        {
                            stmt.AddStatement("providerTypes.Add(providerType);");
                        });

                        method.AddStatement("// Invalidate cache when new registrations are made", stmt => stmt.SeparatedFromPrevious());
                        method.AddStatement("_readOnlyCache = null;");
                    });

                    @class.AddMethod("IReadOnlyDictionary<Type, IReadOnlyList<Type>>", "GetAllRegistrations", method =>
                    {
                        method.AddStatement("EnsureReadOnlyCache();");
                        method.AddStatement("return _readOnlyCache!;");
                    });

                    @class.AddMethod("void", "EnsureReadOnlyCache", method =>
                    {
                        method.Private();
                        method.AddStatement(@"_readOnlyCache ??= _messageTypeToProviderTypes.ToDictionary(
                kvp => kvp.Key,
                kvp => (IReadOnlyList<Type>)kvp.Value.AsReadOnly());");
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
