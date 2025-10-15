using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Aws.Sqs.Templates.SqsEventBus;
using Intent.Modules.Aws.Sqs.Templates.SqsMessageDispatcher;
using Intent.Modules.Aws.Sqs.Templates.SqsMessageDispatcherInterface;
using Intent.Modules.Aws.Sqs.Templates.SqsPublisherOptions;
using Intent.Modules.Aws.Sqs.Templates.SqsSubscriptionOptions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Integration.IaC.Shared.AwsSqs;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Aws.Sqs.Templates.SqsConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class SqsConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Aws.Sqs.SqsConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SqsConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.AwsSdkSqs(OutputTarget));
            AddNugetDependency(NugetPackages.AwsSdkExtensionsNetCoreSetup(OutputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Amazon.Runtime")
                .AddUsing("Amazon.SQS")
                .AddClass($"SqsConfiguration", @class =>
                {
                    @class.Static()
                        .AddMethod("IServiceCollection", "ConfigureSqs", method =>
                        {
                            method.Static();
                            method.AddParameter("IServiceCollection", "services", param => param.WithThisModifier());
                            method.AddParameter("IConfiguration", "configuration");

                            // Register SQS client (with LocalStack support)
                            method.AddStatement("""var serviceUrl = configuration["AWS:ServiceURL"];""");
                            method.AddIfStatement("!string.IsNullOrEmpty(serviceUrl)", ifStmt =>
                            {
                                ifStmt.AddStatement("// LocalStack or custom endpoint");
                                ifStmt.AddInvocationStatement("services.AddSingleton<IAmazonSQS>", inv => inv
                                    .AddArgument(new CSharpLambdaBlock("sp"), arg =>
                                    {
                                        arg.AddStatement(new CSharpAssignmentStatement(
                                            new CSharpVariableDeclaration("sqsConfig"),
                                            new CSharpObjectInitializerBlock("new AmazonSQSConfig")
                                                .AddInitStatement("ServiceURL", "serviceUrl")
                                                .AddInitStatement("AuthenticationRegion", @"configuration[""AWS:Region""] ?? ""us-east-1"""))
                                            .WithSemicolon());
                                        arg.AddReturn(new CSharpInvocationStatement("new AmazonSQSClient")
                                            .AddArgument(@"new BasicAWSCredentials(""test"", ""test"")")
                                            .AddArgument(@"sqsConfig")
                                            .WithoutSemicolon());
                                    }));
                            });

                            method.AddElseStatement(elseStmt =>
                            {
                                elseStmt.AddStatement("// Production AWS");
                                elseStmt.AddStatement("services.AddAWSService<IAmazonSQS>();");
                            });

                            // Register event bus
                            method.AddStatement($"services.AddScoped<{this.GetEventBusInterfaceName()}, {this.GetTypeName(SqsEventBusTemplate.TemplateId)}>();", stmt => stmt.SeparatedFromPrevious());

                            // Register dispatcher
                            method.AddStatement($"services.AddSingleton<{this.GetTypeName(SqsMessageDispatcherTemplate.TemplateId)}>();");
                            method.AddStatement($"services.AddSingleton<{this.GetTypeName(SqsMessageDispatcherInterfaceTemplate.TemplateId)}, {this.GetTypeName(SqsMessageDispatcherTemplate.TemplateId)}>(sp => sp.GetRequiredService<{this.GetTypeName(SqsMessageDispatcherTemplate.TemplateId)}>());");

                            // Get metadata from IntegrationManager
                            var publishers = IntegrationManager.Instance.GetAggregatedPublishedSqsItems(ExecutionContext.GetApplicationConfig().Id);
                            var subscriptions = IntegrationManager.Instance.GetAggregatedSqsSubscriptions(ExecutionContext.GetApplicationConfig().Id);

                            // Configure publisher options (metadata-driven)
                            if (publishers.Any())
                            {
                                method.AddStatement("");
                                method.AddInvocationStatement($"services.Configure<{this.GetTypeName(SqsPublisherOptionsTemplate.TemplateId)}>", inv => inv
                                    .AddArgument(new CSharpLambdaBlock("options"), arg =>
                                    {
                                        foreach (var publisher in publishers)
                                        {
                                            var messageType = publisher.GetModelTypeName(this);
                                            var configKey = $"\"{publisher.QueueConfigurationName}:QueueUrl\"";
                                            arg.AddStatement($"options.AddQueue<{messageType}>(configuration[{configKey}]!);");
                                        }
                                    }));
                            }

                            // Register event handlers (metadata-driven)
                            if (subscriptions.Any())
                            {
                                method.AddStatement("");
                                foreach (var subscription in subscriptions)
                                {
                                    var handlerType = subscription.SubscriptionItem.GetSubscriberTypeName(this);
                                    var handlerImplementation = this.GetTypeName("Intent.Eventing.Contracts.IntegrationEventHandler", subscription.EventHandlerModel);
                                    method.AddStatement($"services.AddTransient<{handlerType}, {handlerImplementation}>();");
                                }
                            }

                            // Configure subscription options (metadata-driven)
                            if (subscriptions.Any())
                            {
                                method.AddStatement("");
                                method.AddInvocationStatement($"services.Configure<{this.GetTypeName(SqsSubscriptionOptionsTemplate.TemplateId)}>", inv => inv
                                    .AddArgument(new CSharpLambdaBlock("options"), arg =>
                                    {
                                        foreach (var subscription in subscriptions)
                                        {
                                            var messageType = subscription.SubscriptionItem.GetModelTypeName(this);
                                            var handlerType = subscription.SubscriptionItem.GetSubscriberTypeName(this);
                                            arg.AddStatement($"options.Add<{messageType}, {handlerType}>();");
                                        }
                                    }));
                            }

                            method.AddStatement("return services;", stmt => stmt.SeparatedFromPrevious());
                        });
                });
        }

        public override void BeforeTemplateExecution()
        {
            this.ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureSqs", ServiceConfigurationRequest.ParameterType.Configuration)
                .HasDependency(this)
                .ForConcern("Infrastructure"));
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
}
