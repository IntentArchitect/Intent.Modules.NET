using System.Collections.Generic;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.MessageBrokers;

internal class RabbitMqMessageBroker : MessageBrokerBase
{
    public RabbitMqMessageBroker(ICSharpFileBuilderTemplate template) : base(template)
    {
    }

    public override string GetMessageBrokerBusFactoryConfiguratorName()
    {
        return "IRabbitMqBusFactoryConfigurator";
    }

    public override string GetMessageBrokerReceiveEndpointConfiguratorName()
    {
        return "IRabbitMqReceiveEndpointConfigurator";
    }

    public override IEnumerable<CSharpStatement> AddBespokeConsumerConfigurationStatements(string configVarName, Consumer consumer)
    {
        var busConsumerSettings = consumer.RabbitMqConsumerSettings!;
        
        if (busConsumerSettings.PrefetchCount().HasValue)
        {
            yield return $@"{configVarName}.PrefetchCount = {busConsumerSettings.PrefetchCount()};";
        }

        yield return $@"{configVarName}.Lazy = {busConsumerSettings.Lazy().ToString().ToLower()};";
        yield return $@"{configVarName}.Durable = {busConsumerSettings.Durable().ToString().ToLower()};";
        yield return $@"{configVarName}.PurgeOnStartup = {busConsumerSettings.PurgeOnStartup().ToString().ToLower()};";
        yield return $@"{configVarName}.Exclusive = {busConsumerSettings.Exclusive().ToString().ToLower()};";
    }

    public override CSharpInvocationStatement AddMessageBrokerConfiguration(string busRegistrationVarName, string factoryConfigVarName, IEnumerable<CSharpStatement> moreConfiguration)
    {
        var stmt = new CSharpInvocationStatement($"{busRegistrationVarName}.UsingRabbitMq")
            .AddArgument(new CSharpLambdaBlock($"(context, {factoryConfigVarName})")
                .AddInvocationStatement($"{factoryConfigVarName}.Host", host => host
                    .AddArgument(@"configuration[""RabbitMq:Host""]")
                    .AddArgument(@"configuration[""RabbitMq:VirtualHost""]")
                    .AddArgument(new CSharpLambdaBlock("host")
                        .AddStatement(@"host.Username(configuration[""RabbitMq:Username""]);")
                        .AddStatement(@"host.Password(configuration[""RabbitMq:Password""]);"))
                    .SeparatedFromPrevious())
                .AddStatements(moreConfiguration));
        stmt.AddMetadata("message-broker", "rabbit-mq");
        return stmt;
    }

    public override AppSettingRegistrationRequest? GetAppSettings()
    {
        return new AppSettingRegistrationRequest("RabbitMq", new
        {
            Host = "localhost",
            VirtualHost = "/",
            Username = "guest",
            Password = "guest"
        });
    }

    public override INugetPackageInfo? GetNugetDependency()
    {
        return NuGetPackages.MassTransitRabbitMq;
    }
}