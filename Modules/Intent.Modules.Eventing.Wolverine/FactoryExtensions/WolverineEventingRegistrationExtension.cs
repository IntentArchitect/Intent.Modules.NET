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
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationCommand;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventHandler;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.Modules.Eventing.Wolverine.Settings;
using Intent.Modules.Eventing.Wolverine.Templates;
using Intent.Modules.Eventing.Wolverine.Templates.WolverineCompositeConfiguration;
using Intent.Modules.Eventing.Wolverine.Templates.WolverineMessageBus;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.Wolverine.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class WolverineEventingRegistrationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.Wolverine.WolverineEventingRegistrationExtension";

        // Decides where ConfigureEventing's call statement lands inside Intent.Wolverine.Common's
        // shared Configure method body - after Intent.Application.Wolverine's ConfigureCqrs (Order 10).
        // This module no longer touches Program.cs at all; see ContributeEventingConfiguration below.
        [IntentManaged(Mode.Ignore)]
        public override int Order => 20;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            ContributeEventingConfiguration(application);
            RegisterWolverineMessageBus(application);
        }

        /// <summary>
        /// R3.9: the bus is only registered against the Contracts <c>IMessageBus</c> interface when
        /// this application has at least one Wolverine-designated published Integration Event or sent
        /// Integration Command - a subscribe-only application gets no registration.
        /// <para>
        /// Composite Message Bus apps are excluded here: <see cref="WolverineMessageBusInteropExtension"/>'s
        /// <c>MessageBusRegistry.Register(...)</c> is a design-time registry that only tells the
        /// Software Factory Wolverine is one of several installed providers (which is what makes
        /// <c>RequiresCompositeMessageBus()</c> return true) - it is unrelated to the RUNTIME
        /// <c>MessageBrokerRegistry</c> a composite app routes messages through. That runtime piece
        /// is <see cref="WolverineCompositeConfiguration.WolverineCompositeConfigurationTemplate"/>'s
        /// job: in composite mode <c>WolverineMessageBus</c> must NOT be registered directly against
        /// the shared bus interface (that registration belongs to <c>CompositeMessageBus</c> itself),
        /// only against its own concrete type, which that template does via
        /// <c>services.AddScoped&lt;WolverineMessageBus&gt;()</c>.
        /// </para>
        /// </summary>
        private static void RegisterWolverineMessageBus(IApplication application)
        {
            var busTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(WolverineMessageBusTemplate.TemplateId);
            if (busTemplate == null)
            {
                return;
            }

            if (busTemplate.RequiresCompositeMessageBus())
            {
                return;
            }

            var hasPublishedMessages = busTemplate.ExecutionContext.MetadataManager
                .GetExplicitlyPublishedMessageModels(application)
                .FilterMessagesForThisMessageBroker(application, Intent.Modules.Eventing.Wolverine.Templates.Constants.BrokerStereotypeIds)
                .Any();

            var hasSentCommands = busTemplate.ExecutionContext.MetadataManager
                .GetExplicitlySentIntegrationCommandModels(application)
                .FilterMessagesForThisMessageBroker(application, Intent.Modules.Eventing.Wolverine.Templates.Constants.BrokerStereotypeIds)
                .Any();

            if (!hasPublishedMessages && !hasSentCommands)
            {
                return;
            }

            var busInterfaceTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(busTemplate.GetBusInterfaceTemplateId());
            if (busInterfaceTemplate == null)
            {
                return;
            }

            application.EventDispatcher.Publish(ContainerRegistrationRequest
                .ToRegister(busTemplate)
                .ForInterface(busInterfaceTemplate)
                .ForConcern("Infrastructure")
                .WithPerServiceCallLifeTime());
        }

        /// <summary>
        /// Finds Intent.Wolverine.Common's WolverineConfiguration template and contributes this
        /// module's eventing configuration to it: a private <c>ConfigureEventing</c> method (itself
        /// decomposed into <c>ConfigurePublishing</c>/<c>ConfigureListeners</c>/
        /// <c>Configure{Transport}Transport</c>, named after Wolverine's own vocabulary - its API
        /// return types are <c>PublishingExpression</c> and <c>ListenerConfiguration</c>, and the
        /// broker plumbing is a "Transport"), plus one call statement into the shared <c>Configure</c>
        /// method body. Same find-template + OnBuild + AddMethod/AddStatement idiom
        /// Intent.Eventing.MassTransit's FinbuckleConfiguratorExtension already uses on
        /// MassTransitConfigurationTemplate. This module takes no ProjectReference on
        /// Intent.Wolverine.Common, so its TemplateId is a string literal, not a compiled constant.
        /// </summary>
        private static void ContributeEventingConfiguration(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Wolverine.Common.WolverineConfiguration");
            if (template == null)
            {
                return;
            }

            var ctx = EventingContext.Build(template);

            template.AddTypeSource(IntegrationEventMessageTemplate.TemplateId);
            template.AddTypeSource(IntegrationCommandTemplate.TemplateId);

            // WolverineFx is the base package regardless of Transport; the transport-specific package
            // is only added for the Transport actually selected, never speculatively.
            template.AddNugetDependency(NugetPackages.WolverineFx(template.OutputTarget));
            switch (ctx.Transport)
            {
                case WolverineMessageBusSettings.TransportOptionsEnum.Local:
                    break;
                case WolverineMessageBusSettings.TransportOptionsEnum.Rabbitmq:
                    template.AddNugetDependency(NugetPackages.WolverineFxRabbitMQ(template.OutputTarget));
                    break;
                case WolverineMessageBusSettings.TransportOptionsEnum.AzureServiceBus:
                    template.AddNugetDependency(NugetPackages.WolverineFxAzureServiceBus(template.OutputTarget));
                    break;
                case WolverineMessageBusSettings.TransportOptionsEnum.AmazonSqs:
                    template.AddNugetDependency(NugetPackages.WolverineFxAmazonSqs(template.OutputTarget));
                    // R3.1: SQS is point-to-point only - SNS is the fan-out primitive an Integration
                    // Event's publish rule targets, and it ships as its own package.
                    template.AddNugetDependency(NugetPackages.WolverineFxAmazonSns(template.OutputTarget));
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported transport type: {ctx.Transport}");
            }

            if (ctx.TransactionalOutbox.IsDurable())
            {
                if (!template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.EntityFrameworkCore"))
                {
                    throw new FriendlyException(
                        "Transactional Outbox is set to Durable but the Intent.EntityFrameworkCore module is not installed. " +
                        "Wolverine's durable outbox shares the EF Core DbContext to guarantee messages are only sent once " +
                        "the related database transaction commits. Install Intent.EntityFrameworkCore, or change the " +
                        "Transactional Outbox setting to None.");
                }

                if (!ctx.DatabaseProvider.IsSupported())
                {
                    throw new FriendlyException(
                        $"Transactional Outbox is set to Durable but the configured Database Provider ('{ctx.DatabaseProvider.Value}') " +
                        "is not supported. Wolverine's durable outbox only supports SQL Server and PostgreSQL. Change the Database " +
                        "Provider to one of those, or change the Transactional Outbox setting to None.");
                }

                template.AddNugetDependency(NugetPackages.WolverineFxEntityFrameworkCore(template.OutputTarget));
                template.AddNugetDependency(ctx.DatabaseProvider.IsSqlServer()
                    ? NugetPackages.WolverineFxSqlServer(template.OutputTarget)
                    : NugetPackages.WolverineFxPostgresql(template.OutputTarget));
            }

            template.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("Wolverine.ErrorHandling");

                var transportUsing = ctx.Transport switch
                {
                    WolverineMessageBusSettings.TransportOptionsEnum.Rabbitmq => "Wolverine.RabbitMQ",
                    WolverineMessageBusSettings.TransportOptionsEnum.AzureServiceBus => "Wolverine.AzureServiceBus",
                    WolverineMessageBusSettings.TransportOptionsEnum.AmazonSqs => "Wolverine.AmazonSqs",
                    _ => null
                };
                if (transportUsing != null)
                {
                    file.AddUsing(transportUsing);
                }

                // RabbitMqTransportExpression (the return type of opts.UseRabbitMq(...), used as
                // ConfigureRabbitMqTransport's explicit return type below so ConfigureListeners can
                // take it as a typed parameter) is a public type, but WolverineFx 5.39.5 declares it
                // in a nested Internal namespace - "Wolverine.RabbitMQ" alone does not bring it into
                // scope.
                if (ctx.Transport == WolverineMessageBusSettings.TransportOptionsEnum.Rabbitmq)
                {
                    file.AddUsing("Wolverine.RabbitMQ.Internal");
                }

                if (ctx.Transport == WolverineMessageBusSettings.TransportOptionsEnum.AmazonSqs)
                {
                    // RegionEndpoint (Amazon.SQS's AmazonSQSConfig, which UseAmazonSqsTransport
                    // configures, derives from Amazon.Runtime.ClientConfig) and BasicAWSCredentials
                    // for the optional AccessKey/SecretKey pair (R2.2).
                    file.AddUsing("Amazon");
                    file.AddUsing("Amazon.Runtime");
                    // R3.1: ToSnsTopic/SubscribeSqsQueue live in Wolverine.AmazonSns, not Wolverine.AmazonSqs.
                    file.AddUsing("Wolverine.AmazonSns");
                }

                if (ctx.TransactionalOutbox.IsDurable())
                {
                    file.AddUsing("Wolverine.EntityFrameworkCore");
                    file.AddUsing(ctx.DatabaseProvider!.IsSqlServer() ? "Wolverine.SqlServer" : "Wolverine.Postgresql");
                }

                var @class = file.Classes.First();
                var configureMethod = @class.FindMethod("Configure");

                AddConfigureEventing(@class, configureMethod, ctx);
            });

            PublishAppSettings(application, ctx);
        }

        // ---------------------------------------------------------------------------------------
        // ConfigureEventing - the one method this module contributes to the shared Configure seam.
        // ---------------------------------------------------------------------------------------

        private static void AddConfigureEventing(CSharpClass @class, CSharpClassMethod configureMethod, EventingContext ctx)
        {
            @class.AddMethod("void", "ConfigureEventing", method =>
            {
                method.Private().Static();
                method.AddParameter("WolverineOptions", "opts");
                method.AddParameter("IConfiguration", "configuration");

                switch (ctx.Transport)
                {
                    case WolverineMessageBusSettings.TransportOptionsEnum.Local:
                        AddLocalBody(@class, method, ctx);
                        break;
                    case WolverineMessageBusSettings.TransportOptionsEnum.Rabbitmq:
                        AddRabbitMqBody(@class, method, ctx);
                        break;
                    case WolverineMessageBusSettings.TransportOptionsEnum.AzureServiceBus:
                        AddAzureServiceBusBody(@class, method, ctx);
                        break;
                    case WolverineMessageBusSettings.TransportOptionsEnum.AmazonSqs:
                        AddAmazonSqsBody(@class, method, ctx);
                        break;
                }

                AddApplyErrorHandlingPolicyMethod(@class, ctx);
                method.AddStatement("ApplyErrorHandlingPolicy(opts, configuration);", s => s.SeparatedFromPrevious());

                if (ctx.TransactionalOutbox.IsDurable())
                {
                    AddApplyTransactionalOutboxMethod(@class, ctx);
                    method.AddStatement("ApplyTransactionalOutbox(opts, configuration);", s => s.SeparatedFromPrevious());
                }
            });

            configureMethod.AddStatement("ConfigureEventing(opts, configuration);", s => s.SeparatedFromPrevious());
        }

        // Transport = Local. In-process only, no external broker. Wolverine already defaults every
        // message to a local, in-process queue when no other routing is configured, so there is no
        // transport-specific listener/exchange-binding plumbing and no Configure{Transport}Transport
        // method at all here, unlike the other three transports.
        private static void AddLocalBody(CSharpClass @class, CSharpClassMethod method, EventingContext ctx)
        {
            var hasPublishing = ctx.PublishedMessages.Count > 0 || ctx.SentCommands.Count > 0;
            if (hasPublishing)
            {
                AddConfigurePublishingMethod(@class, ctx, "ToLocalQueue", "ToLocalQueue");
                method.AddStatement("ConfigurePublishing(opts);", s => s.SeparatedFromPrevious());
            }

            // Bugfix carried forward: a Local-transport handler still needs an explicit
            // opts.Discovery.IncludeType<T>() - Wolverine's default Discovery only scans the entry
            // (.Api) assembly, and a subscribed handler lives in the .Application assembly. Without
            // it the app builds and starts cleanly but silently never invokes the handler.
            if (ctx.SubscribedHandlers.Count > 0)
            {
                @class.AddMethod("void", "ConfigureListeners", listeners =>
                {
                    listeners.Private().Static();
                    listeners.AddParameter("WolverineOptions", "opts");
                    AddHandlerTypeRegistrations(listeners, ctx);
                });
                method.AddStatement("ConfigureListeners(opts);", s => s.SeparatedFromPrevious());
            }
        }

        // Transport = RabbitMQ.
        private static void AddRabbitMqBody(CSharpClass @class, CSharpClassMethod method, EventingContext ctx)
        {
            @class.AddMethod("RabbitMqTransportExpression", "ConfigureRabbitMqTransport", transportMethod =>
            {
                transportMethod.Private().Static();
                transportMethod.AddParameter("WolverineOptions", "opts");
                transportMethod.AddParameter("IConfiguration", "configuration");

                transportMethod.AddStatement("""var section = configuration.GetSection("Wolverine:RabbitMq");""");
                transportMethod.AddStatement("""var host = section["Host"] ?? "localhost";""");
                transportMethod.AddStatement("""var port = int.Parse(section["Port"] ?? "5672");""");
                transportMethod.AddStatement("""var virtualHost = section["VirtualHost"] ?? "/";""");
                transportMethod.AddStatement("""var username = section["Username"] ?? "guest";""");
                transportMethod.AddStatement("""var password = section["Password"] ?? "guest";""");

                var rabbitLambda = new CSharpLambdaBlock("rabbit")
                    .AddStatement("rabbit.HostName = host;")
                    .AddStatement("rabbit.Port = port;")
                    .AddStatement("rabbit.VirtualHost = virtualHost;")
                    .AddStatement("rabbit.UserName = username;")
                    .AddStatement("rabbit.Password = password;");
                var useRabbitMq = new CSharpInvocationStatement("var transport = opts.UseRabbitMq")
                    .AddArgument(rabbitLambda)
                    .WithSemicolon();
                transportMethod.AddStatement(useRabbitMq, s => s.SeparatedFromPrevious());

                transportMethod.AddStatement("transport.AutoProvision();", s => s.SeparatedFromPrevious());
                transportMethod.AddStatement("return transport;", s => s.SeparatedFromPrevious());
            });

            var hasPublishing = ctx.PublishedMessages.Count > 0 || ctx.SentCommands.Count > 0;
            var hasListening = ctx.SubscribedMessages.Count > 0 || ctx.ReceivedCommands.Count > 0;

            method.AddStatement("var transport = ConfigureRabbitMqTransport(opts, configuration);");

            if (hasPublishing)
            {
                AddConfigurePublishingMethod(@class, ctx, "ToRabbitExchange", "ToRabbitQueue");
                method.AddStatement("ConfigurePublishing(opts);", s => s.SeparatedFromPrevious());
            }

            if (hasListening)
            {
                @class.AddMethod("void", "ConfigureListeners", listeners =>
                {
                    listeners.Private().Static();
                    listeners.AddParameter("WolverineOptions", "opts");
                    listeners.AddParameter("RabbitMqTransportExpression", "transport");

                    foreach (var message in ctx.SubscribedMessages)
                    {
                        var queueName = GetSubscriberQueueName(ctx, message);
                        var exchangeName = ResolvePublishName(ctx, message);
                        listeners.AddStatement($"transport.BindExchange(\"{exchangeName}\").ToQueue(\"{queueName}\");", s => s.SeparatedFromPrevious());
                        listeners.AddStatement($"opts.ListenToRabbitQueue(\"{queueName}\");", s => s.SeparatedFromPrevious());
                    }

                    foreach (var command in ctx.ReceivedCommands)
                    {
                        var queueName = ResolveSendName(ctx, command);
                        listeners.AddStatement($"opts.ListenToRabbitQueue(\"{queueName}\");", s => s.SeparatedFromPrevious());
                    }

                    AddHandlerTypeRegistrations(listeners, ctx);
                });
                method.AddStatement("ConfigureListeners(opts, transport);", s => s.SeparatedFromPrevious());
            }
        }

        // Transport = Azure Service Bus. No exchange-to-queue binding step - AzureServiceBusTransport
        // exposes no equivalent to RabbitMQ's transport.BindExchange, so ConfigureListeners here needs
        // no reference to the transport object at all.
        private static void AddAzureServiceBusBody(CSharpClass @class, CSharpClassMethod method, EventingContext ctx)
        {
            @class.AddMethod("void", "ConfigureAzureServiceBusTransport", transportMethod =>
            {
                transportMethod.Private().Static();
                transportMethod.AddParameter("WolverineOptions", "opts");
                transportMethod.AddParameter("IConfiguration", "configuration");

                transportMethod.AddStatement("""const string section = "Wolverine:AzureServiceBus";""");
                transportMethod.AddStatement("""const string key = "ConnectionString";""");
                transportMethod.AddStatement("""var connectionString = configuration[$"{section}:{key}"];""");
                transportMethod.AddStatement(
                    new CSharpIfStatement("string.IsNullOrEmpty(connectionString)")
                        .AddStatement("""throw new InvalidOperationException($"Configuration key '{key}' in section '{section}' is required when Transport is Azure Service Bus.");"""),
                    s => s.SeparatedFromPrevious());

                transportMethod.AddStatement("var transport = opts.UseAzureServiceBus(connectionString);", s => s.SeparatedFromPrevious());
                transportMethod.AddStatement("transport.AutoProvision();", s => s.SeparatedFromPrevious());
            });

            var hasPublishing = ctx.PublishedMessages.Count > 0 || ctx.SentCommands.Count > 0;
            var hasListening = ctx.SubscribedMessages.Count > 0 || ctx.ReceivedCommands.Count > 0;

            method.AddStatement("ConfigureAzureServiceBusTransport(opts, configuration);");

            if (hasPublishing)
            {
                AddConfigurePublishingMethod(@class, ctx, "ToAzureServiceBusTopic", "ToAzureServiceBusQueue");
                method.AddStatement("ConfigurePublishing(opts);", s => s.SeparatedFromPrevious());
            }

            if (hasListening)
            {
                @class.AddMethod("void", "ConfigureListeners", listeners =>
                {
                    listeners.Private().Static();
                    listeners.AddParameter("WolverineOptions", "opts");

                    foreach (var message in ctx.SubscribedMessages)
                    {
                        var queueName = GetSubscriberQueueName(ctx, message);
                        listeners.AddStatement($"opts.ListenToAzureServiceBusQueue(\"{queueName}\");", s => s.SeparatedFromPrevious());
                    }

                    foreach (var command in ctx.ReceivedCommands)
                    {
                        var queueName = ResolveSendName(ctx, command);
                        listeners.AddStatement($"opts.ListenToAzureServiceBusQueue(\"{queueName}\");", s => s.SeparatedFromPrevious());
                    }

                    AddHandlerTypeRegistrations(listeners, ctx);
                });
                method.AddStatement("ConfigureListeners(opts);", s => s.SeparatedFromPrevious());
            }
        }

        // Transport = Amazon SQS. SQS is point-to-point only - WolverineFx.AmazonSqs exposes no
        // fan-out publish expression, so an Integration Event's publish rule targets an SNS topic via
        // WolverineFx.AmazonSns instead (R3.1). The subscriber-side topic-to-queue subscription is
        // chained onto that SAME publish rule via SubscribeSqsQueue - there is no standalone binding
        // API the way RabbitMQ has transport.BindExchange - so ConfigureListeners here needs no
        // reference to either transport object; it only lists the point-to-point queues.
        private static void AddAmazonSqsBody(CSharpClass @class, CSharpClassMethod method, EventingContext ctx)
        {
            var needsSns = ctx.PublishedMessages.Count > 0 || ctx.SubscribedMessages.Count > 0;

            @class.AddMethod("void", "ConfigureAmazonSqsTransport", transportMethod =>
            {
                transportMethod.Private().Static();
                transportMethod.AddParameter("WolverineOptions", "opts");
                transportMethod.AddParameter("IConfiguration", "configuration");

                transportMethod.AddStatement("""const string section = "Wolverine:AmazonSqs";""");
                transportMethod.AddStatement("""const string key = "Region";""");
                transportMethod.AddStatement("""var region = configuration[$"{section}:{key}"];""");
                transportMethod.AddStatement(
                    new CSharpIfStatement("string.IsNullOrEmpty(region)")
                        .AddStatement("""throw new InvalidOperationException($"Configuration key '{key}' in section '{section}' is required when Transport is Amazon SQS.");"""),
                    s => s.SeparatedFromPrevious());

                // R2.2: AccessKey/SecretKey are optional - omitted means the AWS default credential chain.
                transportMethod.AddStatement("""var accessKey = configuration[$"{section}:AccessKey"];""", s => s.SeparatedFromPrevious());
                transportMethod.AddStatement("""var secretKey = configuration[$"{section}:SecretKey"];""");

                transportMethod.AddStatement("var transport = opts.UseAmazonSqsTransport(config => config.RegionEndpoint = RegionEndpoint.GetBySystemName(region));", s => s.SeparatedFromPrevious());
                transportMethod.AddStatement(
                    new CSharpIfStatement("!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey)")
                        .AddStatement("transport.Credentials(new BasicAWSCredentials(accessKey, secretKey));"),
                    s => s.SeparatedFromPrevious());
                transportMethod.AddStatement("transport.AutoProvision();", s => s.SeparatedFromPrevious());

                if (needsSns)
                {
                    transportMethod.AddStatement("var snsTransport = opts.UseAmazonSnsTransport(config => config.RegionEndpoint = RegionEndpoint.GetBySystemName(region));", s => s.SeparatedFromPrevious());
                    transportMethod.AddStatement(
                        new CSharpIfStatement("!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey)")
                            .AddStatement("snsTransport.Credentials(new BasicAWSCredentials(accessKey, secretKey));"),
                        s => s.SeparatedFromPrevious());
                    transportMethod.AddStatement("snsTransport.AutoProvision();", s => s.SeparatedFromPrevious());
                }
            });

            var hasPublishing = ctx.PublishedMessages.Count > 0 || ctx.SentCommands.Count > 0;
            var hasListening = ctx.SubscribedMessages.Count > 0 || ctx.ReceivedCommands.Count > 0;

            method.AddStatement("ConfigureAmazonSqsTransport(opts, configuration);");

            if (hasPublishing)
            {
                @class.AddMethod("void", "ConfigurePublishing", publishing =>
                {
                    publishing.Private().Static();
                    publishing.AddParameter("WolverineOptions", "opts");

                    var subscribedIds = ctx.SubscribedMessages.Select(x => x.Id).ToHashSet();
                    var publishedIds = ctx.PublishedMessages.Select(x => x.Id).ToHashSet();
                    var isFirst = true;

                    void AddStatement(string statement)
                    {
                        var separate = isFirst;
                        isFirst = false;
                        publishing.AddStatement(statement, s => { if (separate) s.SeparatedFromPrevious(); });
                    }

                    foreach (var message in ctx.PublishedMessages)
                    {
                        var typeName = ctx.Template.UseType(ctx.Template.GetFullyQualifiedTypeName(IntegrationEventMessageTemplate.TemplateId, message));
                        var subscription = subscribedIds.Contains(message.Id)
                            ? $".SubscribeSqsQueue(\"{GetSubscriberQueueName(ctx, message)}\")"
                            : string.Empty;
                        AddStatement($"opts.PublishMessage<{typeName}>().ToSnsTopic(\"{ResolvePublishName(ctx, message)}\"){subscription};");
                    }

                    foreach (var message in ctx.SubscribedMessages.Where(x => !publishedIds.Contains(x.Id)))
                    {
                        var typeName = ctx.Template.UseType(ctx.Template.GetFullyQualifiedTypeName(IntegrationEventMessageTemplate.TemplateId, message));
                        AddStatement($"opts.PublishMessage<{typeName}>().ToSnsTopic(\"{ResolvePublishName(ctx, message)}\").SubscribeSqsQueue(\"{GetSubscriberQueueName(ctx, message)}\");");
                    }

                    foreach (var command in ctx.SentCommands)
                    {
                        var typeName = ctx.Template.UseType(ctx.Template.GetFullyQualifiedTypeName(IntegrationCommandTemplate.TemplateId, command));
                        AddStatement($"opts.PublishMessage<{typeName}>().ToSqsQueue(\"{ResolveSendName(ctx, command)}\");");
                    }
                });
                method.AddStatement("ConfigurePublishing(opts);", s => s.SeparatedFromPrevious());
            }

            if (hasListening)
            {
                @class.AddMethod("void", "ConfigureListeners", listeners =>
                {
                    listeners.Private().Static();
                    listeners.AddParameter("WolverineOptions", "opts");

                    foreach (var command in ctx.ReceivedCommands)
                    {
                        var queueName = ResolveSendName(ctx, command);
                        listeners.AddStatement($"opts.ListenToSqsQueue(\"{queueName}\");", s => s.SeparatedFromPrevious());
                    }

                    AddHandlerTypeRegistrations(listeners, ctx);
                });
                method.AddStatement("ConfigureListeners(opts);", s => s.SeparatedFromPrevious());
            }
        }

        /// <summary>
        /// Shared by Local/RabbitMQ/AzureServiceBus (AmazonSqs builds its own publish rules via SNS -
        /// see AddAmazonSqsBody). Emits one publish rule per published Integration Event and one
        /// point-to-point send rule per sent Integration Command. Two messages resolving to the same
        /// name both emit without error (R3.7/R4.7) - no dedup/guard logic needed.
        /// </summary>
        private static void AddConfigurePublishingMethod(CSharpClass @class, EventingContext ctx, string publishRuleMethodName, string sendRuleMethodName)
        {
            @class.AddMethod("void", "ConfigurePublishing", method =>
            {
                method.Private().Static();
                method.AddParameter("WolverineOptions", "opts");

                var isFirst = true;

                foreach (var message in ctx.PublishedMessages)
                {
                    var typeName = ctx.Template.UseType(ctx.Template.GetFullyQualifiedTypeName(IntegrationEventMessageTemplate.TemplateId, message));
                    var name = ResolvePublishName(ctx, message);
                    var separate = isFirst;
                    isFirst = false;
                    method.AddStatement($"opts.PublishMessage<{typeName}>().{publishRuleMethodName}(\"{name}\");",
                        s => { if (separate) s.SeparatedFromPrevious(); });
                }

                foreach (var command in ctx.SentCommands)
                {
                    var typeName = ctx.Template.UseType(ctx.Template.GetFullyQualifiedTypeName(IntegrationCommandTemplate.TemplateId, command));
                    var name = ResolveSendName(ctx, command);
                    var separate = isFirst;
                    isFirst = false;
                    method.AddStatement($"opts.PublishMessage<{typeName}>().{sendRuleMethodName}(\"{name}\");",
                        s => { if (separate) s.SeparatedFromPrevious(); });
                }
            });
        }

        /// <summary>
        /// R5.1/R5.5: one explicit Handler Type Registration naming the
        /// IIntegrationEventHandler&lt;T&gt; implementation for this application's Wolverine-designated
        /// subscriptions. Emitted per handler TYPE, not per subscribed message, and deduplicated by
        /// type name so re-running is idempotent. Conventional discovery stays ON (owned by
        /// Intent.Wolverine.Common) and is not disabled, so a handler in the application assembly is
        /// reachable both ways - verified this does not double-register (see CONTEXT.md).
        /// </summary>
        private static void AddHandlerTypeRegistrations(CSharpClassMethod method, EventingContext ctx)
        {
            var emitted = new HashSet<string>();

            foreach (var handler in ctx.SubscribedHandlers)
            {
                var handlerTypeName = ctx.Template.UseType(ctx.Template.GetFullyQualifiedTypeName(IntegrationEventHandlerTemplate.TemplateId, handler));
                if (!emitted.Add(handlerTypeName))
                {
                    continue;
                }

                method.AddStatement($"opts.Discovery.IncludeType<{handlerTypeName}>();", s => s.SeparatedFromPrevious());
            }
        }

        /// <summary>
        /// R5.6/R5.8: the Error Handling Policy seam, called exactly once per host from
        /// ConfigureEventing - covers every listener configured above it.
        /// </summary>
        private static void AddApplyErrorHandlingPolicyMethod(CSharpClass @class, EventingContext ctx)
        {
            @class.AddMethod("void", "ApplyErrorHandlingPolicy", method =>
            {
                method.Private().Static();
                method.AddParameter("WolverineOptions", "opts");
                method.AddParameter("IConfiguration", "configuration");

                switch (ctx.ErrorHandlingPolicy.AsEnum())
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
                        AddDelayBranch(method, "RetryWithCooldown");
                        AddParseDelaysMethod(@class, ctx.ParseDelaysAdded);
                        break;
                    case WolverineMessageBusSettings.ErrorHandlingPolicyOptionsEnum.ScheduleRetry:
                        method.AddStatement("""var delays = ParseDelays(configuration["Wolverine:ErrorHandling:ScheduleRetry:Delays"] ?? "00:01:00, 00:05:00, 00:15:00");""");
                        AddDelayBranch(method, "ScheduleRetry");
                        AddParseDelaysMethod(@class, ctx.ParseDelaysAdded);
                        break;
                }
            });

            void AddDelayBranch(CSharpClassMethod method, string retryMethodName)
            {
                method.AddIfStatement("delays.Length == 0", @if =>
                {
                    @if.AddStatement("opts.OnException<Exception>().MoveToErrorQueue();");
                });
                method.AddElseStatement(@else =>
                {
                    @else.AddStatement($"opts.OnException<Exception>().{retryMethodName}(delays).Then.MoveToErrorQueue();");
                });
            }
        }

        /// <summary>
        /// Emitted once, the first time either Error Handling Policy that needs it (RetryWithCooldown /
        /// ScheduleRetry) is configured - guarded so a second call in the same file is a no-op.
        /// </summary>
        private static void AddParseDelaysMethod(CSharpClass @class, HashSet<CSharpClass> alreadyAdded)
        {
            if (!alreadyAdded.Add(@class))
            {
                return;
            }

            @class.AddMethod("System.TimeSpan[]", "ParseDelays", method =>
            {
                method.Private().Static();
                method.AddParameter("string", "value");

                method.AddStatement(
                    "return value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(TimeSpan.Parse).ToArray();");
            });
        }

        /// <summary>
        /// R6.3: the five Durable registrations, host-scope, called once from ConfigureEventing. Only
        /// emitted when Transactional Outbox is Durable.
        /// </summary>
        private static void AddApplyTransactionalOutboxMethod(CSharpClass @class, EventingContext ctx)
        {
            @class.AddMethod("void", "ApplyTransactionalOutbox", method =>
            {
                method.Private().Static();
                method.AddParameter("WolverineOptions", "opts");
                method.AddParameter("IConfiguration", "configuration");

                method.AddStatement($$"""var connectionString = configuration.GetConnectionString("{{ctx.ConnectionStringName}}");""");

                var persistStatement = ctx.DatabaseProvider!.IsSqlServer()
                    ? "opts.PersistMessagesWithSqlServer(connectionString);"
                    : "opts.PersistMessagesWithPostgresql(connectionString);";
                method.AddStatement(persistStatement, s => s.SeparatedFromPrevious());
                method.AddStatement("opts.UseEntityFrameworkCoreTransactions();");
                method.AddStatement("opts.Policies.AutoApplyTransactions();");
                method.AddStatement("opts.Policies.UseDurableOutboxOnAllSendingEndpoints();");
                method.AddStatement("opts.Policies.UseDurableInboxOnAllListeners();");
            });
        }

        /// <summary>
        /// Resolution order (R3.3/R3.4): a non-null Topic Name override is used verbatim once
        /// validated - otherwise the kebab-case convention name of the generated message type's
        /// simple name is used.
        /// </summary>
        private static string ResolvePublishName(EventingContext ctx, MessageModel message)
        {
            var topicName = message.GetWolverineMessage()?.TopicName();
            if (topicName != null)
            {
                ValidateNameOverride(topicName, message.InternalElement, message.Name, "Topic Name");
                return topicName;
            }

            return GetConventionName(ctx.Template.GetFullyQualifiedTypeName(IntegrationEventMessageTemplate.TemplateId, message));
        }

        /// <summary>
        /// Resolution order (R4.1/R4.2): a non-null Destination Queue Name override (declared on the
        /// Integration Command element itself, per D4) is used verbatim once validated - otherwise the
        /// kebab-case convention name of the generated command type's simple name is used.
        /// </summary>
        private static string ResolveSendName(EventingContext ctx, IntegrationCommandModel command)
        {
            var queueName = command.GetWolverineMessage()?.DestinationQueueName();
            if (queueName != null)
            {
                ValidateNameOverride(queueName, command.InternalElement, command.Name, "Destination Queue Name");
                return queueName;
            }

            return GetConventionName(ctx.Template.GetFullyQualifiedTypeName(IntegrationCommandTemplate.TemplateId, command));
        }

        /// <summary>
        /// R3.5/R3.6 (and the Command Distribution equivalent, R4.7's sibling requirements): an
        /// override that is present but empty/whitespace-only after trim, or longer than 250
        /// characters, is an error against the offending element.
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

        /// <summary>
        /// R5.7/R5.9: the subscribing application's name and the message's name, each kebab-cased and
        /// joined by a hyphen. Never overridable, so every subscriber of a fanned-out Integration Event
        /// ends up with its own queue rather than sharing one.
        /// </summary>
        private static string GetSubscriberQueueName(EventingContext ctx, MessageModel message)
        {
            var messageNameKebab = GetConventionName(ctx.Template.GetFullyQualifiedTypeName(IntegrationEventMessageTemplate.TemplateId, message));
            return $"{ctx.ApplicationNameKebab}-{messageNameKebab}";
        }

        /// <summary>
        /// Registers default appsettings.json entries for the connection settings the generated
        /// ConfigureXTransport methods read. Registration is additive only - there is no API to remove
        /// a previously-registered key.
        /// </summary>
        private static void PublishAppSettings(IApplication application, EventingContext ctx)
        {
            switch (ctx.Transport)
            {
                case WolverineMessageBusSettings.TransportOptionsEnum.Rabbitmq:
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:RabbitMq:Host", "localhost"));
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:RabbitMq:Port", "5672"));
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:RabbitMq:VirtualHost", "/"));
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:RabbitMq:Username", "guest"));
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:RabbitMq:Password", "guest"));
                    break;
                case WolverineMessageBusSettings.TransportOptionsEnum.AzureServiceBus:
                    // R2.3/R2.6: no default - left as an empty placeholder so the generated reader's
                    // fail-fast guard can actually fire.
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:AzureServiceBus:ConnectionString", ""));
                    break;
                case WolverineMessageBusSettings.TransportOptionsEnum.AmazonSqs:
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:AmazonSqs:Region", ""));
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:AmazonSqs:AccessKey", ""));
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:AmazonSqs:SecretKey", ""));
                    break;
            }

            switch (ctx.ErrorHandlingPolicy.AsEnum())
            {
                case WolverineMessageBusSettings.ErrorHandlingPolicyOptionsEnum.Retry:
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:ErrorHandling:Retry:Attempts", "3"));
                    break;
                case WolverineMessageBusSettings.ErrorHandlingPolicyOptionsEnum.RetryWithCooldown:
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:ErrorHandling:RetryWithCooldown:Delays", "00:00:01, 00:00:05, 00:00:15"));
                    break;
                case WolverineMessageBusSettings.ErrorHandlingPolicyOptionsEnum.ScheduleRetry:
                    application.EventDispatcher.Publish(new AppSettingRegistrationRequest("Wolverine:ErrorHandling:ScheduleRetry:Delays", "00:01:00, 00:05:00, 00:15:00"));
                    break;
            }
        }

        /// <summary>
        /// Everything this module's contribution needs, resolved once per application from the found
        /// Wolverine.Common template. Mirrors what the retired WolverineEventingConfigurationTemplate
        /// used to hold as instance fields.
        /// </summary>
        private sealed class EventingContext
        {
            public ICSharpFileBuilderTemplate Template { get; private set; } = null!;
            public WolverineMessageBusSettings.TransportOptionsEnum Transport { get; private set; }
            public WolverineMessageBusSettings.TransactionalOutboxOptions TransactionalOutbox { get; private set; } = null!;
            public WolverineMessageBusSettings.ErrorHandlingPolicyOptions ErrorHandlingPolicy { get; private set; } = null!;
            public DatabaseProviderExtensions.DatabaseProviderOptions? DatabaseProvider { get; private set; }
            public string? ConnectionStringName { get; private set; }
            public IReadOnlyList<MessageModel> PublishedMessages { get; private set; } = Array.Empty<MessageModel>();
            public IReadOnlyList<IntegrationCommandModel> SentCommands { get; private set; } = Array.Empty<IntegrationCommandModel>();
            public IReadOnlyList<MessageModel> SubscribedMessages { get; private set; } = Array.Empty<MessageModel>();
            public IReadOnlyList<IntegrationCommandModel> ReceivedCommands { get; private set; } = Array.Empty<IntegrationCommandModel>();
            public string ApplicationNameKebab { get; private set; } = "";
            public IReadOnlyList<IntegrationEventHandlerModel> SubscribedHandlers { get; private set; } = Array.Empty<IntegrationEventHandlerModel>();

            /// <summary>Tracks which classes already got a ParseDelays method, so a second Error Handling Policy branch never duplicates it.</summary>
            public HashSet<CSharpClass> ParseDelaysAdded { get; } = new();

            public static EventingContext Build(ICSharpFileBuilderTemplate template)
            {
                var wolverineSettings = template.ExecutionContext.Settings.GetWolverineMessageBusSettings();
                var ctx = new EventingContext
                {
                    Template = template,
                    Transport = wolverineSettings.Transport().AsEnum(),
                    TransactionalOutbox = wolverineSettings.TransactionalOutbox(),
                    ErrorHandlingPolicy = wolverineSettings.ErrorHandlingPolicy(),
                    ApplicationNameKebab = template.ExecutionContext.GetApplicationConfig().Name.ToKebabCase(),
                };

                ctx.PublishedMessages = template.GetWolverineDesignatedMessages(
                    template.ExecutionContext.MetadataManager.GetExplicitlyPublishedMessageModels(template.OutputTarget.Application))
                    .ToList();
                ctx.SentCommands = template.GetWolverineDesignatedIntegrationCommands(
                    template.ExecutionContext.MetadataManager.GetExplicitlySentIntegrationCommandModels(template.OutputTarget.Application))
                    .ToList();
                ctx.SubscribedMessages = template.GetWolverineDesignatedMessages(
                    template.ExecutionContext.MetadataManager.GetExplicitlySubscribedToMessageModels(template.OutputTarget.Application))
                    .ToList();
                ctx.ReceivedCommands = template.GetWolverineDesignatedIntegrationCommands(
                    template.ExecutionContext.MetadataManager.GetExplicitlySubscribedToIntegrationCommandModels(template.OutputTarget.Application))
                    .ToList();

                if (ctx.TransactionalOutbox.IsDurable())
                {
                    ctx.DatabaseProvider = template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider();
                    ctx.ConnectionStringName = template.ExecutionContext.Settings.GetDatabaseSettings().ConnectionStringName();
                }

                ctx.SubscribedHandlers = GetWolverineDesignatedSubscribedHandlers(template);

                return ctx;
            }

            /// <summary>
            /// This application's Integration Event Handlers that carry at least one Wolverine-designated
            /// Integration Event or Integration Command subscription.
            /// </summary>
            private static IReadOnlyList<IntegrationEventHandlerModel> GetWolverineDesignatedSubscribedHandlers(ICSharpFileBuilderTemplate template)
            {
                var allHandlers = template.ExecutionContext.MetadataManager
                    .Services(template.ExecutionContext.GetApplicationConfig().Id)
                    .GetIntegrationEventHandlerModels();

                return allHandlers
                    .Where(handler =>
                        template.GetWolverineDesignatedMessages(handler.IntegrationEventSubscriptions()
                            .Select(subscription => subscription.TypeReference.Element.AsMessageModel()))
                            .Any()
                        || template.GetWolverineDesignatedIntegrationCommands(handler.IntegrationCommandSubscriptions()
                            .Select(subscription => subscription.TypeReference.Element.AsIntegrationCommandModel()))
                            .Any())
                    .OrderBy(handler => handler.Name)
                    .ToList();
            }
        }
    }
}
