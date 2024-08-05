using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.MessageBrokers;

internal class InMemoryMessageBroker : MessageBrokerBase
{
    public InMemoryMessageBroker(ICSharpFileBuilderTemplate template) : base(template)
    {
    }

    public override string GetMessageBrokerBusFactoryConfiguratorName()
    {
        return "IInMemoryBusFactoryConfigurator";
    }

    public override CSharpInvocationStatement AddMessageBrokerConfiguration(string busRegistrationVarName, string factoryConfigVarName, IEnumerable<CSharpStatement> moreConfiguration)
    {
        var stmt = new CSharpInvocationStatement($"{busRegistrationVarName}.UsingInMemory")
            .AddArgument(new CSharpLambdaBlock($"(context, {factoryConfigVarName})")
                .AddStatements(moreConfiguration));
        stmt.AddMetadata("message-broker", "memory");
        return stmt;
    }

    public override AppSettingRegistrationRequest? GetAppSettings()
    {
        return default;
    }

    public override INugetPackageInfo? GetNugetDependency(IOutputTarget outputTarget)
    {
        return default;
    }

    public override IEnumerable<CSharpStatement> GetCustomConfigurationStatements(Consumer consumer, string sanitizedAppName)
    {
        yield return CreateConsumerReceiveEndpointStatement(consumer, sanitizedAppName, null, [], _template);
    }

    public override IEnumerable<CSharpClassMethod> GetCustomConfigurationHelperMethods(CSharpClass configurationClass)
    {
        yield return CreateConsumerReceiveEndpointMethod(configurationClass, GetMessageBrokerBusFactoryConfiguratorName(), "IInMemoryReceiveEndpointConfigurator");
    }
}