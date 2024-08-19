using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.UnitOfWork.Persistence.Shared;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Behaviours.Templates.UnitOfWorkBehaviour
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class UnitOfWorkBehaviourTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.MediatR.Behaviours.UnitOfWorkBehaviour";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public UnitOfWorkBehaviourTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"UnitOfWorkBehaviour", @class =>
                {
                    @class.WithComments(new[]
                    {
                        "/// <summary>",
                        "/// Ensures that all operations processed as part of handling a <see cref=\"ICommand\"/> either",
                        "/// pass or fail as one unit. This behaviour makes it unnecessary for developers to call",
                        "/// SaveChangesAsync() inside their business logic (e.g. command handlers), and doing so should",
                        "/// be avoided unless absolutely necessary.",
                        "/// </summary>",
                    });
                    @class.AddGenericParameter("TRequest", out var tRequest);
                    @class.AddGenericParameter("TResponse", out var tResponse);
                    @class.ImplementsInterface($"{UseType("MediatR.IPipelineBehavior")}<{tRequest}, {tResponse}>");
                    @class.AddGenericTypeConstraint(tRequest, c => c
                        .AddType("notnull")
                        .AddType(this.GetCommandInterfaceName()));

                    @class.AddConstructor();

                    @class.AddMethod($"Task<{tResponse}>", "Handle", method =>
                    {
                        method.Async();
                        method.AddParameter(tRequest, "request");
                        method.AddParameter($"{UseType("MediatR.RequestHandlerDelegate")}<{tResponse}>", "next");
                        method.AddParameter("CancellationToken", "cancellationToken");

                        method.ApplyUnitOfWorkImplementations(
                            template: this,
                            constructor: @class.Constructors.First(),
                            invocationStatement: "await next();",
                            returnType: tResponse,
                            resultVariableName: "response",
                            fieldSuffix: "dataSource");
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && this.SystemUsesPersistenceUnitOfWork();
        }

        public override void BeforeTemplateExecution()
        {
            if (!CanRunTemplate())
            {
                return;
            }

            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister($"typeof({ClassName}<,>)")
                .ForInterface("typeof(IPipelineBehavior<,>)")
                .WithPriority(5)
                .ForConcern("MediatR")
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