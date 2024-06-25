using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventHandlerInterface;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;

namespace Intent.Modules.Eventing.Shared.Templates;

public abstract class IntegrationEventHandlerImplementationTemplateBase : CSharpTemplateBase<MessageSubscribeAssocationTargetEndModel>, ICSharpFileBuilderTemplate
{
    protected IntegrationEventHandlerImplementationTemplateBase(string templateId, IOutputTarget outputTarget, MessageSubscribeAssocationTargetEndModel model)
        : base(templateId, outputTarget, model)
    {
        AddTypeSource(IntegrationEventMessageTemplate.TemplateId);

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("System")
            .AddUsing("System.Threading")
            .AddUsing("System.Threading.Tasks")
            .AddClass($"{Model.TypeReference.Element.Name.RemoveSuffix("Event").ToPascalCase()}EventHandler", @class =>
            {
                @class.AddAttribute("[IntentManaged(Mode.Merge, Signature = Mode.Fully)]");
                @class.WithBaseType($"{this.GetIntegrationEventHandlerInterfaceName()}<{GetMessageName()}>");

                @class.AddConstructor(constructor =>
                {
                    constructor.AddAttribute("[IntentManaged(Mode.Merge)]");
                });

                @class.AddMethod("void", "HandleAsync", method =>
                {
                    method.AddAttribute("[IntentManaged(Mode.Fully, Body = Mode.Ignore)]");
                    method.Async();
                    method.AddParameter(GetMessageName(), "message");
                    method.AddOptionalCancellationTokenParameter(this);

                    method.AddStatement($"// TODO: Implement {method.Name} ({@class.Name}) functionality");
                    method.AddStatement("throw new NotImplementedException();");
                });
            });
    }

    public override void BeforeTemplateExecution()
    {
        ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
            .ForInterface($"{this.GetIntegrationEventHandlerInterfaceName()}<{GetMessageName()}>")
            .ForConcern("Application")
            .WithPriority(100)
            .HasDependency(GetTemplate<IClassProvider>(IntegrationEventHandlerInterfaceTemplate.TemplateId))
            .HasDependency(GetTemplate<IClassProvider>(IntegrationEventMessageTemplate.TemplateId, Model.TypeReference.Element)));
    }

    private string GetMessageName()
    {
        return GetTypeName("Intent.Eventing.Contracts.IntegrationEventMessage", Model.TypeReference.Element);
    }

    public CSharpFile CSharpFile { get; }

    protected override CSharpFileConfig DefineFileConfig() => CSharpFile.GetConfig();

    public override string TransformText() => CSharpFile.ToString();
}