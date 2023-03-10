using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventHandlerInterface;
using Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.MessageTemplates.GenericMessage;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.EventingTemplates.GenericIntegrationEventHandlerImplementation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class GenericIntegrationEventHandlerImplementationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.GoogleCloud.PubSub.EventingTemplates.GenericIntegrationEventHandlerImplementation";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public GenericIntegrationEventHandlerImplementationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"GenericEventHandler")
                .OnBuild(file =>
                {
                    var priClass = file.Classes.First();
                    priClass.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                    priClass.ImplementsInterface($"{this.GetIntegrationEventHandlerInterfaceName()}<{this.GetGenericMessageName()}>");
                    priClass.AddConstructor(ctor => ctor.AddAttribute(CSharpIntentManagedAttribute.Ignore()));
                    priClass.AddMethod("Task", "HandleAsync", method =>
                    {
                        method.Async();
                        method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored());
                        method.AddParameter(this.GetGenericMessageName(), "message")
                            .AddParameter("CancellationToken", "cancellationToken", parm => parm.WithDefaultValue("default"));
                        method.AddStatement("throw new NotImplementedException();");
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForInterface($"{this.GetIntegrationEventHandlerInterfaceName()}<{this.GetGenericMessageName()}>")
                .ForConcern("Application")
                .WithPriority(100)
                .HasDependency(GetTemplate<IClassProvider>(IntegrationEventHandlerInterfaceTemplate.TemplateId))
                .HasDependency(GetTemplate<IClassProvider>(GenericMessageTemplate.TemplateId)));
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