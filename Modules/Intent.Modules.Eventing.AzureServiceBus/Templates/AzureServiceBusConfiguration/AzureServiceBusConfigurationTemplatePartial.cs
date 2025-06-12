using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
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
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Integration.IaC.Shared;
using Intent.Modules.Integration.IaC.Shared.AzureServiceBus;
using Intent.Modules.Modelers.Eventing;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureServiceBus.Templates.AzureServiceBusConfiguration;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class AzureServiceBusConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Eventing.AzureServiceBus.AzureServiceBusConfiguration";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public AzureServiceBusConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("System")
            .AddUsing("Microsoft.Extensions.Configuration")
            .AddUsing("Microsoft.Extensions.DependencyInjection")
            .AddUsing("Azure.Messaging.ServiceBus")
            .AddClass($"AzureServiceBusConfiguration", @class =>
            {
                @class.Static()
                    .AddMethod("IServiceCollection", "ConfigureAzureServiceBus", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", param => param.WithThisModifier());
                        method.AddParameter("IConfiguration", "configuration");

                        method.AddStatement($@"services.AddSingleton<ServiceBusClient>(sp => new ServiceBusClient(configuration[""AzureServiceBus:ConnectionString""]));");
                            
                        method.AddStatement($"services.AddScoped<{this.GetEventBusInterfaceName()}, {this.GetAzureServiceBusEventBusName()}>();");
                        method.AddStatement($"services.AddSingleton<{this.GetAzureServiceBusMessageDispatcherName()}>();");
                        method.AddStatement($"services.AddSingleton<{this.GetAzureServiceBusMessageDispatcherInterfaceName()}, {this.GetAzureServiceBusMessageDispatcherName()}>();");
                            
                        var publishers = IntegrationManager.Instance.GetAggregatedPublishedAzureServiceBusItems(ExecutionContext.GetApplicationConfig().Id);
                        if (publishers.Count != 0)
                        {
                            method.AddInvocationStatement($"services.Configure<{this.GetAzureServiceBusPublisherOptionsName()}>", inv => inv
                                .AddArgument(new CSharpLambdaBlock("options"), arg =>
                                {
                                    foreach (var item in publishers)
                                    {
                                        arg.AddStatement(
                                            $"""options.Add<{item.GetModelTypeName(this)}>(configuration["{item.QueueOrTopicConfigurationName}"]!);""");
                                    }
                                }));
                        }

                        var subscribers = IntegrationManager.Instance.GetAggregatedSubscribedAzureServiceBusItems(ExecutionContext.GetApplicationConfig().Id);
                        if (subscribers.Count != 0)
                        {
                            method.AddInvocationStatement($"services.Configure<{this.GetAzureServiceBusSubscriptionOptionsName()}>", inv => inv
                                .AddArgument(new CSharpLambdaBlock("options"), arg =>
                                {
                                    foreach (var item in subscribers)
                                    {
                                        var inv = new CSharpInvocationStatement($"options.Add<{item.GetModelTypeName(this)}, {item.GetSubscriberTypeName(this)}>")
                                            .AddArgument($@"configuration[""{item.QueueOrTopicConfigurationName}""]!");
                                        if (!string.IsNullOrEmpty(item.QueueOrTopicSubscriptionConfigurationName))
                                        {
                                            inv.AddArgument($@"configuration[""{item.QueueOrTopicSubscriptionConfigurationName}""]");
                                        }
                                        arg.AddStatement(inv);
                                    }
                                }));
                        }

                        method.AddStatement("return services;");
                    });
            });
    }

    public override void BeforeTemplateExecution()
    {
        ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
            .ToRegister("ConfigureAzureServiceBus", ServiceConfigurationRequest.ParameterType.Configuration)
            .HasDependency(this)
            .ForConcern("Infrastructure"));
            
        foreach (var message in IntegrationManager.Instance.GetAggregatedAzureServiceBusItems(ExecutionContext.GetApplicationConfig().Id))
        {
            this.ApplyAppSetting(message.QueueOrTopicConfigurationName, message.QueueOrTopicName);
            if (message.QueueOrTopicSubscriptionConfigurationName != null)
            {
                this.ApplyAppSetting(message.QueueOrTopicSubscriptionConfigurationName, message.QueueOrTopicName + "-subscription");
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