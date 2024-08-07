using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Eventing.MassTransit.Settings;

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.MessageBrokers;

internal abstract class MessageBrokerBase
{
    protected readonly ICSharpFileBuilderTemplate _template;

    protected MessageBrokerBase(ICSharpFileBuilderTemplate template)
    {
        _template = template;
    }

    public abstract string GetMessageBrokerBusFactoryConfiguratorName();
    public abstract CSharpInvocationStatement AddMessageBrokerConfiguration(string busRegistrationVarName, string factoryConfigVarName, IEnumerable<CSharpStatement> moreConfiguration);
    public abstract AppSettingRegistrationRequest? GetAppSettings();
    public abstract INugetPackageInfo? GetNugetDependency(IOutputTarget outputTarget);
    public abstract IEnumerable<CSharpStatement> GetCustomConfigurationStatements(Consumer consumer, string sanitizedAppName);
    public abstract IEnumerable<CSharpClassMethod> GetCustomConfigurationHelperMethods(CSharpClass configurationClass);

    protected static CSharpClassMethod CreateConsumerReceiveEndpointMethod(
        CSharpClass @class, string messageBrokerFactoryConfigName,string messageBrokerEndpointConfigName)
    {
        var method = new CSharpClassMethod("void", "AddConsumerReceiveEndpoint", @class);

        method.Private().Static();
        method.AddGenericParameter("TConsumer", out var tConsumer);
        method.AddGenericTypeConstraint(tConsumer, c => c.AddType("class").AddType("IConsumer"));

        method.AddParameter(messageBrokerFactoryConfigName, "cfg", param => param.WithThisModifier());
        method.AddParameter("IBusRegistrationContext", "context");
        method.AddParameter($"Action<EndpointSettings<IEndpointDefinition<{tConsumer}>>>", "endpointDefinitionConfig");
        method.AddParameter($"Action<{messageBrokerEndpointConfigName}>", "receiveEndpointConfig");

        method.AddStatement($"var settings = new EndpointSettings<IEndpointDefinition<{tConsumer}>>();");
        method.AddStatement($"endpointDefinitionConfig(settings);");

        method.AddInvocationStatement($"cfg.ReceiveEndpoint", stmt => stmt
            .AddArgument(new CSharpInvocationStatement($"new ConsumerEndpointDefinition<{tConsumer}>")
                .WithoutSemicolon()
                .AddArgument("settings"))
            .AddArgument("KebabCaseEndpointNameFormatter.Instance")
            .AddArgument(new CSharpLambdaBlock("endpoint")
                .AddStatement("receiveEndpointConfig.Invoke(endpoint);")
                .AddStatement($"endpoint.ConfigureConsumer<{tConsumer}>(context);"))
            .WithArgumentsOnNewLines()
            .SeparatedFromPrevious());
        
        return method;
    }
    
    protected static CSharpInvocationStatement CreateConsumerReceiveEndpointStatement(
        Consumer consumer, string sanitizedAppName, 
        CSharpLambdaBlock? definitionConfiguration, IEnumerable<CSharpStatement> endpointConfigStatements, 
        IIntentTemplate template)
    {
        var stmt = new CSharpInvocationStatement($"cfg.AddConsumerReceiveEndpoint<{consumer.ConsumerTypeName}>");
        stmt.AddArgument("context");
        if (definitionConfiguration is null)
        {
            var lambda = new CSharpLambdaBlock("definition");
            lambda.WithExpressionBody($@"definition.InstanceId = ""{sanitizedAppName}""");
            stmt.AddArgument(lambda);
        }
        else
        {
            stmt.AddArgument(definitionConfiguration);
        }

        var endpointsBlock = new CSharpLambdaBlock("endpoint").AddStatements(endpointConfigStatements);
        stmt.AddArgument(endpointsBlock).WithArgumentsOnNewLines();
        
        return stmt;
    }
}