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

namespace Intent.Modules.Eventing.Solace.Templates.SolaceEventBus
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class SolaceEventBusTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Solace.SolaceEventBus";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SolaceEventBusTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("SolaceSystems.Solclient.Messaging")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddClass($"SolaceEventBus", @class =>
                {
                    @class.ImplementsInterface(this.GetEventBusInterfaceName());
                    @class.AddField("List<object>", "_messagesToPublish", f => f.PrivateReadOnly().WithAssignment("new List<object>()"));
                    @class.AddField("List<object>", "_messagesToSend", f => f.PrivateReadOnly().WithAssignment("new List<object>()"));
                    @class.AddField("int?", "_defaultPriority", f => f.PrivateReadOnly());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("ISession", "session", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("MessageRegistry", "messageRegistry", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("MessageSerializer", "messageSerializer", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("IConfiguration", "configuration");
                        ctor.AddStatement("_defaultPriority = configuration.GetSection(\"Solace:DefaultSendPriority\").Get<int?>();");
                    });

                    @class.AddMethod("void", "Publish", method =>
                    {
                        method
                            .AddGenericParameter("TMessage", out var tMessage)
                            .AddParameter(tMessage, "message")
                            .AddGenericTypeConstraint(tMessage, c => c.AddType("class"));
                        method.AddStatement("_messagesToPublish.Add(message);");
                    });
                    @class.AddMethod("void", "Send", method =>
                    {
                        method
                            .AddGenericParameter("TMessage", out var tMessage)
                            .AddParameter(tMessage, "message")
                            .AddGenericTypeConstraint(tMessage, c => c.AddType("class"));
                        method.AddStatement("_messagesToSend.Add(message);");
                    });
                    @class.AddMethod("Task", "FlushAllAsync", method =>
                    {
                        method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                        method.AddStatements(@"foreach (var toPublish in _messagesToPublish)
			{
				PublishEvent(_session, toPublish);
			}

			_messagesToPublish.Clear();
			foreach (var toSend in _messagesToSend)
			{
				SendCommand(_session, toSend);
			}
			_messagesToSend.Clear();
			return Task.CompletedTask;".ConvertToStatements());
                    });

                    @class.AddMethod("void", "PublishEvent", method =>
                    {
                        method
                            .Private()
                            .AddParameter("ISession", "session")
                            .AddParameter("object", "toPublish");
                        method.AddStatements(@"using (var message = ContextFactory.Instance.CreateMessage())
			{
                var messageConfig = _messageRegistry.PublishedMessages[toPublish.GetType()];

                message.Destination = ContextFactory.Instance.CreateTopic(messageConfig.PublishTo);
				message.BinaryAttachment = _messageSerializer.SerializeMessage(toPublish);
				message.Priority = GetPriority(messageConfig.Priority);

				var returnCode = session.Send(message);
				if (returnCode != ReturnCode.SOLCLIENT_OK)
				{
					throw new Exception($""Unable to publish message so Solace ({toPublish.GetType().Name}) : {returnCode}"");
				}
			}".ConvertToStatements());

                    });
                    @class.AddMethod("void", "SendCommand", method =>
                    {
                        method
                            .Private()
                            .AddParameter("ISession", "session")
                            .AddParameter("object", "toSend");
                        method.AddStatements(@"using (var message = ContextFactory.Instance.CreateMessage())
			{
                var messageConfig = _messageRegistry.PublishedMessages[toSend.GetType()];

                message.Destination = ContextFactory.Instance.CreateQueue(messageConfig.PublishTo);
				message.BinaryAttachment = _messageSerializer.SerializeMessage(toSend);
				message.Priority = GetPriority(messageConfig.Priority);

				var returnCode = session.Send(message);
				if (returnCode != ReturnCode.SOLCLIENT_OK)
				{
					throw new Exception($""Unable to send command so Solace ({toSend.GetType().Name}) : {returnCode}"");
				}
			}".ConvertToStatements());
                    });
                    @class.AddMethod("int?", "GetPriority", method =>
                    {
                        method
                            .Private()
                            .AddParameter("int?", "messagePriority");
                        method.AddStatements(@"int? result = null;
            if (messagePriority != null)
            {
                result = messagePriority.Value;
            }
            else
            {
                result = _defaultPriority;
            }
            if (result != null)
            {
                result = Math.Clamp(result.Value, 0, 255);
            }
            return result;".ConvertToStatements());
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