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

namespace Intent.Modules.Eventing.Solace.Templates.SolaceConsumingService
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class SolaceConsumingServiceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Solace.SolaceConsumingService";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SolaceConsumingServiceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftExtensionsHosting(OutputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Collections.Generic")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("SolaceSystems.Solclient.Messaging")
                .AddClass($"SolaceConsumingService", @class =>
                {
                    @class.ImplementsInterface("IHostedService");
                    @class.AddField($"List<{this.GetSolaceConsumerName()}>", "_consumers", f => f.PrivateReadOnly().WithAssignment($"new List<{this.GetSolaceConsumerName()}>()"));
                    @class.AddField($"bool", "_bindQueues", f => f.PrivateReadOnly());
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
                        ctor.AddParameter("IConfiguration", "configuration");
                        ctor.AddParameter("IServiceProvider", "provider", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddStatement("_bindQueues = configuration.GetSection(\"Solace:BindQueues\").Get<bool?>() ?? true;");
                        ctor.AddStatement("ValidateEnvironment(session);");
                    });

                    @class.AddMethod("Task", "StartAsync", method =>
                    {
                        method
                            .AddParameter("CancellationToken", "cancellationToken");
                        method.AddStatements(@"if (_bindQueues)
            {
                foreach (var queueConfig in _messageRegistry.Queues)
                {
                    var consumer = _provider.GetRequiredService<SolaceConsumer>();
                    consumer.Start(queueConfig);
                    _consumers.Add(consumer);
                }
            }
            return Task.CompletedTask;".ConvertToStatements());
                    });

                    @class.AddMethod("Task", "StopAsync", method =>
                    {
                        method
                            .AddParameter("CancellationToken", "cancellationToken");
                        method.AddStatements(@"foreach (var consumer in _consumers)
			{
				consumer.Stop();
			}
			return Task.CompletedTask;".ConvertToStatements());
                    });


                    @class.AddMethod("void", "ValidateEnvironment", method =>
                    {
                        method
                            .Private()
                            .AddParameter("ISession", "session");
                        method.AddStatements(@"var returnCode = session.Connect();
			if (returnCode != ReturnCode.SOLCLIENT_OK)
			{
				throw new InvalidOperationException(""Error connecting to Solace, return code: {0}\"", returnCode"");
			}
			if (!session.IsCapable(CapabilityType.PUB_GUARANTEED))
			{
				throw new InvalidOperationException(""Cannot proceed because session's PUB_GUARANTEED capability is not supported."");
			}
			if (!session.IsCapable(CapabilityType.SUB_FLOW_GUARANTEED))
			{
				throw new InvalidOperationException(""Cannot proceed because session's SUB_FLOW_GUARANTEED capability is not supported."");
			}
			if (!session.IsCapable(CapabilityType.ENDPOINT_MANAGEMENT))
			{
				throw new InvalidOperationException(""Cannot proceed because session's ENDPOINT_MANAGEMENT capability is not supported."");
			}
			if (!session.IsCapable(CapabilityType.QUEUE_SUBSCRIPTIONS))
			{
				throw new InvalidOperationException(""Cannot proceed because session's QUEUE_SUBSCRIPTIONS capability is not supported."");
			}".ConvertToStatements());
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