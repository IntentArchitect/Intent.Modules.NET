using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.JsonPatch.Templates.Templates.JsonMergePatchExecutor;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class JsonMergePatchExecutorTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.AspNetCore.JsonPatch.Templates.JsonMergePatchExecutorTemplate";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public JsonMergePatchExecutorTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
        AddNugetDependency(NugetPackages.MorcatkoAspNetCoreJsonMergePatchNewtonsoftJson(outputTarget));

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("Morcatko.AspNetCore.JsonMergePatch")
            .AddClass("JsonMergePatchExecutor", @class =>
            { 
                @class
                    .AddGenericParameter("T", out var T)
                    .AddGenericTypeConstraint(T, constraint => constraint.AddType("class"));

                @class.ImplementsInterface($"{this.GetPatchExecutorInterfaceTemplateName()}<T>");

                @class.AddConstructor(ctor =>
                {
                    @class.AddField($"JsonMergePatchDocument<{T}>", "_document", field => field.PrivateReadOnly());
                    ctor.AddParameter($"JsonMergePatchDocument<{T}>", "document");
                    ctor.AddStatement("_document = document ?? throw new ArgumentNullException(nameof(document));");

                    if (HasFluentValidation())
                    {
                        AddUsing("FluentValidation");
                        @class.AddField("IValidatorProvider", "_validatorProvider", field => field.PrivateReadOnly());
                        ctor.AddParameter("IValidatorProvider", "validatorProvider");
                        ctor.AddStatement("_validatorProvider = validatorProvider ?? throw new ArgumentNullException(nameof(validatorProvider));");
                    }
                });

                @class.AddMethod("void", "ApplyTo", method =>
                {
                    method.AddParameter(T, "target");

                    method.AddStatement("ArgumentNullException.ThrowIfNull(target);")
                        .AddStatement("_document.ApplyTo(target);", stmt => stmt.SeparatedFromPrevious());

                    if (HasFluentValidation())
                    {
                        method.AddStatement($"var validator = _validatorProvider.GetValidator<{T}>();", stmt => stmt.SeparatedFromPrevious())
                            .AddStatement("var validationResult = validator.Validate(target);", stmt => stmt.SeparatedFromPrevious());

                        method.AddIfStatement("!validationResult.IsValid",
                            ifStmt => { ifStmt.AddStatement("throw new ValidationException(validationResult.Errors);"); });
                    }
                });
            });
    }

    private bool HasFluentValidation()
    {
        return TryGetTypeName("Application.Common.ValidatorProviderInterface", out _);
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