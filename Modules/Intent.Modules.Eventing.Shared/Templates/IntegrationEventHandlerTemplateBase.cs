using System.Linq;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationCommand;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventHandlerInterface;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;

namespace Intent.Modules.Eventing.Shared.Templates;

public abstract class IntegrationEventHandlerTemplateBase : CSharpTemplateBase<IntegrationEventHandlerModel>, ICSharpFileBuilderTemplate
{
    public IntegrationEventHandlerTemplateBase(string templateId, IOutputTarget outputTarget, IntegrationEventHandlerModel model) : base(templateId, outputTarget, model)
    {
        AddTypeSource(TemplateRoles.Application.Command);
        AddTypeSource(TemplateRoles.Application.Query);
        AddTypeSource(TemplateRoles.Application.Contracts.Dto);
        AddTypeSource(TemplateRoles.Application.Contracts.Enum);
        AddTypeSource(TemplateRoles.Domain.Enum);

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("System")
            .AddUsing("System.Threading")
            .AddUsing("System.Threading.Tasks")
            .AddClass($"{Model.Name.ToPascalCase()}", @class =>
            {
                @class.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge());
                @class.AddConstructor(ctor => { ctor.AddAttribute(CSharpIntentManagedAttribute.Merge()); });

                foreach (var subscription in Model.IntegrationEventSubscriptions())
                {
                    @class.ImplementsInterface($"{this.GetIntegrationEventHandlerInterfaceName()}<{GetMessageName(subscription)}>");
                    @class.AddMethod("Task", "HandleAsync", method =>
                    {
                        method.Async();
                        method.AddParameter(this.GetIntegrationEventMessageName(subscription.TypeReference.Element.AsMessageModel()), "message");
                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.RepresentsModel(subscription);
                        method.RegisterAsProcessingHandlerForModel(subscription);
                    });
                }

                foreach (var subscription in Model.IntegrationCommandSubscriptions())
                {
                    @class.ImplementsInterface($"{this.GetIntegrationEventHandlerInterfaceName()}<{GetMessageName(subscription)}>");
                    @class.AddMethod("Task", "HandleAsync", method =>
                    {
                        method.Async();
                        method.AddParameter(this.GetIntegrationCommandName(subscription.TypeReference.Element.AsIntegrationCommandModel()), "message");
                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.RepresentsModel(subscription);
                        method.RegisterAsProcessingHandlerForModel(subscription);
                    });
                }
            })
            .AfterBuild(file =>
            {
                foreach (var handleMethod in file.Classes.First().Methods.Where(x => x.Name == "Handle"))
                {
                    if (handleMethod.Statements.Count == 0)
                    {
                        handleMethod.AddAttribute(CSharpIntentManagedAttribute.IgnoreBody());
                        handleMethod.AddStatement($"// TODO: Implement {handleMethod.Name} ({file.Classes.First().Name}) functionality");
                        handleMethod.AddStatement("throw new NotImplementedException(\"Implement your handler logic here...\");");
                    }
                }
            }, 1000);
    }

    private string GetMessageName(SubscribeIntegrationEventTargetEndModel subscription)
    {
        return this.GetIntegrationEventMessageName(subscription.TypeReference.Element.AsMessageModel());
    }

    private string GetMessageName(SubscribeIntegrationCommandTargetEndModel subscription)
    {
        return this.GetIntegrationCommandName(subscription.TypeReference.Element.AsIntegrationCommandModel());
    }

    public CSharpFile CSharpFile { get; }

    protected override CSharpFileConfig DefineFileConfig() => CSharpFile.GetConfig();

    public override string TransformText() => CSharpFile.ToString();

    public override void BeforeTemplateExecution()
    {
        foreach (var subscription in Model.IntegrationEventSubscriptions())
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForInterface($"{this.GetIntegrationEventHandlerInterfaceName()}<{GetMessageName(subscription)}>")
                .ForConcern("Application")
                .WithPriority(100)
                .HasDependency(GetTemplate<IClassProvider>(IntegrationEventHandlerInterfaceTemplate.TemplateId))
                .HasDependency(GetTemplate<IClassProvider>(IntegrationEventMessageTemplate.TemplateId, subscription.TypeReference.Element)));
        }

        foreach (var subscription in Model.IntegrationCommandSubscriptions())
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForInterface($"{this.GetIntegrationEventHandlerInterfaceName()}<{GetMessageName(subscription)}>")
                .ForConcern("Application")
                .WithPriority(100)
                .HasDependency(GetTemplate<IClassProvider>(IntegrationEventHandlerInterfaceTemplate.TemplateId))
                .HasDependency(GetTemplate<IClassProvider>(IntegrationCommandTemplate.TemplateId, subscription.TypeReference.Element)));
        }
    }
}