using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing.NServiceBus.Api;
using Intent.Exceptions;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationCommand;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.Modules.Eventing.NServiceBus.Settings;
using Intent.Modules.Eventing.NServiceBus.Templates.NServiceBusMessageBus;
using Intent.Modules.Eventing.NServiceBus.Templates.NServiceBusMessageHandler;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.Eventing.NServiceBus.Templates.Constants;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.NServiceBus.Templates.NServiceBusConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class NServiceBusConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.NServiceBus.NServiceBusConfiguration";

        private readonly bool _isLegacyFramework;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public NServiceBusConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            _isLegacyFramework = OutputTarget.GetMaxNetAppVersion().Major < 10;

            FulfillsRole("Infrastructure.DependencyInjection.NServiceBus");
            FulfillsRole(TemplateRoles.Application.Eventing.MessageBusConfiguration);

            AddTypeSource(IntegrationEventMessageTemplate.TemplateId);
            AddTypeSource(IntegrationCommandTemplate.TemplateId);
            AddTypeSource(NServiceBusMessageHandlerTemplate.TemplateId);

            AddNugetDependency(NugetPackages.NServiceBus(OutputTarget));
            AddNugetDependency(NugetPackages.NServiceBusExtensionsHosting(OutputTarget));

            var transport = ExecutionContext.Settings.GetNServiceBusSettings().Transport();
            var outboxPattern = ExecutionContext.Settings.GetNServiceBusSettings().OutboxPattern();
            switch (transport.AsEnum())
            {
                case NServiceBusSettings.TransportOptionsEnum.Rabbitmq:
                    AddNugetDependency(NugetPackages.NServiceBusRabbitMQ(OutputTarget));
                    break;
                case NServiceBusSettings.TransportOptionsEnum.AzureServiceBus:
                    AddNugetDependency(NugetPackages.NServiceBusTransportAzureServiceBus(OutputTarget));
                    break;
                case NServiceBusSettings.TransportOptionsEnum.AmazonSqs:
                    AddNugetDependency(NugetPackages.NServiceBusAmazonSQS(OutputTarget));
                    break;
                case NServiceBusSettings.TransportOptionsEnum.LearningTransport:
                    // Learning Transport is included in NServiceBus.Core — no extra package needed
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported transport type: {transport.Value}");
            }

            if (outboxPattern.IsSqlPersistence())
            {
                if (!ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.EntityFrameworkCore"))
                    throw new FriendlyException(
                        "**NServiceBus Outbox — missing dependency**\n\n" +
                        "OutboxPattern is set to **SqlPersistence** but the `Intent.EntityFrameworkCore` module is not installed.\n\n" +
                        "The NServiceBus transactional outbox shares the EF Core `DbConnection`/`DbTransaction` to ensure " +
                        "exactly-once message dispatch alongside your database writes.\n\n" +
                        "**Fix:** Install `Intent.EntityFrameworkCore`, or change the OutboxPattern module setting to `None`.");
                
                AddNugetDependency(NugetPackages.NServiceBusPersistenceSql(OutputTarget));
                AddNugetDependency(NugetPackages.MicrosoftDataSqlClient(OutputTarget));
            }

            if (_isLegacyFramework)
            {
                var programTemplate = ExecutionContext.FindTemplateInstance<IProgramTemplate>("App.Program");
                if (programTemplate != null)
                {
                    programTemplate.CSharpFile.OnBuild(file =>
                    {
                        file.AddUsing(this.Namespace);
                        programTemplate.ProgramFile.AddHostBuilderConfigurationStatement(
                            new CSharpStatement("builder.Host.UseNServiceBusHost();"),
                            priority: 500);
                    }, 30);
                }
            }

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.IO")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("NServiceBus");

            if (_isLegacyFramework)
                CSharpFile.AddUsing("Microsoft.Extensions.Hosting");

            if (outboxPattern.IsSqlPersistence())
            {
                CSharpFile.AddUsing("Microsoft.Data.SqlClient");
                CSharpFile.AddUsing("NServiceBus.TransactionalSession");
            }

            CSharpFile.AddClass("NServiceBusConfiguration", @class =>
            {
                @class.Static();

                // ── Message & handler discovery ────────────────────────────────────────────────────
                var eventTemplates = ExecutionContext
                    .FindTemplateInstances<CSharpTemplateBase<MessageModel>>(IntegrationEventMessageTemplate.TemplateId)
                    .FilterMessagesForThisMessageBroker(ExecutionContext, BrokerStereotypeIds, t => t.Model)
                    .ToList();

                var commandTemplates = ExecutionContext
                    .FindTemplateInstances<CSharpTemplateBase<IntegrationCommandModel>>(IntegrationCommandTemplate.TemplateId)
                    .FilterMessagesForThisMessageBroker(ExecutionContext, BrokerStereotypeIds, t => t.Model)
                    .ToList();

                var handlerTemplate = ExecutionContext
                    .FindTemplateInstances<NServiceBusMessageHandlerTemplate>(NServiceBusMessageHandlerTemplate.TemplateId)
                    .FirstOrDefault();

                var commandSubscriptions = handlerTemplate?.SubscribedCommandModels ?? new List<IntegrationCommandModel>();

                // Commands this app SENDS (knows about but does not handle) — need routing on the main endpoint.
                var subscribedIds = new HashSet<string>(commandSubscriptions.Select(c => c.Id));
                var sentCommandTemplates = commandTemplates
                    .Where(ct => !subscribedIds.Contains(ct.Model.Id))
                    .ToList();

                // ALL NServiceBus commands must have EndpointName — it is a property of the command
                // definition, not of the sender. Both the sending app and the subscribing app must
                // agree on the same name. Missing it here means incomplete definition regardless of
                // whether this app sends or handles the command.
                foreach (var ct in commandTemplates
                    .Where(ct => string.IsNullOrEmpty(ct.Model.GetNServiceBus()?.EndpointName())))
                {
                    throw new ElementException(ct.Model.InternalElement,
                        $"Integration Command `{ct.Model.Name}` has no NServiceBus endpoint name configured.\n\n" +
                        "The **Endpoint Name** identifies which endpoint owns this command — it must be set consistently " +
                        "on every app that sends or subscribes to it.\n\n" +
                        "Apply the **NServiceBus** stereotype to this command and set **Endpoint Name** to the " +
                        "endpoint name of the application that handles it (its `NServiceBus:EndpointName` config value).");
                }

                // ── AddNServiceBusConfiguration ────────────────────────────────────────────────────
                @class.AddMethod("IServiceCollection", "AddNServiceBusConfiguration", method =>
                {
                    method.Static();
                    method.AddParameter("IServiceCollection", "services", p => p.WithThisModifier());
                    method.AddParameter("IConfiguration", "configuration");

                    var requiresCompositeMessageBus = this.RequiresCompositeMessageBus();
                    var nsbBusName = this.GetTypeName(NServiceBusMessageBusTemplate.TemplateId);
                    if (requiresCompositeMessageBus)
                    {
                        method.AddParameter(this.GetMessageBrokerRegistryName(), "registry");
                        method.AddStatement($"services.AddScoped<{nsbBusName}>();");
                    }
                    else
                    {
                        var busInterface = this.GetBusInterfaceName();
                        method.AddStatement($"services.AddScoped<{nsbBusName}>();");
                        method.AddStatement($"services.AddScoped<{busInterface}>(provider => provider.GetRequiredService<{nsbBusName}>());");
                    }

                    if (requiresCompositeMessageBus)
                    {
                        foreach (var et in eventTemplates)
                        {
                            var msgName = GetTypeName(IntegrationEventMessageTemplate.TemplateId, et.Model);
                            method.AddStatement($"registry.Register<{msgName}, {nsbBusName}>();");
                        }

                        foreach (var ct in commandTemplates)
                        {
                            var msgName = GetTypeName(IntegrationCommandTemplate.TemplateId, ct.Model);
                            method.AddStatement($"registry.Register<{msgName}, {nsbBusName}>();");
                        }
                    }

                    if (!_isLegacyFramework)
                    {
                        method.AddStatement("services.AddNServiceBusEndpoint(ConfigureMainEndpoint(configuration));",
                            s => s.SeparatedFromPrevious());
                    }

                    method.AddReturn("services");
                });

                // ── UseNServiceBusHost (v9 / .NET 8-9 host-builder wiring) ──────────────
                if (_isLegacyFramework)
                {
                    @class.AddMethod("IHostBuilder", "UseNServiceBusHost", method =>
                    {
                        method.Static().Public();
                        method.AddParameter("IHostBuilder", "hostBuilder", p => p.WithThisModifier());
                        method.AddReturn("hostBuilder.UseNServiceBus(ctx => ConfigureMainEndpoint(ctx.Configuration))");
                    });
                }

                // ── ConfigureMainEndpoint ──────────────────────────────────────────────────────────
                @class.AddMethod("EndpointConfiguration", "ConfigureMainEndpoint", method =>
                {
                    method.Static().Private();
                    method.AddParameter("IConfiguration", "configuration");

                    method.AddStatement(@"var endpointName = configuration[""NServiceBus:EndpointName""] ?? throw new InvalidOperationException(""NServiceBus:EndpointName is not configured"");");
                    method.AddStatement("var endpointConfiguration = new EndpointConfiguration(endpointName);");

                    var needsRouting = sentCommandTemplates.Count > 0 || commandSubscriptions.Count > 0;
                    AddTransportStatements(method, captureVar: needsRouting);
                    AddPersistenceStatements(method);

                    method.AddStatement("endpointConfiguration.EnableInstallers();", s => s.SeparatedFromPrevious());
                    method.AddStatement("endpointConfiguration.UseSerialization<SystemJsonSerializer>();");

                    AddRecoverabilityStatements(method);

                    if (eventTemplates.Count > 0 || commandTemplates.Count > 0)
                    {
                        method.AddStatement("ConfigureMessageConventions(endpointConfiguration);", s => s.SeparatedFromPrevious());
                    }

                    var subscribedMessages = handlerTemplate?.SubscribedMessageModels ?? new List<MessageModel>();
                    if (subscribedMessages.Any() || commandSubscriptions.Any())
                    {
                        method.AddStatement("RegisterHandlers(endpointConfiguration);");
                    }

                    if (needsRouting)
                    {
                        var first = true;
                        // Commands sent to other endpoints — route to their declared destination
                        foreach (var ct in sentCommandTemplates)
                        {
                            var destinationEndpoint = ct.Model.GetNServiceBus()!.EndpointName();
                            var commandTypeName = GetTypeName(IntegrationCommandTemplate.TemplateId, ct.Model);
                            var commandName = ct.Model.Name;
                            var isFirst = first; first = false;
                            method.AddStatement(
                                $"""routing.RouteToEndpoint(typeof({commandTypeName}), configuration["NServiceBus:Routing:Commands:{commandName}"] ?? "{destinationEndpoint}");""",
                                s => { if (isFirst) s.SeparatedFromPrevious(); });
                        }
                        // Commands handled by this endpoint — route to self so Send() works when this app also sends them
                        foreach (var cmd in commandSubscriptions)
                        {
                            var cmdTypeName = GetTypeName(IntegrationCommandTemplate.TemplateId, cmd);
                            var isFirst = first; first = false;
                            method.AddStatement(
                                $"routing.RouteToEndpoint(typeof({cmdTypeName}), endpointName);",
                                s => { if (isFirst) s.SeparatedFromPrevious(); });
                        }
                    }

                    method.AddReturn("endpointConfiguration", s => s.SeparatedFromPrevious());
                });

                // ── ConfigureMessageConventions ────────────────────────────────────────────────────
                if (eventTemplates.Count > 0 || commandTemplates.Count > 0)
                {
                    @class.AddMethod("void", "ConfigureMessageConventions", method =>
                    {
                        method.Static().Private();
                        method.AddParameter("EndpointConfiguration", "endpointConfiguration");

                        method.AddStatement("var conventions = endpointConfiguration.Conventions();");

                        if (eventTemplates.Count > 0)
                        {
                            var typeOfs = string.Join(", ",
                                eventTemplates.Select(t => $"typeof({GetTypeName(IntegrationEventMessageTemplate.TemplateId, t.Model)})"));
                            method.AddStatement($"conventions.DefiningEventsAs(new[] {{ {typeOfs} }}.Contains);");
                        }

                        if (commandTemplates.Count > 0)
                        {
                            var typeOfs = string.Join(", ",
                                commandTemplates.Select(t => $"typeof({GetTypeName(IntegrationCommandTemplate.TemplateId, t.Model)})"));
                            method.AddStatement($"conventions.DefiningCommandsAs(new[] {{ {typeOfs} }}.Contains);");
                        }
                    });
                }

                // ── RegisterHandlers + RegisterHandler ────────────────────────────────────────────
                var subscribedEventsForReg = handlerTemplate?.SubscribedMessageModels ?? new List<MessageModel>();
                if (subscribedEventsForReg.Any() || commandSubscriptions.Any())
                {
                    @class.AddMethod("void", "RegisterHandlers", method =>
                    {
                        method.Static().Private();
                        method.AddParameter("EndpointConfiguration", "endpointConfiguration");

                        var handlerTypeName = GetTypeName(NServiceBusMessageHandlerTemplate.TemplateId);

                        foreach (var msg in subscribedEventsForReg)
                        {
                            var msgTypeName = GetTypeName(IntegrationEventMessageTemplate.TemplateId, msg);
                            method.AddStatement($"RegisterHandler<{handlerTypeName}<{msgTypeName}>, {msgTypeName}>(endpointConfiguration);");
                        }

                        foreach (var cmd in commandSubscriptions)
                        {
                            var cmdTypeName = GetTypeName(IntegrationCommandTemplate.TemplateId, cmd);
                            method.AddStatement($"RegisterHandler<{handlerTypeName}<{cmdTypeName}>, {cmdTypeName}>(endpointConfiguration);");
                        }
                    });

                    @class.AddMethod("void", "RegisterHandler", method =>
                    {
                        method.Static().Private();
                        method.AddGenericParameter("THandler");
                        method.AddGenericParameter("TMessage");
                        method.AddGenericTypeConstraint("THandler", c => c.AddType("class").AddType("IHandleMessages<TMessage>"));
                        method.AddGenericTypeConstraint("TMessage", c => c.AddType("class"));
                        method.AddParameter("EndpointConfiguration", "endpointConfiguration");
                        method.AddStatement("var settings = NServiceBus.Configuration.AdvancedExtensibility.AdvancedExtensibilityExtensions.GetSettings(endpointConfiguration);");
                        method.AddStatement("var messageHandlerRegistry = settings.GetOrCreate<NServiceBus.Unicast.MessageHandlerRegistry>();");
                        method.AddStatement("var messageMetadataRegistry = settings.GetOrCreate<NServiceBus.Unicast.Messages.MessageMetadataRegistry>();");
                        method.AddStatement("messageHandlerRegistry.AddMessageHandlerForMessage<THandler, TMessage>();");
                        method.AddStatement("messageMetadataRegistry.RegisterMessageTypeWithHierarchy(typeof(TMessage), Array.Empty<Type>());");
                    });
                }
            });
        }

        public override void BeforeTemplateExecution()
        {
            if (!this.RequiresCompositeMessageBus())
            {
                ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                    .ToRegister("AddNServiceBusConfiguration", ServiceConfigurationRequest.ParameterType.Configuration)
                    .ForConcern("Infrastructure")
                    .HasDependency(this));
            }

            PublishAppSettings();
        }

        private void AddTransportStatements(CSharpClassMethod method, bool captureVar)
        {
            var transport = ExecutionContext.Settings.GetNServiceBusSettings().Transport();
            var varPrefix = captureVar ? "var routing = " : "";

            if (transport.IsRabbitmq())
            {
                method.AddStatement(
                    @"var connectionString = configuration.GetConnectionString(""RabbitMQ"") ?? throw new InvalidOperationException(""ConnectionStrings:RabbitMQ is not configured"");",
                    s => s.SeparatedFromPrevious());
                method.AddStatement($"{varPrefix}endpointConfiguration.UseTransport(new RabbitMQTransport(RoutingTopology.Conventional(QueueType.Quorum), connectionString));");
            }
            else if (transport.IsAzureServiceBus())
            {
                method.AddStatement(
                    @"var connectionString = configuration.GetConnectionString(""AzureServiceBus"") ?? throw new InvalidOperationException(""ConnectionStrings:AzureServiceBus is not configured"");",
                    s => s.SeparatedFromPrevious());
                method.AddStatement($"{varPrefix}endpointConfiguration.UseTransport(new AzureServiceBusTransport(connectionString, TopicTopology.Default));");
            }
            else if (transport.IsAmazonSqs())
            {
                method.AddStatement($"{varPrefix}endpointConfiguration.UseTransport(new SqsTransport());", s => s.SeparatedFromPrevious());
            }
            else if (transport.IsLearningTransport())
            {
                method.AddStatement(
                    """
                    var rawStoragePath = configuration["NServiceBus:LearningTransport:StorageDirectory"];
                    """,
                    s => s.SeparatedFromPrevious());
                method.AddStatement(new CSharpAssignmentStatement(
                    new CSharpVariableDeclaration("storageDirectory"),
                    new CSharpConditionalExpressionStatement(
                        "rawStoragePath is not null",
                        "Environment.ExpandEnvironmentVariables(rawStoragePath)",
                        @"Path.Combine(Path.GetTempPath(), ""nservicebus-learning"")")).WithSemicolon().SeparatedFromPrevious());
                method.AddStatement($"{varPrefix}endpointConfiguration.UseTransport(new LearningTransport {{ StorageDirectory = storageDirectory }});");
            }
        }

        private void AddPersistenceStatements(CSharpClassMethod method)
        {
            var outboxPattern = ExecutionContext.Settings.GetNServiceBusSettings().OutboxPattern();
            if (!outboxPattern.IsSqlPersistence()) return;

            method.AddStatement(
                @"var persistenceConnectionString = configuration.GetConnectionString(""DefaultConnection"") ?? throw new InvalidOperationException(""ConnectionStrings:DefaultConnection is not configured"");",
                s => s.SeparatedFromPrevious());
            method.AddStatement("var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();");
            method.AddStatement("persistence.SqlDialect<SqlDialect.MsSqlServer>();");
            method.AddStatement("persistence.ConnectionBuilder(connectionBuilder: () => new SqlConnection(persistenceConnectionString));");
            method.AddStatement("persistence.EnableTransactionalSession();");
            method.AddStatement("endpointConfiguration.EnableOutbox();");
        }

        private void AddRecoverabilityStatements(CSharpClassMethod method)
        {
            var recoverabilityPolicy = ExecutionContext.Settings.GetNServiceBusSettings().RecoverabilityPolicy();
            if (recoverabilityPolicy.IsNone()) return;

            var recoverability = new CSharpStatement("endpointConfiguration.Recoverability()");

            if (recoverabilityPolicy.IsImmediateOnly() || recoverabilityPolicy.IsImmediateAndDelayed())
                recoverability = recoverability.AddInvocation("Immediate", inv => inv
                    .AddArgument("""r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:ImmediateRetries", 5))""")
                    .OnNewLine());

            if (recoverabilityPolicy.IsDelayedOnly() || recoverabilityPolicy.IsImmediateAndDelayed())
                recoverability = recoverability.AddInvocation("Delayed", inv => inv
                    .AddArgument("""r => r.NumberOfRetries(configuration.GetValue<int>("NServiceBus:Recoverability:DelayedRetries", 3)).TimeIncrease(TimeSpan.FromSeconds(configuration.GetValue<int>("NServiceBus:Recoverability:DelayIncreaseSeconds", 10)))""")
                    .OnNewLine());

            method.AddStatement(recoverability, s => s.SeparatedFromPrevious());
            method.AddStatement("""endpointConfiguration.SendFailedMessagesTo(configuration["NServiceBus:ErrorQueue"] ?? "error");""");
        }

        private void PublishAppSettings()
        {
            var transport = ExecutionContext.Settings.GetNServiceBusSettings().Transport();
            var recoverabilityPolicy = ExecutionContext.Settings.GetNServiceBusSettings().RecoverabilityPolicy();

            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(
                "NServiceBus:EndpointName", OutputTarget.ApplicationName()));

            switch (transport.AsEnum())
            {
                case NServiceBusSettings.TransportOptionsEnum.LearningTransport:
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(
                        "NServiceBus:LearningTransport:StorageDirectory",
                        GetLearningTransportDefaultPath()));
                    break;
                case NServiceBusSettings.TransportOptionsEnum.Rabbitmq:
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(
                        "ConnectionStrings:RabbitMQ", "amqp://guest:guest@localhost:5672"));
                    break;
                case NServiceBusSettings.TransportOptionsEnum.AzureServiceBus:
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(
                        "ConnectionStrings:AzureServiceBus", "Endpoint=sb://<namespace>.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=<key>"));
                    break;
                case NServiceBusSettings.TransportOptionsEnum.AmazonSqs:
                    // Amazon SQS uses AWS credentials/environment — no connection string needed
                    break;
            }

            if (!recoverabilityPolicy.IsNone())
            {
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(
                    "NServiceBus:ErrorQueue", "error"));

                if (recoverabilityPolicy.IsImmediateOnly() || recoverabilityPolicy.IsImmediateAndDelayed())
                {
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(
                        "NServiceBus:Recoverability:ImmediateRetries", 5));
                }

                if (recoverabilityPolicy.IsDelayedOnly() || recoverabilityPolicy.IsImmediateAndDelayed())
                {
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(
                        "NServiceBus:Recoverability:DelayedRetries", 3));
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(
                        "NServiceBus:Recoverability:DelayIncreaseSeconds", 10));
                }
            }
        }

        private static string GetLearningTransportDefaultPath()
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                return @"%TEMP%\nservicebus-learning";

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
                return "%TMPDIR%/nservicebus-learning";

            // Linux: TMPDIR is not reliably set; use /tmp directly
            return "/tmp/nservicebus-learning";
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
