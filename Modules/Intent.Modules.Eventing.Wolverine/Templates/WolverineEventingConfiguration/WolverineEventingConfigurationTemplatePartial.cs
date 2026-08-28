using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing.Wolverine.Api;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationCommand;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventHandler;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.Modules.Eventing.Wolverine.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Wolverine.Templates.WolverineEventingConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class WolverineEventingConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Wolverine.WolverineEventingConfiguration";

        private readonly WolverineMessageBusSettings.TransportOptions _transport;
        private readonly WolverineMessageBusSettings.TransactionalOutboxOptions _transactionalOutbox;
        private readonly WolverineMessageBusSettings.ErrorHandlingPolicyOptions _errorHandlingPolicy;
        private readonly DatabaseProviderExtensions.DatabaseProviderOptions? _databaseProvider;
        private readonly string? _connectionStringName;
        private readonly IReadOnlyList<MessageModel> _publishedMessages;
        private readonly IReadOnlyList<IntegrationCommandModel> _sentCommands;
        private readonly IReadOnlyList<MessageModel> _subscribedMessages;
        private readonly IReadOnlyList<IntegrationCommandModel> _receivedCommands;
        private readonly string _applicationNameKebab;
        private readonly IReadOnlyList<IntegrationEventHandlerModel> _subscribedHandlers;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public WolverineEventingConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var wolverineSettings = ExecutionContext.Settings.GetWolverineMessageBusSettings();
            _transport = wolverineSettings.Transport();
            _transactionalOutbox = wolverineSettings.TransactionalOutbox();
            _errorHandlingPolicy = wolverineSettings.ErrorHandlingPolicy();

            AddTypeSource(IntegrationEventMessageTemplate.TemplateId);
            AddTypeSource(IntegrationCommandTemplate.TemplateId);

            _publishedMessages = this.GetWolverineDesignatedMessages(
                ExecutionContext.MetadataManager.GetExplicitlyPublishedMessageModels(OutputTarget.Application))
                .ToList();
            _sentCommands = this.GetWolverineDesignatedIntegrationCommands(
                ExecutionContext.MetadataManager.GetExplicitlySentIntegrationCommandModels(OutputTarget.Application))
                .ToList();
            _subscribedMessages = this.GetWolverineDesignatedMessages(
                ExecutionContext.MetadataManager.GetExplicitlySubscribedToMessageModels(OutputTarget.Application))
                .ToList();
            _receivedCommands = this.GetWolverineDesignatedIntegrationCommands(
                ExecutionContext.MetadataManager.GetExplicitlySubscribedToIntegrationCommandModels(OutputTarget.Application))
                .ToList();
            _applicationNameKebab = ExecutionContext.GetApplicationConfig().Name.ToKebabCase();
            _subscribedHandlers = GetWolverineDesignatedSubscribedHandlers();

            // WolverineFx is the base package regardless of Transport; the transport-specific
            // package is only added for the Transport actually selected, never speculatively.
            AddNugetDependency(NugetPackages.WolverineFx(OutputTarget));

            switch (_transport.AsEnum())
            {
                case WolverineMessageBusSettings.TransportOptionsEnum.Local:
                    break;
                case WolverineMessageBusSettings.TransportOptionsEnum.Rabbitmq:
                    AddNugetDependency(NugetPackages.WolverineFxRabbitMQ(OutputTarget));
                    break;
                case WolverineMessageBusSettings.TransportOptionsEnum.AzureServiceBus:
                    AddNugetDependency(NugetPackages.WolverineFxAzureServiceBus(OutputTarget));
                    break;
                case WolverineMessageBusSettings.TransportOptionsEnum.AmazonSqs:
                    AddNugetDependency(NugetPackages.WolverineFxAmazonSqs(OutputTarget));
                    // R3.1: SQS is point-to-point only - SNS is the fan-out primitive an
                    // Integration Event's publish rule targets, and it ships as its own package.
                    AddNugetDependency(NugetPackages.WolverineFxAmazonSns(OutputTarget));
                    break;
                default:
                    throw new System.InvalidOperationException($"Unsupported transport type: {_transport.Value}");
            }

            if (_transactionalOutbox.IsDurable())
            {
                if (!ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.EntityFrameworkCore"))
                {
                    throw new FriendlyException(
                        "Transactional Outbox is set to Durable but the Intent.EntityFrameworkCore module is not installed. " +
                        "Wolverine's durable outbox shares the EF Core DbContext to guarantee messages are only sent once " +
                        "the related database transaction commits. Install Intent.EntityFrameworkCore, or change the " +
                        "Transactional Outbox setting to None.");
                }

                _databaseProvider = ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider();
                if (!_databaseProvider.IsSupported())
                {
                    throw new FriendlyException(
                        $"Transactional Outbox is set to Durable but the configured Database Provider ('{_databaseProvider.Value}') " +
                        "is not supported. Wolverine's durable outbox only supports SQL Server and PostgreSQL. Change the Database " +
                        "Provider to one of those, or change the Transactional Outbox setting to None.");
                }

                _connectionStringName = ExecutionContext.Settings.GetDatabaseSettings().ConnectionStringName();

                AddNugetDependency(NugetPackages.WolverineFxEntityFrameworkCore(OutputTarget));
                AddNugetDependency(_databaseProvider.IsSqlServer()
                    ? NugetPackages.WolverineFxSqlServer(OutputTarget)
                    : NugetPackages.WolverineFxPostgresql(OutputTarget));
            }

            var transportUsing = _transport.AsEnum() switch
            {
                WolverineMessageBusSettings.TransportOptionsEnum.Rabbitmq => "Wolverine.RabbitMQ",
                WolverineMessageBusSettings.TransportOptionsEnum.AzureServiceBus => "Wolverine.AzureServiceBus",
                WolverineMessageBusSettings.TransportOptionsEnum.AmazonSqs => "Wolverine.AmazonSqs",
                _ => null
            };

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Wolverine")
                .AddUsing("Wolverine.ErrorHandling")
                .AddUsing("Microsoft.Extensions.Configuration");

            if (transportUsing != null)
            {
                CSharpFile.AddUsing(transportUsing);
            }

            if (_transport.AsEnum() == WolverineMessageBusSettings.TransportOptionsEnum.AmazonSqs)
            {
                // RegionEndpoint (Amazon.SQS's AmazonSQSConfig, which UseAmazonSqsTransport configures,
                // derives from Amazon.Runtime.ClientConfig) and BasicAWSCredentials for the optional
                // AccessKey/SecretKey pair (R2.2).
                CSharpFile.AddUsing("Amazon");
                CSharpFile.AddUsing("Amazon.Runtime");
                // R3.1: ToSnsTopic/SubscribeSqsQueue live in Wolverine.AmazonSns, not Wolverine.AmazonSqs.
                CSharpFile.AddUsing("Wolverine.AmazonSns");
            }

            if (_transactionalOutbox.IsDurable())
            {
                // R6.3: UseEntityFrameworkCoreTransactions() (DbContext integration + AutoApplyTransactions)
                // and PersistMessagesWith{Provider} (message-persistence) - call shapes confirmed against
                // Wolverine.EntityFrameworkCore/Wolverine.SqlServer/Wolverine.Postgresql at 5.39.5 via
                // intent/.specs/wolverine-eventing-module/golden-sample/probes/DurableAndTransportProbe/Probe.cs.
                CSharpFile.AddUsing("Wolverine.EntityFrameworkCore");
                CSharpFile.AddUsing(_databaseProvider!.IsSqlServer() ? "Wolverine.SqlServer" : "Wolverine.Postgresql");
            }

            CSharpFile.AddClass("WolverineEventingConfiguration", @class =>
            {
                @class.Static();

                switch (_transport.AsEnum())
                {
                    case WolverineMessageBusSettings.TransportOptionsEnum.Local:
                        AddConfigureLocal(@class);
                        break;
                    case WolverineMessageBusSettings.TransportOptionsEnum.Rabbitmq:
                        AddConfigureRabbitMq(@class);
                        break;
                    case WolverineMessageBusSettings.TransportOptionsEnum.AzureServiceBus:
                        AddConfigureAzureServiceBus(@class);
                        break;
                    case WolverineMessageBusSettings.TransportOptionsEnum.AmazonSqs:
                        AddConfigureAmazonSqs(@class);
                        break;
                }

                // R5.6/R5.8: the Error Handling Policy seam every Configure{Transport} method
                // above calls exactly once, host-scope. Empty until the Error Handling Policy
                // module setting exists (wave 6, T6.2) and its emission (T6.7-T6.9) fills this
                // body in - deliberately not wired to any setting yet, so this wave carries no
                // dependency on one that doesn't exist.
                @class.AddMethod("void", "ApplyErrorHandlingPolicy", method =>
                {
                    method.Static();
                    method.AddParameter("WolverineOptions", "opts");
                    method.AddParameter("IConfiguration", "configuration");

                    switch (_errorHandlingPolicy.AsEnum())
                    {
                        case WolverineMessageBusSettings.ErrorHandlingPolicyOptionsEnum.None:
                            method.AddStatement("opts.OnException<Exception>().MoveToErrorQueue();");
                            break;
                        case WolverineMessageBusSettings.ErrorHandlingPolicyOptionsEnum.Retry:
                            method.AddStatement("""var attempts = int.Parse(configuration["Wolverine:ErrorHandling:Retry:Attempts"] ?? "3");""");
                            method.AddStatement("opts.OnException<Exception>().RetryTimes(attempts).Then.MoveToErrorQueue();", s => s.SeparatedFromPrevious());
                            break;
                        case WolverineMessageBusSettings.ErrorHandlingPolicyOptionsEnum.RetryWithCooldown:
                            method.AddStatement("""var delays = ParseDelays(configuration["Wolverine:ErrorHandling:RetryWithCooldown:Delays"] ?? "00:00:01, 00:00:05, 00:00:15");""");
                            method.AddStatement(
                                """
                                if (delays.Length == 0)
                                {
                                opts.OnException<Exception>().MoveToErrorQueue();
                                }
                                else
                                {
                                opts.OnException<Exception>().RetryWithCooldown(delays).Then.MoveToErrorQueue();
                                }
                                """,
                                s => s.SeparatedFromPrevious());
                            break;
                        case WolverineMessageBusSettings.ErrorHandlingPolicyOptionsEnum.ScheduleRetry:
                            method.AddStatement("""var delays = ParseDelays(configuration["Wolverine:ErrorHandling:ScheduleRetry:Delays"] ?? "00:01:00, 00:05:00, 00:15:00");""");
                            method.AddStatement(
                                """
                                if (delays.Length == 0)
                                {
                                opts.OnException<Exception>().MoveToErrorQueue();
                                }
                                else
                                {
                                opts.OnException<Exception>().ScheduleRetry(delays).Then.MoveToErrorQueue();
                                }
                                """,
                                s => s.SeparatedFromPrevious());
                            break;
                    }
                });

                @class.AddMethod("System.TimeSpan[]", "ParseDelays", method =>
                {
                    method.Static();
                    method.AddParameter("string", "value");
                    method.AddStatement(
                        """
                        return value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(TimeSpan.Parse)
                        .ToArray();
                        """);
                });

                // R6.3: the five Durable registrations, host-scope, called once by every
                // Configure{Transport} method via AddTransactionalOutboxHook - mirrors the
                // ApplyErrorHandlingPolicy seam above. Only emitted when Transactional Outbox is Durable.
                if (_transactionalOutbox.IsDurable())
                {
                    @class.AddMethod("void", "ApplyTransactionalOutbox", method =>
                    {
                        method.Static();
                        method.AddParameter("WolverineOptions", "opts");
                        method.AddParameter("IConfiguration", "configuration");

                        method.AddStatement($$"""var connectionString = configuration.GetConnectionString("{{_connectionStringName}}");""");

                        var persistStatement = _databaseProvider!.IsSqlServer()
                            ? "opts.PersistMessagesWithSqlServer(connectionString);"
                            : "opts.PersistMessagesWithPostgresql(connectionString);";
                        method.AddStatement(persistStatement, s => s.SeparatedFromPrevious());
                        method.AddStatement("opts.UseEntityFrameworkCoreTransactions();");
                        method.AddStatement("opts.Policies.AutoApplyTransactions();");
                        method.AddStatement("opts.Policies.UseDurableOutboxOnAllSendingEndpoints();");
                        method.AddStatement("opts.Policies.UseDurableInboxOnAllListeners();");
                    });
                }
            });
        }

        /// <summary>
        /// The name of the single <c>Configure{Transport}(WolverineOptions, IConfiguration)</c>
        /// method this template emits for the currently selected Transport module setting. Shared
        /// with <c>WolverineEventingRegistrationExtension</c> so the host-configuration call it
        /// contributes always matches the method this template actually generated.
        /// </summary>
        public static string GetConfigureMethodName(WolverineMessageBusSettings.TransportOptionsEnum transport)
        {
            return transport switch
            {
                WolverineMessageBusSettings.TransportOptionsEnum.Local => "ConfigureLocal",
                WolverineMessageBusSettings.TransportOptionsEnum.Rabbitmq => "ConfigureRabbitMq",
                WolverineMessageBusSettings.TransportOptionsEnum.AzureServiceBus => "ConfigureAzureServiceBus",
                WolverineMessageBusSettings.TransportOptionsEnum.AmazonSqs => "ConfigureAmazonSqs",
                _ => throw new System.InvalidOperationException($"Unsupported transport type: {transport}")
            };
        }

        // Transport = Local. In-process only, no external broker. Wolverine already defaults every
        // message to a local, in-process queue when no other routing is configured, so there is no
        // transport-specific listener/exchange-binding plumbing to wire up here, unlike the other
        // Configure{Transport} methods.
        // Bugfix: this method still needs AddHandlerTypeRegistrations though - Wolverine's default
        // Discovery only scans the entry (.Api) assembly, and a subscribed handler lives in the
        // .Application assembly. Every other transport gets this for free as the last statement
        // inside AddListenerRules; Local has no listener rules to piggyback on, so it must be called
        // directly. Without it, a Local-transport handler is silently never invoked - Wolverine logs
        // "found no handlers" at startup and every message logs "No known handler" instead of being
        // dispatched, with no build-time or Software-Factory-time signal that anything is wrong.
        // The IConfiguration parameter is kept even though unused, so every Configure{Transport}
        // method shares one call shape at the host call-site.
        private void AddConfigureLocal(CSharpClass @class)
        {
            @class.AddMethod("void", "ConfigureLocal", method =>
            {
                method.Static();
                method.AddParameter("WolverineOptions", "opts");
                method.AddParameter("IConfiguration", "configuration");

                AddPublishAndSendRules(method, "ToLocalQueue", "ToLocalQueue");
                AddHandlerTypeRegistrations(method);
                AddTransactionalOutboxHook(method);
                AddErrorHandlingPolicyHook(method);
            });
        }

        // Transport = RabbitMQ. Reproduces the Golden Sample's ConfigureTransport shape
        // (Tests/WolverineEventing.Publish.RabbitMQ.../Eventing/WolverineEventingConfiguration.cs),
        // generalized to read the host/port/credentials from configuration with the same defaults.
        // Publish rules, listeners and error-handling policy are out of scope for this wave (they
        // depend on message designation and land in waves 4-6) - only the transport/topology
        // plumbing is emitted here.
        private void AddConfigureRabbitMq(CSharpClass @class)
        {
            @class.AddMethod("void", "ConfigureRabbitMq", method =>
            {
                method.Static();
                method.AddParameter("WolverineOptions", "opts");
                method.AddParameter("IConfiguration", "configuration");

                method.AddStatement("""var section = configuration.GetSection("Wolverine:RabbitMq");""");
                method.AddStatement("""var host = section["Host"] ?? "localhost";""");
                method.AddStatement("""var port = int.Parse(section["Port"] ?? "5672");""");
                method.AddStatement("""var virtualHost = section["VirtualHost"] ?? "/";""");
                method.AddStatement("""var username = section["Username"] ?? "guest";""");
                method.AddStatement("""var password = section["Password"] ?? "guest";""");

                method.AddStatement(
                    """
                    var transport = opts.UseRabbitMq(rabbit =>
                    {
                    rabbit.HostName = host;
                    rabbit.Port = port;
                    rabbit.VirtualHost = virtualHost;
                    rabbit.UserName = username;
                    rabbit.Password = password;
                    });
                    """,
                    s => s.SeparatedFromPrevious());

                method.AddStatement("transport.AutoProvision();", s => s.SeparatedFromPrevious());

                AddPublishAndSendRules(method, "ToRabbitExchange", "ToRabbitQueue");
                AddListenerRules(method, "ListenToRabbitQueue", supportsExchangeBinding: true);
                AddTransactionalOutboxHook(method);
                AddErrorHandlingPolicyHook(method);
            });
        }

        // Transport = Azure Service Bus. Call shapes cited verbatim from
        // intent/.specs/wolverine-eventing-module/golden-sample/probes/DurableAndTransportProbe/Probe.cs
        // (TransportProbe.AzureServiceBus and FailFastConfigurationProbe.AzureServiceBusConnectionString),
        // parameterized rather than hardcoded. Publish/listen rules are out of scope this wave.
        private void AddConfigureAzureServiceBus(CSharpClass @class)
        {
            @class.AddMethod("void", "ConfigureAzureServiceBus", method =>
            {
                method.Static();
                method.AddParameter("WolverineOptions", "opts");
                method.AddParameter("IConfiguration", "configuration");

                method.AddStatement("""const string section = "Wolverine:AzureServiceBus";""");
                method.AddStatement("""const string key = "ConnectionString";""");
                method.AddStatement("""var connectionString = configuration[$"{section}:{key}"];""");
                method.AddStatement(
                    """
                    if (string.IsNullOrEmpty(connectionString))
                    {
                    throw new System.InvalidOperationException($"Configuration key '{key}' in section '{section}' is required when Transport is Azure Service Bus.");
                    }
                    """,
                    s => s.SeparatedFromPrevious());

                method.AddStatement("var transport = opts.UseAzureServiceBus(connectionString);", s => s.SeparatedFromPrevious());

                method.AddStatement("transport.AutoProvision();", s => s.SeparatedFromPrevious());

                AddPublishAndSendRules(method, "ToAzureServiceBusTopic", "ToAzureServiceBusQueue");
                AddListenerRules(method, "ListenToAzureServiceBusQueue", supportsExchangeBinding: false);
                AddTransactionalOutboxHook(method);
                AddErrorHandlingPolicyHook(method);
            });
        }

        // Transport = Amazon SQS. Call shapes cited verbatim from the same probe
        // (TransportProbe.AmazonSqs and FailFastConfigurationProbe.AmazonSqsRegion), parameterized
        // rather than hardcoded. Publish/listen rules are out of scope this wave.
        private void AddConfigureAmazonSqs(CSharpClass @class)
        {
            @class.AddMethod("void", "ConfigureAmazonSqs", method =>
            {
                method.Static();
                method.AddParameter("WolverineOptions", "opts");
                method.AddParameter("IConfiguration", "configuration");

                method.AddStatement("""const string section = "Wolverine:AmazonSqs";""");
                method.AddStatement("""const string key = "Region";""");
                method.AddStatement("""var region = configuration[$"{section}:{key}"];""");
                method.AddStatement(
                    """
                    if (string.IsNullOrEmpty(region))
                    {
                    throw new System.InvalidOperationException($"Configuration key '{key}' in section '{section}' is required when Transport is Amazon SQS.");
                    }
                    """,
                    s => s.SeparatedFromPrevious());

                // R2.2: AccessKey/SecretKey are optional - omitted means the AWS default credential chain.
                method.AddStatement("""var accessKey = configuration[$"{section}:AccessKey"];""", s => s.SeparatedFromPrevious());
                method.AddStatement("""var secretKey = configuration[$"{section}:SecretKey"];""");

                method.AddStatement("var transport = opts.UseAmazonSqsTransport(config => config.RegionEndpoint = RegionEndpoint.GetBySystemName(region));", s => s.SeparatedFromPrevious());
                method.AddStatement(
                    """
                    if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
                    {
                    transport.Credentials(new BasicAWSCredentials(accessKey, secretKey));
                    }
                    """,
                    s => s.SeparatedFromPrevious());

                method.AddStatement("transport.AutoProvision();", s => s.SeparatedFromPrevious());

                // R3.1: SNS carries the Integration Event fan-out, SQS the point-to-point commands
                // and the subscriber queues. Only registered when this application actually deals in
                // Integration Events - an application that only sends/receives Integration Commands
                // needs no SNS topics at all.
                if (_publishedMessages.Count > 0 || _subscribedMessages.Count > 0)
                {
                    method.AddStatement("var snsTransport = opts.UseAmazonSnsTransport(config => config.RegionEndpoint = RegionEndpoint.GetBySystemName(region));", s => s.SeparatedFromPrevious());
                    method.AddStatement(
                        """
                        if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
                        {
                        snsTransport.Credentials(new BasicAWSCredentials(accessKey, secretKey));
                        }
                        """,
                        s => s.SeparatedFromPrevious());
                    method.AddStatement("snsTransport.AutoProvision();", s => s.SeparatedFromPrevious());
                }

                AddSqsPublishAndSendRules(method);
                AddListenerRules(method, "ListenToSqsQueue", supportsExchangeBinding: false);
                AddTransactionalOutboxHook(method);
                AddErrorHandlingPolicyHook(method);
            });
        }


        /// <summary>
        /// R3.1 for Transport = Amazon SQS. SQS is a point-to-point queue service and
        /// <c>WolverineFx.AmazonSqs</c> exposes no fan-out publish expression at all (its only
        /// publish targets are <c>ToSqsQueue</c>/<c>ToSqsQueueOnNamedBroker</c>/sharded queues), so an
        /// Integration Event's publish rule targets an <b>SNS topic</b> via <c>WolverineFx.AmazonSns</c>'s
        /// <c>ToSnsTopic</c> - SNS-to-SQS being AWS's fan-out primitive. Integration Commands stay
        /// point-to-point on <c>ToSqsQueue</c>, exactly as before.
        /// <para>
        /// The subscriber-side topic-to-queue subscription is chained onto the same rule via
        /// <c>SubscribeSqsQueue</c>. That is the only place Wolverine exposes it - there is no
        /// standalone binding API on the SNS transport expression the way RabbitMQ has
        /// <c>transport.BindExchange(...).ToQueue(...)</c> - so a subscribing application declares its
        /// own subscription through a publish expression for the event it subscribes to. The routing
        /// rule that produces is inert in a subscriber that never publishes that event.
        /// </para>
        /// An event that is both published and subscribed by the same application emits ONE
        /// <c>ToSnsTopic</c> rule with the subscription chained onto it, not two competing rules for
        /// the same topic.
        /// </summary>
        private void AddSqsPublishAndSendRules(CSharpClassMethod method)
        {
            var isFirstStatement = true;
            var subscribedIds = _subscribedMessages.Select(x => x.Id).ToHashSet();
            var publishedIds = _publishedMessages.Select(x => x.Id).ToHashSet();

            void AddStatement(string statement)
            {
                var separateFromPrevious = isFirstStatement;
                isFirstStatement = false;
                method.AddStatement(statement, s => { if (separateFromPrevious) s.SeparatedFromPrevious(); });
            }

            foreach (var message in _publishedMessages)
            {
                var typeName = UseType(GetFullyQualifiedTypeName(IntegrationEventMessageTemplate.TemplateId, message));
                var subscription = subscribedIds.Contains(message.Id)
                    ? $".SubscribeSqsQueue(\"{GetSubscriberQueueName(message)}\")"
                    : string.Empty;
                AddStatement($"opts.PublishMessage<{typeName}>().ToSnsTopic(\"{ResolvePublishName(message)}\"){subscription};");
            }

            foreach (var message in _subscribedMessages.Where(x => !publishedIds.Contains(x.Id)))
            {
                var typeName = UseType(GetFullyQualifiedTypeName(IntegrationEventMessageTemplate.TemplateId, message));
                AddStatement($"opts.PublishMessage<{typeName}>().ToSnsTopic(\"{ResolvePublishName(message)}\").SubscribeSqsQueue(\"{GetSubscriberQueueName(message)}\");");
            }

            foreach (var command in _sentCommands)
            {
                var typeName = UseType(GetFullyQualifiedTypeName(IntegrationCommandTemplate.TemplateId, command));
                AddStatement($"opts.PublishMessage<{typeName}>().ToSqsQueue(\"{ResolveSendName(command)}\");");
            }
        }

        /// <summary>
        /// Emits one Wolverine publish rule per Wolverine-designated published Integration Event
        /// (using <paramref name="publishRuleMethodName"/>, e.g. <c>ToRabbitExchange</c>) and one
        /// point-to-point send rule per Wolverine-designated sent Integration Command (using
        /// <paramref name="sendRuleMethodName"/>, e.g. <c>ToRabbitQueue</c>). Two messages resolving
        /// to the same name both emit without error (R3.7/R4.7) - WolverineOptions allows this, so
        /// no dedup/guard logic is needed here.
        /// </summary>
        private void AddPublishAndSendRules(CSharpClassMethod method, string publishRuleMethodName, string sendRuleMethodName)
        {
            var isFirstStatement = true;

            foreach (var message in _publishedMessages)
            {
                var typeName = UseType(GetFullyQualifiedTypeName(IntegrationEventMessageTemplate.TemplateId, message));
                var name = ResolvePublishName(message);
                var separateFromPrevious = isFirstStatement;
                isFirstStatement = false;
                method.AddStatement($"opts.PublishMessage<{typeName}>().{publishRuleMethodName}(\"{name}\");",
                    s => { if (separateFromPrevious) s.SeparatedFromPrevious(); });
            }

            foreach (var command in _sentCommands)
            {
                var typeName = UseType(GetFullyQualifiedTypeName(IntegrationCommandTemplate.TemplateId, command));
                var name = ResolveSendName(command);
                var separateFromPrevious = isFirstStatement;
                isFirstStatement = false;
                method.AddStatement($"opts.PublishMessage<{typeName}>().{sendRuleMethodName}(\"{name}\");",
                    s => { if (separateFromPrevious) s.SeparatedFromPrevious(); });
            }
        }

        /// <summary>
        /// R5.1/R5.7/R5.8: emits one listener per Wolverine-designated subscribed message. An
        /// Integration Event listens on its Subscriber Queue name (this application's name and
        /// the message's name, each kebab-cased) and, under Auto-provision only, first gets an
        /// explicit exchange-to-queue binding so the fan-out exchange a publisher's
        /// PublishMessage rule declares actually reaches this queue - Externally owned never
        /// declares that binding, matching the transport's own topology branch (R2.7). An
        /// Integration Command listens on its Destination Queue Name unchanged, with no
        /// application-name prefix and no binding, since that point-to-point queue is shared by
        /// design and the publisher already sends directly to it. No Handler Type Registration
        /// is emitted here - the IIntegrationEventHandler&lt;T&gt; is reachable through the
        /// host's conventional discovery (D1/d-a1), never registered per message, which is also
        /// why R5.2's "no intermediate consumer class" holds without any extra guard here.
        /// </summary>
        private void AddListenerRules(CSharpClassMethod method, string listenMethodName, bool supportsExchangeBinding)
        {
            foreach (var message in _subscribedMessages)
            {
                var queueName = GetSubscriberQueueName(message);

                if (supportsExchangeBinding)
                {
                    var exchangeName = ResolvePublishName(message);
                    method.AddStatement($"transport.BindExchange(\"{exchangeName}\").ToQueue(\"{queueName}\");", s => s.SeparatedFromPrevious());
                }

                method.AddStatement($"opts.{listenMethodName}(\"{queueName}\");", s => s.SeparatedFromPrevious());
            }

            foreach (var command in _receivedCommands)
            {
                var queueName = ResolveSendName(command);
                method.AddStatement($"opts.{listenMethodName}(\"{queueName}\");", s => s.SeparatedFromPrevious());
            }

            AddHandlerTypeRegistrations(method);
        }

        /// <summary>
        /// R5.1/R5.5: one explicit Handler Type Registration naming the
        /// <c>IIntegrationEventHandler&lt;T&gt;</c> implementation for this application's
        /// Wolverine-designated subscriptions, emitted alongside the listeners of
        /// <see cref="AddListenerRules"/> and at the same host scope.
        /// <para>
        /// Emitted per handler TYPE, not per subscribed message: one generated handler class
        /// implements <c>IIntegrationEventHandler&lt;T&gt;</c> once per subscription it carries, so
        /// several subscribed messages routinely resolve to the same class. Deduplicating by type
        /// name is what makes R5.5's "exactly one, however many times it is re-run, and no duplicate
        /// when the same message is subscribed to more than once" hold - the statement set is a pure
        /// function of the distinct handler types, so it is idempotent by construction.
        /// </para>
        /// <para>
        /// R18.3: only the Integration Event Handlers of THIS module's subscribe-side surface are
        /// registered. The CQRS handler types the Golden Sample's publisher registers belong to
        /// <c>Intent.Application.Wolverine</c> and are deliberately not reachable from here - the
        /// handler models are read from this application's own Integration Event Handlers and
        /// filtered to Wolverine-designated messages only.
        /// </para>
        /// <para>
        /// Conventional discovery stays ON (owned by <c>Intent.Wolverine.Common</c>) and is NOT
        /// disabled, so a handler in the application assembly is reachable both ways. Verified
        /// against WolverineFx 5.39.5 that this does not double-register: with
        /// <c>Discovery.IncludeAssembly(entry)</c> and <c>Discovery.IncludeType&lt;T&gt;()</c> both
        /// present, the message's handler chain still holds exactly one HandlerCall for that
        /// type/method and the handler pipeline generates and executes cleanly. See CONTEXT.md.
        /// </para>
        /// </summary>
        private void AddHandlerTypeRegistrations(CSharpClassMethod method)
        {
            var emitted = new HashSet<string>();

            foreach (var handler in _subscribedHandlers)
            {
                var handlerTypeName = UseType(GetFullyQualifiedTypeName(IntegrationEventHandlerTemplate.TemplateId, handler));
                if (!emitted.Add(handlerTypeName))
                {
                    continue;
                }

                method.AddStatement($"opts.Discovery.IncludeType<{handlerTypeName}>();", s => s.SeparatedFromPrevious());
            }
        }

        /// <summary>
        /// This application's Integration Event Handlers that carry at least one Wolverine-designated
        /// Integration Event or Integration Command subscription. Mirrors the model-selection shape
        /// the sibling consumer templates use (e.g. Aws.Lambda.Functions.Sqs's
        /// <c>LambdaFunctionConsumerTemplateRegistration.GetModels</c>), so a handler whose
        /// subscriptions all belong to another broker is never registered here.
        /// </summary>
        private IReadOnlyList<IntegrationEventHandlerModel> GetWolverineDesignatedSubscribedHandlers()
        {
            var allHandlers = ExecutionContext.MetadataManager
                .Services(ExecutionContext.GetApplicationConfig().Id)
                .GetIntegrationEventHandlerModels();

            return allHandlers
                .Where(handler =>
                    this.GetWolverineDesignatedMessages(handler.IntegrationEventSubscriptions()
                        .Select(subscription => subscription.TypeReference.Element.AsMessageModel()))
                        .Any()
                    || this.GetWolverineDesignatedIntegrationCommands(handler.IntegrationCommandSubscriptions()
                        .Select(subscription => subscription.TypeReference.Element.AsIntegrationCommandModel()))
                        .Any())
                .OrderBy(handler => handler.Name)
                .ToList();
        }

        /// <summary>
        /// R5.7/R5.9: the subscribing application's name and the message's name, each
        /// kebab-cased and joined by a hyphen. Never overridable - no stereotype property exists
        /// for it - so every subscriber of a fanned-out Integration Event ends up with its own
        /// queue rather than sharing one.
        /// </summary>
        private string GetSubscriberQueueName(MessageModel message)
        {
            var messageNameKebab = GetConventionName(GetFullyQualifiedTypeName(IntegrationEventMessageTemplate.TemplateId, message));
            return $"{_applicationNameKebab}-{messageNameKebab}";
        }

        /// <summary>
        /// R5.6/R5.8: reserves the host-scope seam wave 6's Error Handling Policy emission
        /// writes into. Called exactly once per Configure{Transport} method - i.e. once per
        /// host, never per-listener - so whatever policy lands in <see cref="ApplyErrorHandlingPolicy"/>
        /// covers every listener configured above it.
        /// </summary>
        private static void AddErrorHandlingPolicyHook(CSharpClassMethod method)
        {
            method.AddStatement("ApplyErrorHandlingPolicy(opts, configuration);", s => s.SeparatedFromPrevious());
        }

        /// <summary>
        /// R6.3: calls <c>ApplyTransactionalOutbox</c> once per Configure{Transport} method, same
        /// host-scope seam shape as <see cref="AddErrorHandlingPolicyHook"/>. No-op (the method
        /// itself is never emitted) when Transactional Outbox is not Durable.
        /// </summary>
        private void AddTransactionalOutboxHook(CSharpClassMethod method)
        {
            if (!_transactionalOutbox.IsDurable())
            {
                return;
            }

            method.AddStatement("ApplyTransactionalOutbox(opts, configuration);", s => s.SeparatedFromPrevious());
        }

        /// <summary>
        /// Resolution order (R3.3/R3.4): a non-null Topic Name override is used verbatim (no
        /// kebab-casing) once validated - otherwise the kebab-case convention name of the
        /// generated message type's simple name is used.
        /// </summary>
        private string ResolvePublishName(MessageModel message)
        {
            var topicName = message.GetWolverineMessage()?.TopicName();
            if (topicName != null)
            {
                ValidateNameOverride(topicName, message.InternalElement, message.Name, "Topic Name");
                return topicName;
            }

            return GetConventionName(GetFullyQualifiedTypeName(IntegrationEventMessageTemplate.TemplateId, message));
        }

        /// <summary>
        /// Resolution order (R4.1/R4.2): a non-null Destination Queue Name override (declared on
        /// the Integration Command element itself, per D4 - not on any send association) is used
        /// verbatim once validated - otherwise the kebab-case convention name of the generated
        /// command type's simple name is used.
        /// </summary>
        private string ResolveSendName(IntegrationCommandModel command)
        {
            var queueName = command.GetWolverineMessage()?.DestinationQueueName();
            if (queueName != null)
            {
                ValidateNameOverride(queueName, command.InternalElement, command.Name, "Destination Queue Name");
                return queueName;
            }

            return GetConventionName(GetFullyQualifiedTypeName(IntegrationCommandTemplate.TemplateId, command));
        }

        /// <summary>
        /// R3.5/R3.6 (and the Command Distribution equivalent, R4.7's sibling requirements): an
        /// override that is present but empty/whitespace-only after trim, or longer than 250
        /// characters, is an error against the offending element - the convention name is never
        /// substituted for an invalid override.
        /// </summary>
        private static void ValidateNameOverride(string value, ICanBeReferencedType element, string elementName, string settingName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ElementException(element,
                    $"{settingName} on '{elementName}' is set but is empty or contains only whitespace. " +
                    $"Either remove the {settingName} override so the convention name is used, or set it to a non-blank value.");
            }

            if (value.Length > 250)
            {
                throw new ElementException(element,
                    $"{settingName} on '{elementName}' is {value.Length} characters long, which exceeds the 250 character limit. " +
                    $"Shorten the {settingName} value to 250 characters or fewer.");
            }
        }

        private static string GetConventionName(string fullyQualifiedTypeName)
        {
            var simpleName = fullyQualifiedTypeName.Split('.').Last();
            return simpleName.ToKebabCase();
        }

        public override void BeforeTemplateExecution()
        {
            PublishAppSettings();
        }

        /// <summary>
        /// Registers default appsettings.json entries for the connection settings the generated
        /// ConfigureX method above reads. Registration is additive only - there is no API to remove
        /// a previously-registered key, so uninstalling this module (or switching Transport) leaves
        /// any keys it once registered behind for the developer to clean up by hand.
        /// </summary>
        private void PublishAppSettings()
        {
            switch (_transport.AsEnum())
            {
                case WolverineMessageBusSettings.TransportOptionsEnum.Rabbitmq:
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:RabbitMq:Host", "localhost"));
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:RabbitMq:Port", "5672"));
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:RabbitMq:VirtualHost", "/"));
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:RabbitMq:Username", "guest"));
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:RabbitMq:Password", "guest"));
                    break;
                case WolverineMessageBusSettings.TransportOptionsEnum.AzureServiceBus:
                    // R2.3/R2.6: this key has no default - it is left as an empty placeholder so the
                    // generated reader's fail-fast guard in AddConfigureAzureServiceBus can actually fire.
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:AzureServiceBus:ConnectionString", ""));
                    break;
                case WolverineMessageBusSettings.TransportOptionsEnum.AmazonSqs:
                    // R2.3/R2.6: Region has no default and is left empty for the same reason. AccessKey
                    // and SecretKey are genuinely optional (R2.2) - omitted means the AWS default
                    // credential chain - so they are also registered empty rather than invented.
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:AmazonSqs:Region", ""));
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:AmazonSqs:AccessKey", ""));
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:AmazonSqs:SecretKey", ""));
                    break;
            }

            switch (_errorHandlingPolicy.AsEnum())
            {
                case WolverineMessageBusSettings.ErrorHandlingPolicyOptionsEnum.Retry:
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:ErrorHandling:Retry:Attempts", "3"));
                    break;
                case WolverineMessageBusSettings.ErrorHandlingPolicyOptionsEnum.RetryWithCooldown:
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:ErrorHandling:RetryWithCooldown:Delays", "00:00:01, 00:00:05, 00:00:15"));
                    break;
                case WolverineMessageBusSettings.ErrorHandlingPolicyOptionsEnum.ScheduleRetry:
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:ErrorHandling:ScheduleRetry:Delays", "00:01:00, 00:05:00, 00:15:00"));
                    break;
            }
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
