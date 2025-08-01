using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.UnitOfWork.Persistence.Shared;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureServiceBus.Templates.AzureServiceBusHostedService
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureServiceBusHostedServiceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.AzureServiceBus.AzureServiceBusHostedService";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureServiceBusHostedServiceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Transactions")
                .AddUsing("Azure.Messaging.ServiceBus")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddUsing("Microsoft.Extensions.Logging")
                .AddUsing("Microsoft.Extensions.Options")
                .AddClass($"AzureServiceBusHostedService", @class =>
                {
                    @class.WithBaseType("BackgroundService")
                        .ImplementsInterface("IAsyncDisposable");

                    @class.AddConstructor(ctor =>
                    {
                        ctor
                            .AddParameter("IServiceProvider", "rootServiceProvider", param => param.IntroduceReadonlyField())
                            .AddParameter(this.GetAzureServiceBusMessageDispatcherInterfaceName(), "dispatcher", param => param.IntroduceReadonlyField())
                            .AddParameter("ServiceBusClient", "serviceBusClient", param => param.IntroduceReadonlyField())
                            .AddParameter("ILogger<AzureServiceBusHostedService>", "logger", param => param.IntroduceReadonlyField())
                            .AddParameter($"IOptions<{this.GetAzureServiceBusSubscriptionOptionsName()}>", "subscriptionOptions");
                        ctor.AddStatement("_subscriptionOptions = subscriptionOptions.Value;");
                    });

                    @class.AddField(this.GetAzureServiceBusSubscriptionOptionsName(), "_subscriptionOptions", field => field.PrivateReadOnly())
                        .AddField("List<ServiceBusProcessor>", "_processors", field => field.WithAssignment("[]").PrivateReadOnly());

                    @class.AddMethod("Task", "ExecuteAsync", method =>
                    {
                        method.Protected().Override().Async();
                        method.AddParameter("CancellationToken", "stoppingToken");
                        method.AddForEachStatement("subscription", "_subscriptionOptions.Entries", f =>
                        {
                            f.AddStatement("var processor = CreateProcessor(subscription, _serviceBusClient);");
                            f.AddStatement("processor.ProcessMessageAsync += args => ProcessMessageAsync(args, stoppingToken);");
                            f.AddStatement("processor.ProcessErrorAsync += ProcessErrorAsync;");
                            f.AddStatement("_processors.Add(processor);");
                            f.AddStatement("await processor.StartProcessingAsync(stoppingToken);");
                        });
                        method.AddStatement("await Task.Delay(Timeout.Infinite, stoppingToken);");
                    });

                    @class.AddMethod("ServiceBusProcessor", "CreateProcessor", method =>
                    {
                        method.Private().Static();
                        method.AddParameter(this.GetSubscriptionEntryName(), "subscription");
                        method.AddParameter("ServiceBusClient", "serviceBusClient");
                        method.AddStatement(new CSharpAssignmentStatement(new CSharpVariableDeclaration("options"),
                            new CSharpObjectInitializerBlock("new ServiceBusProcessorOptions")
                                .AddInitStatement("AutoCompleteMessages", "false")
                                .AddInitStatement("MaxConcurrentCalls", "1")
                                .AddInitStatement("PrefetchCount", "0")
                        ).WithSemicolon());
                        method.AddReturn(new CSharpConditionalExpressionStatement(
                            "subscription.SubscriptionName != null",
                            "serviceBusClient.CreateProcessor(subscription.QueueOrTopicName, subscription.SubscriptionName, options)",
                            "serviceBusClient.CreateProcessor(subscription.QueueOrTopicName, options)"));
                    });

                    @class.AddMethod("Task", "ProcessMessageAsync", method =>
                    {
                        method.Private().Async();
                        method.AddParameter("ProcessMessageEventArgs", "args");
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddTryBlock(tryBlock =>
                        {
                            tryBlock.AddStatement("using var scope = _rootServiceProvider.CreateScope();");
                            tryBlock.AddStatement("var scopedServiceProvider = scope.ServiceProvider;");
                            
                            tryBlock.AddStatement(new CSharpAssignmentStatement(
                                new CSharpVariableDeclaration("eventBus"),
                                $"scopedServiceProvider.GetRequiredService<{this.GetTypeName(TemplateRoles.Application.Eventing.EventBusInterface)}>()").WithSemicolon());
                            
                            tryBlock.ApplyUnitOfWorkImplementations(
                                template: this, 
                                constructor: @class.Constructors.First(), 
                                invocationStatement: "await _dispatcher.DispatchAsync(scopedServiceProvider, args.Message, cancellationToken);",
                                configure: config =>
                                {
                                    config.UseServiceProvider("scopedServiceProvider");
                                });
                            
                            tryBlock.AddStatement("await eventBus.FlushAllAsync(cancellationToken);");
                            tryBlock.AddStatement("await args.CompleteMessageAsync(args.Message, cancellationToken);");
                        });
                        method.AddCatchBlock("Exception", "ex", catchBlock => catchBlock
                            .AddStatement("_logger.LogError(ex, \"Error processing ServiceBus message\");")
                            .AddStatement("throw;")
                        );
                    });

                    @class.AddMethod("Task", "ProcessErrorAsync", method =>
                    {
                        method.Private();
                        method.AddParameter("ProcessErrorEventArgs", "args");
                        method.AddStatement("_logger.LogError(args.Exception, \"ServiceBus processing error\");");
                        method.AddReturn("Task.CompletedTask");
                    });

                    @class.AddMethod("ValueTask", "DisposeAsync", method =>
                    {
                        method.Async(true);
                        method.AddForEachStatement("processor", "_processors", f =>
                        {
                            f.AddStatement("await processor.StopProcessingAsync();");
                            f.AddStatement("await processor.DisposeAsync();");
                        });
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            var appStartup = ExecutionContext.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            return appStartup != null;
        }

        public override void BeforeTemplateExecution()
        {
            var appStartup = ExecutionContext.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            appStartup.StartupFile.ConfigureServices((stmt, ctx) =>
            {
                stmt.AddStatement($@"{ctx.Services}.AddHostedService<{appStartup.GetAzureServiceBusHostedServiceName()}>();");
            });
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