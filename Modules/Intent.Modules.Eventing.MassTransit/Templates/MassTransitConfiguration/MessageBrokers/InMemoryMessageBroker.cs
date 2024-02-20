using System.Collections.Generic;
using System.Linq;
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

    public override bool HasMessageBrokerStereotype(Subscription subscription)
    {
        return false;
    }

    public override string GetMessageBrokerBusFactoryConfiguratorName()
    {
        return "IInMemoryBusFactoryConfigurator";
    }

    public override string GetMessageBrokerReceiveEndpointConfiguratorName()
    {
        return "IInMemoryReceiveEndpointConfigurator";
    }

    public override IEnumerable<CSharpStatement> AddBespokeConsumerConfigurationStatements(string configVarName, Subscription subscription)
    {
        return Enumerable.Empty<CSharpStatement>();
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

    public override INugetPackageInfo? GetNugetDependency()
    {
        return default;
    }
}