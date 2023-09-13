using Intent.Engine;
using Intent.Modules.Application.FluentValidation.Dtos.Templates.ValidationServiceInterface;
using Intent.Modules.Application.FluentValidation.Shared;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.FluentValidation.Dtos.Templates.ValidationService
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class ValidationServiceTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Application.FluentValidation.Dtos.ValidationService";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ValidationServiceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.FluentValidation);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"ValidationService",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        public override void BeforeTemplateExecution()
        {
            var interfaceTemplate = ExecutionContext.FindTemplateInstance<ValidationServiceInterfaceTemplate>(ValidationServiceInterfaceTemplate.TemplateId);
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(ClassName)
                .WithPriority(10)
                .ForConcern("Application")
                .ForInterface(interfaceTemplate.ClassName)
                .HasDependency(this)
                .HasDependency(interfaceTemplate));
        }
    }
}