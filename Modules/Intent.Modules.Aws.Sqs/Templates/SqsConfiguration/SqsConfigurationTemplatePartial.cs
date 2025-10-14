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
        public SqsConfigurationTemplate(IOutputTarget outputTarget, object? model = null) : base(TemplateId, outputTarget, model)
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
                            var ifStmt = method.AddIfStatement("!string.IsNullOrEmpty(serviceUrl)");
                            ifStmt.AddStatement("// LocalStack or custom endpoint");
                            ifStmt.AddInvocationStatement("services.AddSingleton<IAmazonSQS>", inv => inv
                                .AddArgument(new CSharpLambdaBlock("sp"), arg =>
                                {
                                    arg.AddStatement("""
                                        var sqsConfig = new AmazonSQSConfig
                                        {
                                            ServiceURL = serviceUrl,
                                            AuthenticationRegion = configuration["AWS:Region"] ?? "us-east-1"
                                        };
                                        """);
                                    arg.AddStatement("""
                                        return new AmazonSQSClient(
                                            new BasicAWSCredentials("test", "test"),
                                            sqsConfig
                                        );
                                        """);
                                }));
                            
                            var elseStmt = ifStmt.AddElseStatement();
                            elseStmt.AddStatement("// Production AWS");
                            elseStmt.AddStatement("services.AddAWSService<IAmazonSQS>();");

                            // Register event bus
                            method.AddStatement($"services.AddScoped<{this.GetEventBusInterfaceName()}, {this.GetTypeName(SqsEventBusTemplate.TemplateId)}>();", stmt => stmt.SeparatedFromPrevious());
                            
                            // Register dispatcher
                            method.AddStatement($"services.AddSingleton<{this.GetTypeName(SqsMessageDispatcherTemplate.TemplateId)}>();");
                            method.AddStatement($"services.AddSingleton<{this.GetTypeName(SqsMessageDispatcherInterfaceTemplate.TemplateId)}, {this.GetTypeName(SqsMessageDispatcherTemplate.TemplateId)}>(sp => sp.GetRequiredService<{this.GetTypeName(SqsMessageDispatcherTemplate.TemplateId)}>());");

                            // Configure publisher options (metadata-driven - placeholder for now)
                            method.AddStatement("");
                            method.AddInvocationStatement($"services.Configure<{this.GetTypeName(SqsPublisherOptionsTemplate.TemplateId)}>", inv => inv
                                .AddArgument(new CSharpLambdaBlock("options"), arg =>
                                {
                                    arg.AddStatement("// Publisher options will be configured here based on metadata");
                                    arg.AddStatement("// Example: options.AddQueue<YourMessageType>(configuration[\"Sqs:Queues:YourMessageType:QueueUrl\"]!);");
                                }));

                            // Configure subscription options (metadata-driven - placeholder for now)
                            method.AddInvocationStatement($"services.Configure<{this.GetTypeName(SqsSubscriptionOptionsTemplate.TemplateId)}>", inv => inv
                                .AddArgument(new CSharpLambdaBlock("options"), arg =>
                                {
                                    arg.AddStatement("// Subscription options will be configured here based on metadata");
                                    arg.AddStatement("// Example: options.Add<YourMessageType, YourMessageHandler>();");
                                }));

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
