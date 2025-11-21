using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Settings;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Integration.IaC.Shared;
using Intent.Modules.Integration.IaC.Shared.AzureEventGrid;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureEventGrid.Templates.AzureEventGridConfiguration;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class AzureEventGridConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Eventing.AzureEventGrid.AzureEventGridConfiguration";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public AzureEventGridConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("Microsoft.Extensions.DependencyInjection")
            .AddUsing("Microsoft.Extensions.Configuration")
            .AddClass($"AzureEventGridConfiguration", @class =>
            {
                @class.Static()
                    .AddMethod("IServiceCollection", "ConfigureEventGrid", method =>
                    {
                        method.Static()
                            .AddParameter("IServiceCollection", "services", param => param.WithThisModifier())
                            .AddParameter("IConfiguration", "configuration");
                        
                        var compositeTemplate = GetTemplate<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Intent.Eventing.Contracts.CompositeMessageBusConfiguration"));
                        var isCompositeMode = compositeTemplate != null;
                        
                        if (isCompositeMode)
                        {
                            method.AddParameter(this.GetMessageBrokerRegistryName(), "registry");
                        }

                        if (isCompositeMode)
                        {
                            method.AddStatement($"// Register as concrete type for composite message bus");
                            method.AddStatement($"services.AddScoped<{this.GetAzureEventGridEventBusName()}>;");
                        }
                        else
                        {
                            var busInterface = this.GetBusInterfaceName();
                            
                            method.AddStatement($"// Register as {busInterface} for standalone mode");
                            method.AddStatement($"services.AddScoped<{this.GetAzureEventGridEventBusName()}>;");
                            method.AddStatement($"services.AddScoped<{busInterface}>(provider => provider.GetRequiredService<{this.GetAzureEventGridEventBusName()}>);");
                        }
                        method.AddStatement($"services.AddSingleton<{this.GetAzureEventGridMessageDispatcherName()}>();");
                        method.AddStatement($"services.AddSingleton<{this.GetAzureEventGridMessageDispatcherInterfaceName()}, {this.GetAzureEventGridMessageDispatcherName()}>();");

                        method.AddStatement($"services.AddScoped<{this.GetEventContextName()}>();", s => s.SeparatedFromPrevious());
                        method.AddStatement($"services.AddScoped<{this.GetEventContextInterfaceName()}, {this.GetEventContextName()}>(sp => sp.GetRequiredService<{this.GetEventContextName()}>());");
                        method.AddStatement($"services.AddScoped<{this.GetAzureEventGridPublisherPipelineName()}>();");
                        method.AddStatement($"services.AddScoped<{this.GetAzureEventGridConsumerPipelineName()}>();");

                        method.AddStatement($"services.AddScoped<{this.GetAzureEventGridConsumerBehaviorInterfaceName()}, {this.GetInboundCloudEventBehaviorName()}>();", s => s.SeparatedFromPrevious());
                        
                        var publishMessages = IntegrationManager.Instance.GetPublishedAzureEventGridMessages(ExecutionContext.GetApplicationConfig().Id);
                        if (publishMessages.Count != 0)
                        {
                            ConfigurePublisherOptions(method, publishMessages);
                            
                            if (isCompositeMode)
                            {
                                method.AddStatement("", s => s.SeparatedFromPrevious());
                                method.AddStatement("// Register message types with the composite message bus registry");
                                foreach (var publishMessage in publishMessages)
                                {
                                    method.AddStatement($"registry.Register<{publishMessage.GetModelTypeName(this)}, {this.GetAzureEventGridEventBusName()}>();");
                                }
                            }
                        }
                            
                        var eventHandlers = IntegrationManager.Instance.GetSubscribedAzureEventGridMessages(ExecutionContext.GetApplicationConfig().Id);
                        if (eventHandlers.Count != 0)
                        {
                            method.AddInvocationStatement($"services.Configure<{this.GetAzureEventGridSubscriptionOptionsName()}>", inv => inv
                                .AddArgument(new CSharpLambdaBlock("options"), arg =>
                                {
                                    foreach (var eventHandler in eventHandlers)
                                    {
                                        arg.AddStatement($"""options.Add<{eventHandler.GetModelTypeName(this)}, {eventHandler.GetSubscriberTypeName(this)}>();""");
                                    }
                                }));
                        }

                        method.AddStatement("return services;");
                    });
            });
    }

    private void ConfigurePublisherOptions(CSharpClassMethod method, IReadOnlyList<AzureEventGridMessage> publishMessages)
    {
        var customTopicMessages = publishMessages.Where(x => x.DomainName is null).ToList();
        var eventDomainMessages = publishMessages.Where(x => x.DomainName is not null).ToList();

        method.AddInvocationStatement($"services.Configure<{this.GetAzureEventGridPublisherOptionsName()}>", inv => inv
            .AddArgument(new CSharpLambdaBlock("options"), arg =>
            {
                // Configure Custom Topics
                foreach (var publishMessage in customTopicMessages)
                {
                    arg.AddStatement(
                        $"""options.AddTopic<{publishMessage.GetModelTypeName(this)}>(configuration["{publishMessage.TopicConfigurationKeyName}"]!, configuration["{publishMessage.TopicConfigurationEndpointName}"]!, configuration["{publishMessage.TopicConfigurationSourceName}"]!);""");
                }

                // Configure Event Domains
                var domainGroups = eventDomainMessages.GroupBy(x => x.DomainName);
                foreach (var domainGroup in domainGroups)
                {
                    var domainConfig = domainGroup.First();
                    arg.AddInvocationStatement("options.AddDomain", domainInv => domainInv
                        .AddArgument($"""configuration["{domainConfig.DomainConfigurationKeyName}"]!""")
                        .AddArgument($"""configuration["{domainConfig.DomainConfigurationEndpointName}"]!""")
                        .AddArgument(new CSharpLambdaBlock("domain"), domainArg =>
                        {
                            foreach (var publishMessage in domainGroup)
                            {
                                domainArg.AddStatement($"""domain.Add<{publishMessage.GetModelTypeName(this)}>(configuration["{publishMessage.TopicConfigurationSourceName}"]!);""");
                            }
                        }));
                }
            }));
    }

    public override void BeforeTemplateExecution()
    {
        ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
            .ToRegister("ConfigureEventGrid", ServiceConfigurationRequest.ParameterType.Configuration)
            .HasDependency(this)
            .ForConcern("Infrastructure"));

        var allMessages = IntegrationManager.Instance.GetAggregatedAzureEventGridMessages(ExecutionContext.GetApplicationConfig().Id);
        foreach (var message in allMessages)
        {
            if (message.MethodType != AzureEventGridMethodType.Publish)
            {
                continue;
            }
            
            this.ApplyAppSetting(message.TopicConfigurationSourceName, message.TopicName.ToKebabCase());
                
            if (message.DomainName is not null)
            {
                this.ApplyAppSetting(message.DomainConfigurationKeyName, "");
                this.ApplyAppSetting(message.DomainConfigurationEndpointName, "");
            }
            else
            {
                this.ApplyAppSetting(message.TopicConfigurationKeyName, "");
                this.ApplyAppSetting(message.TopicConfigurationEndpointName, "");
            }
        }
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