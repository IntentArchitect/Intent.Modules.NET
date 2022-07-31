using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Application.FluentValidation.Templates.ValidationBehaviour;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.FluentValidation.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ValidatorBehaviourDecorator : ValidationBehaviourContract, IDecoratorExecutionHooks
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Application.MediatR.FluentValidation.ValidatorBehaviourDecorator";

        [IntentManaged(Mode.Fully)] private readonly ValidationBehaviourTemplate _template;
        [IntentManaged(Mode.Fully)] private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ValidatorBehaviourDecorator(ValidationBehaviourTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _template.AddUsing("MediatR");
        }

        public void BeforeTemplateExecution()
        {
            _template.ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister($"typeof({_template.ClassName}<,>)")
                .ForInterface("typeof(IPipelineBehavior<,>)")
                .WithPriority(4)
                .ForConcern("Application")
                .RequiresUsingNamespaces("MediatR")
                .HasDependency(_template));
        }

        public override IEnumerable<string> GetGenericTypeParameters()
        {
            yield return "TResponse";
        }

        public override IEnumerable<string> GetInheritanceDeclarations()
        {
            yield return "IPipelineBehavior<TRequest, TResponse>";
        }

        public override IEnumerable<string> GetGenericTypeConstraints()
        {
            yield return "where TRequest : IRequest<TResponse>";
        }

        public override IEnumerable<string> GetHandleParameterList()
        {
            yield return "RequestHandlerDelegate<TResponse> next";
        }

        public override string GetHandleReturnType()
        {
            return "TResponse";
        }

        public override IEnumerable<string> GetHandleExitStatementList()
        {
            yield return "return await next();";
        }
    }
}