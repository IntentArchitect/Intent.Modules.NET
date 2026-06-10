using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing.NServiceBus.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
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

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public NServiceBusConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            FulfillsRole("Infrastructure.DependencyInjection.NServiceBus");
            FulfillsRole(TemplateRoles.Application.Eventing.MessageBusConfiguration);

            AddTypeSource(IntegrationEventMessageTemplate.TemplateId);
            AddTypeSource(IntegrationCommandTemplate.TemplateId);
            AddTypeSource(NServiceBusMessageHandlerTemplate.TemplateId);

            AddNugetDependency(NugetPackages.NServiceBus(OutputTarget));

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
                    throw new InvalidOperationException(
                        "OutboxPattern is set to 'SqlPersistence' but the 'Intent.EntityFrameworkCore' module is not installed. " +
                        "The NServiceBus transactional outbox requires EF Core to share the same database transaction. " +
                        "Please install 'Intent.EntityFrameworkCore' or change OutboxPattern to 'None'.");

                AddNugetDependency(NugetPackages.NServiceBusPersistenceSql(OutputTarget));
                AddNugetDependency(NugetPackages.MicrosoftDataSqlClient(OutputTarget));
            }

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.IO")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("NServiceBus");

            if (outboxPattern.IsSqlPersistence())
            {
                CSharpFile.AddUsing("Microsoft.Data.SqlClient");
                CSharpFile.AddUsing("NServiceBus.TransactionalSession");
            }

            CSharpFile.AddClass("NServiceBusConfiguration", @class =>
            {
                @class.Static();

                // ── Message & handler discovery (class scope — shared by all method lambdas) ──────
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

                // Group handled commands by resolved endpoint name.
                // Same endpoint name → same NSB endpoint (one queue, multiple handlers).
                var commandGroups = commandSubscriptions
                    .GroupBy(ResolveCommandEndpointName)
                    .ToList();

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

                    // Command endpoints first, main endpoint last.
                    // The last AddNServiceBusEndpoint call wins the IMessageSession DI registration —
                    // NServiceBusMessageBus relies on this for out-of-handler publish/send.
                    var firstEndpoint = true;
                    foreach (var group in commandGroups)
                    {
                        var isFirst = firstEndpoint; firstEndpoint = false;
                        var suffix = EndpointNameToMethodSuffix(group.Key);
                        method.AddStatement(
                            $"services.AddNServiceBusEndpoint(ConfigureEndpointFor{suffix}(configuration));",
                            s => { if (isFirst) s.SeparatedFromPrevious(); });
                    }

                    method.AddStatement(
                        "services.AddNServiceBusEndpoint(ConfigureMainEndpoint(configuration));",
                        s => { if (firstEndpoint) s.SeparatedFromPrevious(); });

                    method.AddReturn("services");
                });

                // ── ConfigureMainEndpoint ──────────────────────────────────────────────────────────
                // Handles events + routes sent commands. Named by NServiceBus:EndpointName (app name).
                @class.AddMethod("EndpointConfiguration", "ConfigureMainEndpoint", method =>
                {
                    method.Static().Private();
                    method.AddParameter("IConfiguration", "configuration");

                    method.AddStatement(@"var endpointName = configuration[""NServiceBus:EndpointName""] ?? throw new InvalidOperationException(""NServiceBus:EndpointName is not configured"");");
                    method.AddStatement("var endpointConfiguration = new EndpointConfiguration(endpointName);");

                    AddTransportStatements(method, captureVar: sentCommandTemplates.Count > 0);
                    AddPersistenceStatements(method);

                    method.AddStatement("endpointConfiguration.EnableInstallers();", s => s.SeparatedFromPrevious());
                    method.AddStatement("endpointConfiguration.UseSerialization<SystemJsonSerializer>();");

                    if (eventTemplates.Count > 0 || sentCommandTemplates.Count > 0)
                    {
                        method.AddStatement("var conventions = endpointConfiguration.Conventions();", s => s.SeparatedFromPrevious());

                        if (eventTemplates.Count > 0)
                        {
                            var typeOfs = string.Join(", ",
                                eventTemplates.Select(t => $"typeof({GetTypeName(IntegrationEventMessageTemplate.TemplateId, t.Model)})"));
                            method.AddStatement($"conventions.DefiningEventsAs(new[] {{ {typeOfs} }}.Contains);");
                        }

                        if (sentCommandTemplates.Count > 0)
                        {
                            var typeOfs = string.Join(", ",
                                sentCommandTemplates.Select(t => $"typeof({GetTypeName(IntegrationCommandTemplate.TemplateId, t.Model)})"));
                            method.AddStatement($"conventions.DefiningCommandsAs(new[] {{ {typeOfs} }}.Contains);");
                        }
                    }

                    AddRecoverabilityStatements(method);

                    if (sentCommandTemplates.Count > 0)
                    {
                        var first = true;
                        foreach (var ct in sentCommandTemplates)
                        {
                            var resolvedEndpoint = ResolveCommandEndpointName(ct.Model);
                            var commandTypeName = GetTypeName(IntegrationCommandTemplate.TemplateId, ct.Model);
                            var commandName = ct.Model.Name;
                            var isFirst = first; first = false;
                            method.AddStatement(
                                $"""transportConfig.RouteToEndpoint(typeof({commandTypeName}), configuration["NServiceBus:Routing:Commands:{commandName}"] ?? "{resolvedEndpoint}");""",
                                s => { if (isFirst) s.SeparatedFromPrevious(); });
                        }
                    }

                    method.AddReturn("endpointConfiguration", s => s.SeparatedFromPrevious());
                });

                // ── Per-command-group endpoint methods ────────────────────────────────────────────
                // One method per unique resolved endpoint name among handled commands.
                foreach (var group in commandGroups)
                {
                    var groupKey = group.Key;
                    var cmdsInGroup = group.ToList();
                    // Single command with no stereotype → convention name; allow config override.
                    var isSingleConvention = cmdsInGroup.Count == 1
                        && string.IsNullOrEmpty(cmdsInGroup[0].GetNServiceBus()?.EndpointName());
                    var suffix = EndpointNameToMethodSuffix(groupKey);

                    @class.AddMethod("EndpointConfiguration", $"ConfigureEndpointFor{suffix}", method =>
                    {
                        method.Static().Private();
                        method.AddParameter("IConfiguration", "configuration");

                        if (isSingleConvention)
                        {
                            // Convention-derived name: allow override via the same key the publisher uses for routing
                            // so both sides can be changed in sync through config.
                            var cmdName = cmdsInGroup[0].Name;
                            method.AddStatement($@"var endpointName = configuration[""NServiceBus:Routing:Commands:{cmdName}""] ?? ""{groupKey}"";");
                        }
                        else
                        {
                            // Stereotype-set or multi-command group: designer is the source of truth.
                            method.AddStatement($@"var endpointName = ""{groupKey}"";");
                        }

                        method.AddStatement("var endpointConfiguration = new EndpointConfiguration(endpointName);");

                        // Command endpoints never route — no need to capture the transport config variable.
                        AddTransportStatements(method, captureVar: false);
                        AddPersistenceStatements(method);

                        method.AddStatement("endpointConfiguration.EnableInstallers();", s => s.SeparatedFromPrevious());
                        method.AddStatement("endpointConfiguration.UseSerialization<SystemJsonSerializer>();");

                        var typeOfs = string.Join(", ",
                            cmdsInGroup.Select(cmd => $"typeof({GetTypeName(IntegrationCommandTemplate.TemplateId, cmd)})"));
                        method.AddStatement("var conventions = endpointConfiguration.Conventions();", s => s.SeparatedFromPrevious());
                        method.AddStatement($"conventions.DefiningCommandsAs(new[] {{ {typeOfs} }}.Contains);");

                        AddRecoverabilityStatements(method);

                        // Handler registrations are inserted by the OnBuild callback below.

                        method.AddReturn("endpointConfiguration", s => s.SeparatedFromPrevious());
                    });
                }
            });

            // OnBuild: insert RegisterHandler<> calls into the correct endpoint method per message type.
            // Runs after the main class lambdas so all endpoint methods are already defined.
            CSharpFile.OnBuild(file =>
            {
                var handlerTemplate = ExecutionContext
                    .FindTemplateInstances<NServiceBusMessageHandlerTemplate>(NServiceBusMessageHandlerTemplate.TemplateId)
                    .FirstOrDefault();

                var eventSubscriptions = handlerTemplate?.SubscribedMessageModels ?? new List<MessageModel>();
                var commandSubscriptions = handlerTemplate?.SubscribedCommandModels ?? new List<IntegrationCommandModel>();

                if (!eventSubscriptions.Any() && !commandSubscriptions.Any()) return;

                var cls = file.Classes.First();
                var handlerTypeName = this.GetTypeName(NServiceBusMessageHandlerTemplate.TemplateId);

                // Event handlers → main endpoint
                if (eventSubscriptions.Any())
                {
                    var mainMethod = cls.FindMethod("ConfigureMainEndpoint");
                    if (mainMethod != null)
                    {
                        foreach (var sub in eventSubscriptions)
                        {
                            var msgType = this.GetTypeName(IntegrationEventMessageTemplate.TemplateId, sub);
                            mainMethod.InsertStatement(
                                mainMethod.Statements.Count - 1,
                                $"RegisterHandler<{handlerTypeName}<{msgType}>, {msgType}>(endpointConfiguration);");
                        }
                    }
                }

                // Command handlers → their group's endpoint method
                var commandGroups = commandSubscriptions
                    .GroupBy(ResolveCommandEndpointName)
                    .ToList();

                foreach (var group in commandGroups)
                {
                    var methodName = $"ConfigureEndpointFor{EndpointNameToMethodSuffix(group.Key)}";
                    var endpointMethod = cls.FindMethod(methodName);
                    if (endpointMethod == null) continue;

                    foreach (var cmd in group)
                    {
                        var cmdType = this.GetTypeName(IntegrationCommandTemplate.TemplateId, cmd);
                        endpointMethod.InsertStatement(
                            endpointMethod.Statements.Count - 1,
                            $"RegisterHandler<{handlerTypeName}<{cmdType}>, {cmdType}>(endpointConfiguration);");
                    }
                }

                // RegisterHandler helper — added once, reachable from all endpoint methods.
                cls.AddMethod("void", "RegisterHandler", method =>
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

        // Resolves the NSB endpoint name for a command: stereotype override, else command name in kebab-case.
        // Used as the GroupBy key so commands with the same resolved name share one NSB endpoint.
        private static string ResolveCommandEndpointName(IntegrationCommandModel cmd)
        {
            var stereotype = cmd.GetNServiceBus()?.EndpointName();
            return !string.IsNullOrEmpty(stereotype) ? stereotype : cmd.Name.ToKebabCase();
        }

        // Converts an endpoint name string to a PascalCase method-name suffix.
        // "test-command" → "TestCommand", "x-group-command" → "XGroupCommand"
        private static string EndpointNameToMethodSuffix(string endpointName) =>
            string.Concat(
                endpointName
                    .Split(new[] { '-', '.', '_', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(part => char.ToUpperInvariant(part[0]) + part.Substring(1)));

        private void AddTransportStatements(CSharpClassMethod method, bool captureVar)
        {
            var transport = ExecutionContext.Settings.GetNServiceBusSettings().Transport();
            var varPrefix = captureVar ? "var transportConfig = " : "";

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
                    var storageDirectory = rawStoragePath is not null
                        ? Environment.ExpandEnvironmentVariables(rawStoragePath)
                        : Path.Combine(Path.GetTempPath(), "nservicebus-learning");
                    """,
                    s => s.SeparatedFromPrevious());
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
