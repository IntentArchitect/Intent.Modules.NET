using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Behaviours.Templates.MongoDbUnitOfWorkBehaviour
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MongoDbUnitOfWorkBehaviourTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.MediatR.Behaviours.MongoDbUnitOfWorkBehaviour";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MongoDbUnitOfWorkBehaviourTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Transactions")
                .AddUsing("MediatR")
                .AddClass($"MongoDbUnitOfWorkBehaviour")
                .OnBuild(file =>
                {
                    if (!CanRunTemplate())
                    {
                        return;
                    }
                    
                    var @class = file.Classes.First();
                    @class.AddGenericParameter("TRequest", out var TRequest)
                        .AddGenericParameter("TResponse", out var TResponse);
                    @class.ImplementsInterface($"IPipelineBehavior<{TRequest}, {TResponse}>");
                    @class.AddGenericTypeConstraint(TRequest, type => type
                        .AddType($"notnull")
                        .AddType(GetTypeName("Application.Command.Interface")));
                    @class.AddConstructor(ctor =>
                        ctor.AddParameter(GetTypeName(TemplateFulfillingRoles.Domain.MongoDbUnitOfWork), "dataSource", param => param.IntroduceReadonlyField()));
                    @class.AddMethod($"Task<{TResponse}>", "Handle", method =>
                    {
                        method.Async();
                        method.AddParameter(TRequest, "request")
                            .AddParameter($"RequestHandlerDelegate<{TResponse}>", "next")
                            .AddParameter("CancellationToken", "cancellationToken");
                        method.AddStatement($"var response = await next();")
                            .AddStatement($"await _dataSource.SaveChangesAsync(cancellationToken);")
                            .AddStatement($"return response;");
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>("Domain.UnitOfWork.MongoDb") != null;
        }

        public override void BeforeTemplateExecution()
        {
            if (!CanRunTemplate())
            {
                return;
            }
            
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister($"typeof({ClassName}<,>)")
                .ForInterface("typeof(IPipelineBehavior<,>)")
                .WithPriority(4)
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