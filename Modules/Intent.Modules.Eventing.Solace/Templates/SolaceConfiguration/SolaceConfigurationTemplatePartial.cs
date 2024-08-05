using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Solace.Templates.SolaceConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class SolaceConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Solace.SolaceConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SolaceConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.SolaceSystemsSolclientMessaging(OutputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("SolaceSystems.Solclient.Messaging")
                .AddClass($"SolaceConfiguration", @class =>
                {
                    @class.Static();

                    @class.AddMethod("void", "AddSolaceConfiguration", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", p => p.WithThisModifier());
                        method.AddParameter("IConfiguration", "configuration");

                        method.AddStatements(@"var config = configuration.GetSection(""Solace"").Get<SolaceConfig>();
			if (config == null)
			{
				throw new Exception(""Unable to load / find Solace configuration in appsettings.json"");
			}
			else
			{
				config.Validate();
			}

			// Initialize Solace Systems Messaging API with logging to console at Warning level
			var cfp = new ContextFactoryProperties()
			{
				SolClientLogLevel = SolLogLevel.Warning
			};
			ContextFactory.Instance.Init(cfp);
			var context = ContextFactory.Instance.CreateContext(new ContextProperties(), null);

			// Create session properties
			var sessionProps = new SessionProperties()
			{
				Host = config.Host,
				VPNName = config.VPNName,
				UserName = config.UserName,
				Password = config.Password,
				ReconnectRetries = config.ReconnectRetries ?? -1,
				GdWithWebTransport = config.GdWithWebTransport ?? true,
				SSLValidateCertificate = config.SSLValidateCertificate ?? false,
				ProvisionTimeoutInMsecs = config.ProvisionTimeoutInMsecs ?? 10000
			};

			var session = context.CreateSession(sessionProps, HandleSessionMessageEvent, HandleSessionEvent);

".ConvertToStatements());

                        method.AddStatement("services.AddSingleton(context);");
                        method.AddStatement("services.AddSingleton(session);");
                        method.AddStatement($"services.AddSingleton<{this.GetDispatchResolverName()}>();");
                        method.AddStatement($"services.AddSingleton<{this.GetMessageSerializerName()}>();");
                        method.AddStatement($"services.AddSingleton<{this.GetMessageRegistryName()}>();");
                        method.AddStatement($"services.AddHostedService<{this.GetSolaceConsumingServiceName()}>();");
                        method.AddStatement($"services.AddScoped(typeof({this.GetSolaceEventDispatcherInterfaceName()}<>),typeof({this.GetSolaceEventDispatcherName()}<>) );");
                        method.AddStatement($"services.AddScoped<{this.GetEventBusInterfaceName()},{this.GetSolaceEventBusName()}>();");
                        method.AddStatement($"services.AddTransient<{this.GetSolaceConsumerName()}>();");

                        @class.AddMethod("void", "HandleSessionMessageEvent", method =>
                        {
                            method.Private().Static();
                            method.AddParameter("object?", "sender");
                            method.AddParameter("MessageEventArgs", "e");
                            method.AddStatement("// Handle incoming messages if necessary");
                        });

                        @class.AddMethod("void", "HandleSessionEvent", method =>
                        {
                            method.Private().Static();
                            method.AddParameter("object?", "sender");
                            method.AddParameter("SessionEventArgs", "e");
                            method.AddStatement("// Handle session events if necessary");
                        });
                    });

                    @class.AddNestedClass("SolaceConfig", config =>
                    {
                        config.AddProperty("string?", "Host");
                        config.AddProperty("string?", "VPNName");
                        config.AddProperty("string?", "UserName");
                        config.AddProperty("string?", "Password");
                        config.AddProperty("int?", "ReconnectRetries");
                        config.AddProperty("bool?", "GdWithWebTransport");
                        config.AddProperty("bool?", "SSLValidateCertificate");
                        config.AddProperty("int?", "ProvisionTimeoutInMsecs");
                        config.AddProperty("string?", "EnvironmentPrefix");
                        config.AddProperty("string?", "Application");

                        config.AddMethod("void", "Validate", method =>
                        {
                            method.AddStatements(@"if (Host == null)
				throw new Exception(""Solace Host not configured"");
			if (VPNName == null)
				throw new Exception(""Solace VPN Name not configured"");
			if (UserName == null)
				throw new Exception(""Solace UserName not configured"");
			if (Password == null)
				throw new Exception(""Solace Password not configured"");".ConvertToStatements());
                        });
                    });
                });
        }

        public override void AfterTemplateRegistration()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("AddSolaceConfiguration", ServiceConfigurationRequest.ParameterType.Configuration)
                .ForConcern("Infrastructure")
                .HasDependency(this));

            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest($"Solace", new
            {
                Host = "tcp://localhost:55555",
                VPNName = "default",
                UserName = "admin",
                Password = "admin",
                Application = OutputTarget.ApplicationName().Replace('.', '/'),
            }));
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