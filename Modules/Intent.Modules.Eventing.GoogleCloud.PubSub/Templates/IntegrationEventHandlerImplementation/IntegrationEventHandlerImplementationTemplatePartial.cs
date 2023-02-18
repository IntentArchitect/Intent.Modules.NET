using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.IntegrationEventHandlerImplementation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class IntegrationEventHandlerImplementationTemplate : CSharpTemplateBase<MessageSubscribeAssocationTargetEndModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.GoogleCloud.PubSub.IntegrationEventHandlerImplementation";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntegrationEventHandlerImplementationTemplate(IOutputTarget outputTarget, MessageSubscribeAssocationTargetEndModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"{Model.TypeReference.Element.Name.RemoveSuffix("Event").ToPascalCase()}EventHandler")
                .OnBuild(file =>
                {
                    var priClass = file.Classes.First();
                    priClass.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                    priClass.ImplementsInterface($"{this.GetIntegrationEventHandlerInterfaceName()}<{GetTypeName(Model.TypeReference)}>");
                    priClass.AddConstructor(ctor => ctor.AddAttribute(CSharpIntentManagedAttribute.Ignore()));
                    priClass.AddMethod("Task", "HandleAsync", method =>
                    {
                        method.Async();
                        method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored());
                        method.AddParameter(GetTypeName(Model.TypeReference), "message")
                            .AddParameter("CancellationToken", "cancellationToken", parm => parm.WithDefaultValue("default"));
                        method.AddStatement("throw new NotImplementedException();");
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