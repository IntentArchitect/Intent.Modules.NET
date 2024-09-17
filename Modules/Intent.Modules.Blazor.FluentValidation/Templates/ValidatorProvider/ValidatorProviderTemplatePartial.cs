using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Intent.Modules.Blazor.FluentValidation.Templates.ValidatorProvider
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ValidatorProviderTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.FluentValidation.ValidatorProvider";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ValidatorProviderTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("FluentValidation")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"ValidatorProvider", @class =>
                {
                    @class.ImplementsInterface(this.GetValidatorProviderInterfaceName());
                    @class.AddConstructor(ctor => ctor
                        .AddParameter(UseType("System.IServiceProvider"), "serviceProvider", param => param.IntroduceReadonlyField()));
                    @class.AddMethod($"IValidator<T>", "GetValidator", method => method
                        .AddGenericParameter("T")
                        .AddStatement($"return _serviceProvider.GetService<IValidator<T>>()!;"));
                });
        }




        // public override bool CanRunTemplate()
        // {
        //     var validatorTemplates = ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>(DtoValidatorTemplate.TemplateId);
        //     return validatorTemplates.Any(p => p.CSharpFile.Classes.FirstOrDefault()?.Constructors.FirstOrDefault()?.Parameters.Any(q => q.Type.Contains("IValidatorProvider")) == true);
        // }

        public override void BeforeTemplateExecution()
        {
            if (!TryGetTemplate<ITemplate>(ValidatorProviderInterface.ValidatorProviderInterfaceTemplate.TemplateId, out var template))
            {
                return;
            }

            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest
                .ToRegister(this)
                .ForConcern("BlazorClient")
                .ForInterface("IValidatorProvider")
                .HasDependency(template)
                .WithPerServiceCallLifeTime());
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