using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing.GoogleCloud.PubSub.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.ImplementationTemplates.GoogleSubscriberBackgroundService
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GoogleSubscriberBackgroundServiceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.GoogleCloud.PubSub.ImplementationTemplates.GoogleSubscriberBackgroundService";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public GoogleSubscriberBackgroundServiceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.GoogleCloudPubSubV1);
            
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Google.Cloud.PubSub.V1")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddClass($"GoogleSubscriberBackgroundService")
                .OnBuild(file =>
                {
                    var priClass = file.Classes.First();
                    priClass.ExtendsClass("BackgroundService");
                    priClass.AddConstructor(ctor => ctor
                        .AddParameter("IServiceProvider", "serviceProvider", parm => parm.IntroduceReadonlyField())
                        .AddParameter("string", "subscriptionId", parm => parm.IntroduceReadonlyField())
                        .AddParameter("string", "topicId", parm => parm.IntroduceReadonlyField()));
                    priClass.AddField("SubscriberClient", "_subscriberClient");

                    priClass.AddMethod("Task", "StartAsync", method =>
                    {
                        method.Override().Async();
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddStatement($"var resourceManager = _serviceProvider.GetService<{this.GetCloudResourceManagerInterfaceName()}>();");
                        method.AddStatement(new CSharpStatementBlock("if (resourceManager.ShouldSetupCloudResources)")
                            .AddStatement($"await resourceManager.CreateTopicIfNotExistAsync(_topicId, cancellationToken);")
                            .AddStatement($"await resourceManager.CreateSubscriptionIfNotExistAsync((_subscriptionId, _topicId), cancellationToken);"));
                        method.AddStatement($"var subscriptionName = SubscriptionName.FromProjectSubscription(resourceManager.ProjectId, _subscriptionId);");
                        method.AddStatement($"_subscriberClient = await SubscriberClient.CreateAsync(subscriptionName);");
                        method.AddStatement($"await base.StartAsync(cancellationToken);");
                    });

                    priClass.AddMethod("Task", "ExecuteAsync", method =>
                    {
                        method.Protected().Override().Async();
                        method.AddParameter("CancellationToken", "stoppingToken");
                        method.AddStatement(new CSharpStatementBlock("while (!stoppingToken.IsCancellationRequested)")
                            .AddStatement(new CSharpInvocationStatement("await _subscriberClient.StartAsync")
                                .AddArgument(new CSharpLambdaBlock("async (message, token)")
                                    .AddStatement($"var subscriptionManager = _serviceProvider.GetService<{this.GetEventBusSubscriptionManagerInterfaceName()}>();")
                                    .AddStatement($"await subscriptionManager.DispatchAsync(_serviceProvider, message, token);")
                                    .AddStatement($"return SubscriberClient.Reply.Ack;"))
                                .WithArgumentsOnNewLines()));
                    });

                    priClass.AddMethod("Task", "StopAsync", method =>
                    {
                        method.Override().Async();
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddStatement($"await _subscriberClient.StopAsync(cancellationToken);");
                        method.AddStatement($"await base.StopAsync(cancellationToken);");
                    });
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