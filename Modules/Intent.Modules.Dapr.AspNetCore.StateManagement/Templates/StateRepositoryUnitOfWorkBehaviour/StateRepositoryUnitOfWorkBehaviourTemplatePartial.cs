using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.StateRepositoryUnitOfWorkBehaviour
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class StateRepositoryUnitOfWorkBehaviourTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.StateManagement.StateRepositoryUnitOfWorkBehaviour";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public StateRepositoryUnitOfWorkBehaviourTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("MediatR")
                .AddClass("StateRepositoryUnitOfWorkBehaviour", @class => @class
                    .AddGenericParameter("TRequest")
                    .AddGenericParameter("TResponse")
                    .ImplementsInterface("IPipelineBehavior<TRequest, TResponse>")
                    .AddGenericTypeConstraint("TRequest", c => c.AddType("IRequest<TResponse>"))
                    .AddConstructor(constructor => constructor
                        .AddParameter(this.GetStateRepositoryInterfaceName(), "stateRepository", p => p.IntroduceReadonlyField())
                    )
                    .AddMethod("Task<TResponse>", "Handle", method => method
                        .Async()
                        .AddParameter("TRequest", "request")
                        .AddParameter("CancellationToken", "cancellationToken")
                        .AddParameter("RequestHandlerDelegate<TResponse>", "next")
                        .AddStatement("var response = await next();")
                        .AddStatement("await _stateRepository.FlushAllAsync(cancellationToken);", s => s.SeparatedFromPrevious())
                        .AddStatement("return response;", s => s.SeparatedFromPrevious())
                    )
                );
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister($"typeof({ClassName}<,>)")
                .ForInterface("typeof(IPipelineBehavior<,>)")
                .WithPriority(6)
                .ForConcern("Application")
                .RequiresUsingNamespaces("MediatR")
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