using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Behaviours.Templates.UnitOfWorkBehaviour
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class UnitOfWorkBehaviourTemplate : CSharpTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.MediatR.Behaviours.UnitOfWorkBehaviour";

        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public UnitOfWorkBehaviourTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            
        }

        public override bool CanRunTemplate()
        {
            var hasUnitOfWorkInterface =
                ExecutionContext.FindTemplateInstance<IClassProvider>(TemplateFulfillingRoles.Domain.UnitOfWork) != null ||
                ExecutionContext.FindTemplateInstance<IClassProvider>(TemplateFulfillingRoles.Application.Common.DbContextInterface) != null; 
            var hasUnitOfWorkConcrete = ExecutionContext.FindTemplateInstance<IClassProvider>("Infrastructure.Data.DbContext") != null;
            return hasUnitOfWorkInterface && hasUnitOfWorkConcrete;
        }

        private string GetUnitOfWorkTypeName()
        {
            var result = TryGetTypeName(TemplateFulfillingRoles.Domain.UnitOfWork, out var unitOfWorkTypeName) || 
                TryGetTypeName(TemplateFulfillingRoles.Application.Common.DbContextInterface, out unitOfWorkTypeName);
            
            return unitOfWorkTypeName;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"UnitOfWorkBehaviour",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
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
                .ForConcern("Application")
                .RequiresUsingNamespaces("MediatR")
                .HasDependency(this));
        }
    }
}