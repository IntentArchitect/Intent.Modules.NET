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

                        method.AddStatement($"services.AddScoped<{this.GetEventBusInterfaceName()}, {this.GetAzureEventGridEventBusName()}>();");
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
                            method.AddInvocationStatement($"services.Configure<{this.GetAzureEventGridPublisherOptionsName()}>", inv => inv
                                .AddArgument(new CSharpLambdaBlock("options"), arg =>
                                {
                                    foreach (var publishMessage in publishMessages)
                                    {
                                        arg.AddStatement(
                                            $"""options.Add<{publishMessage.GetModelTypeName(this)}>(configuration["{publishMessage.TopicConfigurationKeyName}"]!, configuration["{publishMessage.TopicConfigurationEndpointName}"]!, configuration["{publishMessage.TopicConfigurationSourceName}"]!);""");
                                    }
                                }));
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

    public override void BeforeTemplateExecution()
    {
        ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
            .ToRegister("ConfigureEventGrid", ServiceConfigurationRequest.ParameterType.Configuration)
            .HasDependency(this)
            .ForConcern("Infrastructure"));

        foreach (var message in IntegrationManager.Instance.GetAggregatedAzureEventGridMessages(ExecutionContext.GetApplicationConfig().Id))
        {
            if (message.MethodType == AzureEventGridMethodType.Publish)
            {
                this.ApplyAppSetting(message.TopicConfigurationSourceName, message.TopicName.ToKebabCase());
            }
            this.ApplyAppSetting(message.TopicConfigurationKeyName, "");
            this.ApplyAppSetting(message.TopicConfigurationEndpointName, "");
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