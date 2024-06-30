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

namespace Intent.Modules.Eventing.Solace.Templates.DispatchResolver
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DispatchResolverTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Solace.DispatchResolver";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DispatchResolverTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"DispatchResolver", @class =>
                {
                    @class.AddField("Dictionary<Type, Type>", "_dispatcherMappings", p => p.PrivateReadOnly());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"{this.GetMessageRegistryName()}", "messageRegistry");
                        ctor.AddStatements(@"_dispatcherMappings = new Dictionary<Type, Type>();
			foreach (var queue in messageRegistry.Queues)
			{
				foreach (var message in queue.SubscribedMessages)
				{
					var genericTypeDefinition = typeof(ISolaceEventDispatcher<>);
					var closedConstructedType = genericTypeDefinition.MakeGenericType(message.MessageType);
					_dispatcherMappings.Add(message.MessageType, closedConstructedType);
				}
			}".ConvertToStatements());
                    });

                    @class.AddMethod("object", "ResolveDispatcher", method =>
                    {
                        method.AddParameter("Type", "messageType");
                        method.AddParameter("IServiceProvider", "scopedProvider");
                        method.AddStatements(@"if (_dispatcherMappings.TryGetValue(messageType, out var dispatcherType))
			{
				return scopedProvider.GetRequiredService(dispatcherType);
			}

			throw new InvalidOperationException($""Dispatcher for type {messageType.Name} not found"");".ConvertToStatements());
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