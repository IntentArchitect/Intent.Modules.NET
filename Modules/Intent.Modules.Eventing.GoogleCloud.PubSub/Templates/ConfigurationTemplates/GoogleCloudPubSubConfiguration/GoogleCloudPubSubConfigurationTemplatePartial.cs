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
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.ConfigurationTemplates.GoogleCloudPubSubConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GoogleCloudPubSubConfigurationTemplate : CSharpTemplateBase<IList<EventingPackageModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.GoogleCloud.PubSub.ConfigurationTemplates.GoogleCloudPubSubConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public GoogleCloudPubSubConfigurationTemplate(IOutputTarget outputTarget, IList<EventingPackageModel> model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.GoogleCloudPubSubV1);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"GoogleCloudPubSubConfiguration")
                .OnBuild(file =>
                {
                    var priClass = file.Classes.First();
                    priClass.Static();
                    priClass.AddMethod("IServiceCollection", "AddSubscribers", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", parm => parm.WithThisModifier());
                        method.AddStatements(Model.Select(packageModel =>
                            $@"services.AddHostedService(provider => new {this.GetGoogleCloudPubSubSubscriberBackgroundServiceName()}(provider, ""{Helper.GetSubscriptionId(OutputTarget.ApplicationName(), packageModel)}"", ""{packageModel.GetGoogleCloudSettings().TopicId()}""));"));
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
                            .Select(x => $@"topicEventManager.RegisterTopicEvent<{GetTypeName(x.Message.InternalElement)}>(""{x.Package.GetGoogleCloudSettings().TopicId()}"");"));
                    });
                    priClass.AddMethod("IServiceCollection", "RegisterEventHandlers", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", parm => parm.WithThisModifier());
                        method.AddStatements($@"
var subscriptionManager = new {this.GetGoogleEventBusSubscriptionManagerName()}();
services.AddSingleton(subscriptionManager);
services.AddTransient<{this.GetEventBusSubscriptionManagerInterfaceName()}>(provider => provider.GetService<{this.GetGoogleEventBusSubscriptionManagerName()}>());");

                        method.AddStatements(Model.SelectMany(package => package.Messages).Select(message => $@"subscriptionManager.RegisterEventHandler<{GetTypeName(message.InternalElement)}>();"));
                        method.AddStatement("return services;");
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("AddSubscribers")
                .ForConcern("Infrastructure")
                .HasDependency(this));
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("RegisterEventHandlers")
                .ForConcern("Infrastructure")
                .HasDependency(this));
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("RegisterTopicEvents")
                .ForConcern("Infrastructure")
                .HasDependency(this));
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