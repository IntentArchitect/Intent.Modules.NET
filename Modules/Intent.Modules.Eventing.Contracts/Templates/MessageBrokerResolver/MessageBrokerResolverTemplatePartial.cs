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

namespace Intent.Modules.Eventing.Contracts.Templates.MessageBrokerResolver
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MessageBrokerResolverTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Contracts.MessageBrokerResolver";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MessageBrokerResolverTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass("MessageBrokerResolver", @class =>
                {
                    @class.AddField("IServiceProvider", "_serviceProvider", field => field.PrivateReadOnly());
                    @class.AddField(this.GetMessageBrokerRegistryName(), "_registry", field => field.PrivateReadOnly());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IServiceProvider", "serviceProvider");
                        ctor.AddParameter(this.GetMessageBrokerRegistryName(), "registry");
                        ctor.AddStatement("_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));");
                        ctor.AddStatement("_registry = registry ?? throw new ArgumentNullException(nameof(registry));");
                    });

                    @class.AddMethod("bool", "IsMessageTypeRegistered", method =>
                    {
                        method.AddParameter("Type", "messageType");
                        method.AddStatement("return _registry.GetAllRegistrations().ContainsKey(messageType);");
                    });

                    @class.AddMethod($"IReadOnlyList<{this.GetMessageBusInterfaceName()}>", "GetMessageBusProvidersForMessageType", method =>
                    {
                        method.AddParameter("Type", "messageType");
                        
                        method.AddIfStatement("!_registry.GetAllRegistrations().TryGetValue(messageType, out var providerTypes)", stmt =>
                        {
                            stmt.AddStatement("return [];");
                        });

                        method.AddStatement($@"return providerTypes
                .Select(pt => ({this.GetMessageBusInterfaceName()})_serviceProvider.GetRequiredService(pt))
                .ToList();", stmt => stmt.SeparatedFromPrevious());
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
