using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing.GoogleCloud.PubSub.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.ConfigurationTemplates.GoogleCloudPubSubConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class GoogleCloudPubSubConfigurationTemplate : CSharpTemplateBase<IList<EventingPackageModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.GoogleCloud.PubSub.ConfigurationTemplates.GoogleCloudPubSubConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public GoogleCloudPubSubConfigurationTemplate(IOutputTarget outputTarget, IList<EventingPackageModel> model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.GoogleCloudPubSubV1);
            AddNugetDependency(NugetPackages.MicrosoftExtensionsHostingAbstractions(outputTarget));
            AddNugetDependency(NugetPackages.MicrosoftExtensionsOptionsConfigurationExtensions(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddClass($"GoogleCloudPubSubConfiguration")
                .OnBuild(file =>
                {
                    var priClass = file.Classes.First();
                    priClass.Static();
                    priClass.AddMethod("IServiceCollection", "RegisterGoogleCloudPubSubServices", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", parm => parm.WithThisModifier());
                        method.AddParameter("IConfiguration", "configuration");
                        method.AddStatements($@"
services.Configure<{this.GetPubSubOptionsName()}>(configuration.GetSection(""GoogleCloudPubSub""));
services.AddScoped<{this.GetGooglePubSubEventBusName()}>();
services.AddTransient<{this.GetEventBusInterfaceName()}>(provider => provider.GetService<{this.GetGooglePubSubEventBusName()}>());
services.AddSingleton<{this.GetGoogleCloudResourceManagerName()}>();
services.AddTransient<{this.GetCloudResourceManagerInterfaceName()}>(provider => provider.GetService<{this.GetGoogleCloudResourceManagerName()}>());
return services;");
                    });
                    priClass.AddMethod("IServiceCollection", "AddSubscribers", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", parm => parm.WithThisModifier());
                        method.AddStatements(Model.Where(HasConsumingMessage).Select(packageModel =>
                            $@"services.AddHostedService(provider => new {this.GetGoogleSubscriberBackgroundServiceName()}(provider, ""{Helper.GetSubscriptionId(OutputTarget.ApplicationName(), packageModel)}"", ""{packageModel.GetGoogleCloudSettings().TopicId()}""));"));
                        method.AddStatement($@"return services;");
                    });
                    priClass.AddMethod("IServiceCollection", "RegisterTopicEvents", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", parm => parm.WithThisModifier());
                        method.AddStatement(new CSharpInvocationStatement($@"services.AddSingleton")
                            .AddArgument(new CSharpLambdaBlock("provider")
                                .AddStatements(
                                    $@"
var topicEventManager = new {this.GetGoogleEventBusTopicEventManagerName()}(provider.GetService<{this.GetCloudResourceManagerInterfaceName()}>());
RegisterTopicEvents(topicEventManager);
return topicEventManager;"))
                            .WithArgumentsOnNewLines());
                        method.AddStatement($@"services.AddTransient<{this.GetEventBusTopicEventManagerInterfaceName()}>(provider => provider.GetService<{this.GetGoogleEventBusTopicEventManagerName()}>());");
                        method.AddStatement($@"return services;");
                    });
                    priClass.AddMethod("void", "RegisterTopicEvents", method =>
                    {
                        method.Static();
                        method.Private();
                        method.AddParameter(this.GetGoogleEventBusTopicEventManagerName(), "topicEventManager");
                        method.AddStatements(Model
                            .SelectMany(packageModel => packageModel.Messages.Select(message => new { Message = message, Package = packageModel }))
                            .Select(x => $@"topicEventManager.RegisterTopicEvent<{GetMessageName(x.Message)}>(""{x.Package.GetGoogleCloudSettings().TopicId()}"");"));
                    });
                    priClass.AddMethod("IServiceCollection", "RegisterEventHandlers", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", parm => parm.WithThisModifier());
                        method.AddStatements($@"
var subscriptionManager = new {this.GetGoogleEventBusSubscriptionManagerName()}();
services.AddSingleton(subscriptionManager);
services.AddTransient<{this.GetEventBusSubscriptionManagerInterfaceName()}>(provider => provider.GetService<{this.GetGoogleEventBusSubscriptionManagerName()}>());");

                        method.AddStatements(Model.SelectMany(package => package.Messages).Select(message => $@"subscriptionManager.RegisterEventHandler<{GetMessageName(message)}>();"));
                        method.AddStatement("return services;");
                    });
                });
        }

        private bool HasConsumingMessage(EventingPackageModel packageModel)
        {
            return packageModel.Messages.Any(message => message.ConsumingApplications().Any());
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister(extensionMethodName: "RegisterGoogleCloudPubSubServices", extensionMethodParameterList: ServiceConfigurationRequest.ParameterType.Configuration)
                .ForConcern("Infrastructure")
                .HasDependency(this));
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister(extensionMethodName: "AddSubscribers")
                .ForConcern("Infrastructure")
                .HasDependency(this));
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister(extensionMethodName: "RegisterEventHandlers")
                .ForConcern("Infrastructure")
                .HasDependency(this));
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister(extensionMethodName: "RegisterTopicEvents")
                .ForConcern("Infrastructure")
                .HasDependency(this));
        }

        private string GetMessageName(MessageModel messageModel)
        {
            return GetTypeName("Intent.Eventing.Contracts.IntegrationEventMessage", messageModel.InternalElement);
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