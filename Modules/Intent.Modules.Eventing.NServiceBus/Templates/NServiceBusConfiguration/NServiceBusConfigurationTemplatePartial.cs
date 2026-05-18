using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.NServiceBus.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

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

AddNugetDependency(NugetPackages.NServiceBus(OutputTarget));
                    AddNugetDependency(NugetPackages.NServiceBusExtensionsHosting(OutputTarget));

            var transport = ExecutionContext.Settings.GetNServiceBusSettings().Transport();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddUsing("NServiceBus")
                .OnBuild(file =>
                {
                    if (transport.IsRabbitmq())
                        AddNugetDependency(NugetPackages.NServiceBusRabbitMQ(OutputTarget));
                    else if (transport.IsAzureServiceBus())
                        AddNugetDependency(NugetPackages.NServiceBusTransportAzureServiceBus(OutputTarget));
                    else if (transport.IsAmazonSqs())
                        AddNugetDependency(NugetPackages.NServiceBusAmazonSQS(OutputTarget));
                    else if (transport.IsSqlServer())
                        AddNugetDependency(NugetPackages.NServiceBusTransportSqlServer(OutputTarget));
                })
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

                    @class.AddMethod("EndpointConfiguration", "ConfigureEndpoint", method =>
                    {
                        method.Static().Private();
                        method.AddParameter("IConfiguration", "configuration");

                        method.AddStatement(@"var endpointName = configuration[""NServiceBus:EndpointName""] ?? throw new InvalidOperationException(""NServiceBus:EndpointName is not configured"");");
                        method.AddStatement("var endpointConfiguration = new EndpointConfiguration(endpointName);");

                        if (transport.IsRabbitmq())
                        {
                            method.AddStatement(@"var connectionString = configuration.GetConnectionString(""RabbitMQ"") ?? throw new InvalidOperationException(""ConnectionStrings:RabbitMQ is not configured"");", s => s.SeparatedFromPrevious());
                            method.AddStatement("endpointConfiguration.UseTransport(new RabbitMQTransport(RoutingTopology.Conventional(QueueType.Quorum), connectionString));");
                        }
                        else if (transport.IsAzureServiceBus())
                        {
                            method.AddStatement(@"var connectionString = configuration.GetConnectionString(""AzureServiceBus"") ?? throw new InvalidOperationException(""ConnectionStrings:AzureServiceBus is not configured"");", s => s.SeparatedFromPrevious());
                            method.AddStatement("endpointConfiguration.UseTransport(new AzureServiceBusTransport(connectionString));");
                        }
                        else if (transport.IsAmazonSqs())
                        {
                            method.AddStatement("endpointConfiguration.UseTransport(new SqsTransport());", s => s.SeparatedFromPrevious());
                        }
                        else if (transport.IsSqlServer())
                        {
                            method.AddStatement(@"var connectionString = configuration.GetConnectionString(""SqlServer"") ?? throw new InvalidOperationException(""ConnectionStrings:SqlServer is not configured"");", s => s.SeparatedFromPrevious());
                            method.AddStatement("endpointConfiguration.UseTransport(new SqlServerTransport(connectionString));");
                        }
                        else if (transport.IsLearningTransport())
                        {
                            method.AddStatement("endpointConfiguration.UseTransport(new LearningTransport());", s => s.SeparatedFromPrevious());
                        }

                        method.AddStatement("endpointConfiguration.EnableInstallers();", s => s.SeparatedFromPrevious());
                        method.AddStatement("endpointConfiguration.UseSerialization<SystemJsonSerializer>();");

                        method.AddReturn("endpointConfiguration", s => s.SeparatedFromPrevious());
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
