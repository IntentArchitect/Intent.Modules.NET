using Intent.Engine;
using Intent.Modules.Application.FluentValidation.Templates.ValidationBehaviour;
using Intent.Modules.Application.ServiceImplementations.FluentValidation.Templates;
using Intent.Modules.Application.ServiceImplementations.FluentValidation.Templates.ValidationProvider;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Application.ServiceImplementations.FluentValidation.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ValidationBehaviourDecorator : ValidationBehaviourContract, IDecoratorExecutionHooks
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Application.ServiceImplementations.FluentValidation.ValidationBehaviourDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly ValidationBehaviourTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public ValidationBehaviourDecorator(ValidationBehaviourTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public void BeforeTemplateExecution()
        {
            _template.ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister($"typeof({_template.ClassName}<>)")
                .WithPriority(4)
                .ForConcern("Application")
                .HasDependency(_template));
            
            var validationTemplateProvider = _application.FindTemplateInstance<ValidationProviderTemplate>(ValidationProviderTemplate.TemplateId);
            if (validationTemplateProvider != null)
            {
                _template.ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(validationTemplateProvider.ClassName)
                    .WithPriority(5)
                    .ForConcern("Application")
                    .HasDependency(validationTemplateProvider));
            }
        }
    }
}