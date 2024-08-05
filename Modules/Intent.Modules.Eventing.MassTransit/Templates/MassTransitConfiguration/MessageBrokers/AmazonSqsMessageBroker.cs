using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.MessageBrokers;

internal class AmazonSqsMessageBroker : MessageBrokerBase
{
    public AmazonSqsMessageBroker(ICSharpFileBuilderTemplate template) : base(template)
    {
    }

    public override string GetMessageBrokerBusFactoryConfiguratorName()
    {
        return "IAmazonSqsBusFactoryConfigurator";
    }

    public override CSharpInvocationStatement AddMessageBrokerConfiguration(string busRegistrationVarName, string factoryConfigVarName, IEnumerable<CSharpStatement> moreConfiguration)
    {
        var stmt = new CSharpInvocationStatement($"{busRegistrationVarName}.UsingAmazonSqs")
            .AddArgument(new CSharpLambdaBlock($"(context, {factoryConfigVarName})")
                .AddInvocationStatement($"{factoryConfigVarName}.Host", host => host
                    .AddArgument(@"configuration[""AmazonSqs:Host""]")
                    .AddArgument(new CSharpLambdaBlock("host")
                        .AddStatement(@"host.AccessKey(configuration[""AmazonSqs:AccessKey""]);")
                        .AddStatement(@"host.SecretKey(configuration[""AmazonSqs:SecretKey""]);"))
                    .SeparatedFromPrevious())
                .AddStatements(moreConfiguration));
        stmt.AddMetadata("message-broker", "amazon-sqs");
        return stmt;
    }

    public override AppSettingRegistrationRequest? GetAppSettings()
    {
        return new AppSettingRegistrationRequest("AmazonSqs", new
        {
            Host = "us-east-1",
            AccessKey = "your-iam-access-key",
            SecretKey = "your-iam-secret-key"
        });
    }

    public override INugetPackageInfo? GetNugetDependency(IOutputTarget outputTarget)
    {
        return NugetPackages.MassTransitAmazonSQS(outputTarget);
    }

    public override IEnumerable<CSharpStatement> GetCustomConfigurationStatements(Consumer consumer, string sanitizedAppName)
    {
        yield return CreateConsumerReceiveEndpointStatement(consumer, sanitizedAppName, null, [], _template);
    }

    public override IEnumerable<CSharpClassMethod> GetCustomConfigurationHelperMethods(CSharpClass configurationClass)
    {
        yield return CreateConsumerReceiveEndpointMethod(configurationClass, GetMessageBrokerBusFactoryConfiguratorName(), "IAmazonSqsReceiveEndpointConfigurator");
    }
}