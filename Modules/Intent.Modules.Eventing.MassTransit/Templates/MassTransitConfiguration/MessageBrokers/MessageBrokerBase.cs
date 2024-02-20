using System.Collections.Generic;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.MessageBrokers;

internal abstract class MessageBrokerBase
{
    protected readonly ICSharpFileBuilderTemplate _template;

    protected MessageBrokerBase(ICSharpFileBuilderTemplate template)
    {
        _template = template;
    }

    public abstract bool HasMessageBrokerStereotype(Subscription subscription);
    public abstract string GetMessageBrokerBusFactoryConfiguratorName();
    public abstract string GetMessageBrokerReceiveEndpointConfiguratorName();
    public abstract IEnumerable<CSharpStatement> AddBespokeConsumerConfigurationStatements(string configVarName, Subscription subscription);
    public abstract CSharpInvocationStatement AddMessageBrokerConfiguration(string busRegistrationVarName, string factoryConfigVarName, IEnumerable<CSharpStatement> moreConfiguration);
    public abstract AppSettingRegistrationRequest? GetAppSettings();
    public abstract INugetPackageInfo? GetNugetDependency();
}