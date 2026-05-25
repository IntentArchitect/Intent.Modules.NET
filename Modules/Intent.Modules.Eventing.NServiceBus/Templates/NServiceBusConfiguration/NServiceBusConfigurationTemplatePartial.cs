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

            AddNugetDependency(NugetPackages.NServiceBus(OutputTarget));
            AddNugetDependency(NugetPackages.NServiceBusExtensionsHosting(OutputTarget));

            var transport = ExecutionContext.Settings.GetNServiceBusSettings().Transport();
            var recoverabilityPolicy = ExecutionContext.Settings.GetNServiceBusSettings().RecoverabilityPolicy();
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
                case NServiceBusSettings.TransportOptionsEnum.SqlServer:
                    AddNugetDependency(NugetPackages.NServiceBusTransportSqlServer(OutputTarget));
                    break;
                case NServiceBusSettings.TransportOptionsEnum.LearningTransport:
                    // Learning Transport is included in NServiceBus.Core — no extra package needed
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported transport type: {transport.Value}");
            }

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddUsing("NServiceBus")
                .AddClass("NServiceBusConfiguration", @class =>
                {
                    @class.Static();

                    @class.AddMethod("IHostBuilder", "AddNServiceBus", method =>
                    {
                        method.Static();
                        method.AddParameter("IHostBuilder", "hostBuilder", p => p.WithThisModifier());
                        method.AddParameter("IConfiguration", "configuration");
                        method.AddReturn("hostBuilder.UseNServiceBus(ctx => ConfigureEndpoint(configuration))");
                    });

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
                            var registryEventTemplates = ExecutionContext
                                .FindTemplateInstances<CSharpTemplateBase<MessageModel>>(IntegrationEventMessageTemplate.TemplateId)
                                .FilterMessagesForThisMessageBroker(ExecutionContext, BrokerStereotypeIds, t => t.Model)
                                .ToList();

                            var registryCommandTemplates = ExecutionContext
                                .FindTemplateInstances<CSharpTemplateBase<IntegrationCommandModel>>(IntegrationCommandTemplate.TemplateId)
                                .FilterMessagesForThisMessageBroker(ExecutionContext, BrokerStereotypeIds, t => t.Model)
                                .ToList();

                            foreach (var et in registryEventTemplates)
                            {
                                var msgName = GetTypeName(IntegrationEventMessageTemplate.TemplateId, et.Model);
                                method.AddStatement($"registry.Register<{msgName}, {nsbBusName}>();");
                            }

                            foreach (var ct in registryCommandTemplates)
                            {
                                var msgName = GetTypeName(IntegrationCommandTemplate.TemplateId, ct.Model);
                                method.AddStatement($"registry.Register<{msgName}, {nsbBusName}>();");
                            }
                        }

                        method.AddReturn("services");
                    });

                    @class.AddMethod("EndpointConfiguration", "ConfigureEndpoint", method =>
                    {
                        method.Static().Private();
                        method.AddParameter("IConfiguration", "configuration");

                        method.AddStatement(@"var endpointName = configuration[""NServiceBus:EndpointName""] ?? throw new InvalidOperationException(""NServiceBus:EndpointName is not configured"");");
                        method.AddStatement("var endpointConfiguration = new EndpointConfiguration(endpointName);");

                        if (transport.IsRabbitmq())
                        {
                            method.AddStatement(@"var connectionString = configuration.GetConnectionString(""RabbitMQ"") ?? throw new InvalidOperationException(""ConnectionStrings:RabbitMQ is not configured"");", s => s.SeparatedFromPrevious());
                            method.AddStatement("var transportConfig = endpointConfiguration.UseTransport(new RabbitMQTransport(RoutingTopology.Conventional(QueueType.Quorum), connectionString));");
                        }
                        else if (transport.IsAzureServiceBus())
                        {
                            method.AddStatement(@"var connectionString = configuration.GetConnectionString(""AzureServiceBus"") ?? throw new InvalidOperationException(""ConnectionStrings:AzureServiceBus is not configured"");", s => s.SeparatedFromPrevious());
                            method.AddStatement("var transportConfig = endpointConfiguration.UseTransport(new AzureServiceBusTransport(connectionString));");
                        }
                        else if (transport.IsAmazonSqs())
                        {
                            method.AddStatement("var transportConfig = endpointConfiguration.UseTransport(new SqsTransport());", s => s.SeparatedFromPrevious());
                        }
                        else if (transport.IsSqlServer())
                        {
                            method.AddStatement(@"var connectionString = configuration.GetConnectionString(""SqlServer"") ?? throw new InvalidOperationException(""ConnectionStrings:SqlServer is not configured"");", s => s.SeparatedFromPrevious());
                            method.AddStatement("var transportConfig = endpointConfiguration.UseTransport(new SqlServerTransport(connectionString));");
                        }
                        else if (transport.IsLearningTransport())
                        {
                            method.AddStatement("var transportConfig = endpointConfiguration.UseTransport(new LearningTransport());", s => s.SeparatedFromPrevious());
                        }

                        method.AddStatement("endpointConfiguration.EnableInstallers();", s => s.SeparatedFromPrevious());
                        method.AddStatement("endpointConfiguration.UseSerialization<SystemJsonSerializer>();");

                        // Discover modeled events and commands belonging to this broker.
                        // We classify them explicitly by type rather than by name/namespace
                        // so the convention is precise and respects multi-broker filtering.
                        var eventTemplates = ExecutionContext
                            .FindTemplateInstances<CSharpTemplateBase<MessageModel>>(
                                IntegrationEventMessageTemplate.TemplateId)
                            .FilterMessagesForThisMessageBroker(ExecutionContext, BrokerStereotypeIds, t => t.Model)
                            .ToList();

                        var commandTemplates = ExecutionContext
                            .FindTemplateInstances<CSharpTemplateBase<IntegrationCommandModel>>(
                                IntegrationCommandTemplate.TemplateId)
                            .FilterMessagesForThisMessageBroker(ExecutionContext, BrokerStereotypeIds, t => t.Model)
                            .ToList();

                        if (eventTemplates.Count > 0 || commandTemplates.Count > 0)
                        {
                            method.AddStatement("var conventions = endpointConfiguration.Conventions();", s => s.SeparatedFromPrevious());

                            if (eventTemplates.Count > 0)
                            {
                                var eventTypeOfs = string.Join(", ",
                                    eventTemplates.Select(t => $"typeof({GetTypeName(IntegrationEventMessageTemplate.TemplateId, t.Model)})"));
                                method.AddStatement($"conventions.DefiningEventsAs(new[] {{ {eventTypeOfs} }}.Contains);");
                            }

                            if (commandTemplates.Count > 0)
                            {
                                var commandTypeOfs = string.Join(", ",
                                    commandTemplates.Select(t => $"typeof({GetTypeName(IntegrationCommandTemplate.TemplateId, t.Model)})"));
                                method.AddStatement($"conventions.DefiningCommandsAs(new[] {{ {commandTypeOfs} }}.Contains);");
                            }
                        }

                        if (!recoverabilityPolicy.IsNone())
                        {
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

                        if (commandTemplates.Count > 0)
                        {
                            var first = true;
                            foreach (var ct in commandTemplates)
                            {
                                var commandTypeName = GetTypeName(IntegrationCommandTemplate.TemplateId, ct.Model);
                                var commandName = ct.Model.Name;
                                var defaultEndpoint = ct.Model.GetNServiceBus()?.EndpointName() ?? commandName;
                                var isFirst = first;
                                first = false;
                                method.AddStatement(
                                    $"""transportConfig.RouteToEndpoint(typeof({commandTypeName}), configuration["NServiceBus:Routing:Commands:{commandName}"] ?? "{defaultEndpoint}");""",
                                    s => { if (isFirst) s.SeparatedFromPrevious(); });
                            }
                        }

                        method.AddReturn("endpointConfiguration", s => s.SeparatedFromPrevious());
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            if (this.RequiresCompositeMessageBus())
            {
                return;
            }

            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("AddNServiceBusConfiguration", ServiceConfigurationRequest.ParameterType.Configuration)
                .ForConcern("Infrastructure")
                .HasDependency(this));
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
