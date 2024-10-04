using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.MudBlazor.Templates.FluentValidationProviderExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class FluentValidationProviderExtensionsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Components.MudBlazor.FluentValidationProviderExtensions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public FluentValidationProviderExtensionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("FluentValidation")
                .AddClass($"ValidatorProviderExtensions", @class =>
                {
                    @class.Static();
                    @class.AddMethod("Func<object, string, Task<IEnumerable<string>>>", "GetValidationFunc", method =>
                    {
                        method.Static();
                        method.AddGenericParameter("TModel");
                        method.AddParameter(GetTypeName("Blazor.Client.Validation.ValidatorProviderInterface"), "provider", param => param.WithThisModifier());
                        method.AddStatements("""
                                             return async (model, propertyName) =>
                                             {
                                                 var result = await provider.GetValidator<TModel>().ValidateAsync(ValidationContext<TModel>.CreateWithOptions((TModel)model, x => x.IncludeProperties(propertyName)));
                                                 if (result.IsValid)
                                                     return Array.Empty<string>();
                                                 return result.Errors.Select(e => e.ErrorMessage);
                                             };
                                             """);
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return ExecutionContext.TemplateExists("Blazor.Client.Validation.ValidatorProviderInterface");
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