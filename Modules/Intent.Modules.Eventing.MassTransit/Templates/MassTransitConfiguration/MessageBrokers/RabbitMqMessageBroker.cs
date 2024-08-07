using System.Collections.Generic;
using Intent.Engine;
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

    public override INugetPackageInfo? GetNugetDependency(IOutputTarget outputTarget)
    {
        return NugetPackages.MassTransitRabbitMQ(outputTarget);
    }

    public override IEnumerable<CSharpStatement> GetCustomConfigurationStatements(Consumer consumer, string sanitizedAppName)
    {
        CSharpLambdaBlock? definitionBlock = null;
        if (!string.IsNullOrWhiteSpace(consumer.Settings.RabbitMqConsumerSettings!.EndpointName()))
        {
            definitionBlock = new CSharpLambdaBlock("definition");
            definitionBlock.WithExpressionBody($@"definition.Name = ""{consumer.Settings.RabbitMqConsumerSettings.EndpointName()}""");
        }
        
        yield return CreateConsumerReceiveEndpointStatement(consumer, sanitizedAppName, definitionBlock, GetConfigurationStatements("endpoint", consumer), _template);
    }

    public override IEnumerable<CSharpClassMethod> GetCustomConfigurationHelperMethods(CSharpClass configurationClass)
    {
        yield return CreateConsumerReceiveEndpointMethod(configurationClass, GetMessageBrokerBusFactoryConfiguratorName(), "IRabbitMqReceiveEndpointConfigurator");
    }
    
    private IEnumerable<CSharpStatement> GetConfigurationStatements(string configVarName, Consumer consumer)
    {
        var busConsumerSettings = consumer.Settings.RabbitMqConsumerSettings!;
        
        if (busConsumerSettings.PrefetchCount().HasValue)
        {
            yield return $@"{configVarName}.PrefetchCount = {busConsumerSettings.PrefetchCount()};";
        }
    
        yield return $@"{configVarName}.Lazy = {busConsumerSettings.Lazy().ToString().ToLower()};";
        yield return $@"{configVarName}.Durable = {busConsumerSettings.Durable().ToString().ToLower()};";
        yield return $@"{configVarName}.PurgeOnStartup = {busConsumerSettings.PurgeOnStartup().ToString().ToLower()};";
        yield return $@"{configVarName}.Exclusive = {busConsumerSettings.Exclusive().ToString().ToLower()};";
    
        if (busConsumerSettings.ConcurrentMessageLimit().HasValue)
        {
            yield return $@"{configVarName}.ConcurrentMessageLimit = {busConsumerSettings.ConcurrentMessageLimit()};";
        }
    }
}