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

namespace Intent.Modules.Application.Wolverine.Templates.ValidationMiddleware
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ValidationMiddlewareTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Wolverine.ValidationMiddleware";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ValidationMiddlewareTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Linq")
                .AddUsing("System.Reflection")
                .AddClass("ValidationMiddleware", @class =>
                {
                    var validatorProvider = GetTypeName("Intent.Application.FluentValidation.Dtos.ValidatorProviderInterface");
                    // Prefer the transport-agnostic shared role; fall back to the MediatR-specific one for backwards compatibility
                    var hasBypassInterface = TryGetTypeName("Application.Common.BypassValidationInterface", out var bypassInterface)
                        || TryGetTypeName("Intent.Application.MediatR.FluentValidation.BypassPipelineValidationInterface", out bypassInterface);
                    @class.AddMethod(UseType("System.Threading.Tasks.Task"), "BeforeAsync", method =>
                    {
                        method.Async();
                        method.AddParameter(UseType("Wolverine.Envelope"), "envelope");
                        method.AddParameter(validatorProvider, "validatorProvider");
                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");
                        method.AddStatement("await ValidateAsync(envelope.Message, validatorProvider, cancellationToken);");
                    });

                    @class.AddMethod(UseType("System.Threading.Tasks.Task"), "ValidateAsync", method =>
                    {
                        method.Static().Async().Private();
                        method.AddParameter("object", "request");
                        method.AddParameter(validatorProvider, "validatorProvider");
                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");

                        if (hasBypassInterface)
                        {
                            method.AddIfStatement($"request is {bypassInterface}", @if =>
                            {
                                @if.AddStatement("return;");
                            });
                        }

                        method.AddStatement("var validator = GetValidator(request, validatorProvider);");
                        method.AddIfStatement("validator is null", @if =>
                        {
                            @if.AddStatement("return;");
                        });

                        method.AddStatement($"var context = new {UseType("FluentValidation.ValidationContext<object>")}(request);");
                        method.AddStatement("var validationResult = await validator.ValidateAsync(context, cancellationToken);");
                        method.AddStatement("var failures = validationResult.Errors.Where(error => error is not null).ToList();");
                        method.AddIfStatement("failures.Count != 0", @if =>
                        {
                            @if.AddStatement($"throw new {UseType("FluentValidation.ValidationException")}(failures);");
                        });
                    });

                    @class.AddMethod($"{UseType("FluentValidation.IValidator")}?", "GetValidator", method =>
                    {
                        method.Static().Private();
                        method.AddParameter("object", "request");
                        method.AddParameter(validatorProvider, "validatorProvider");

                        method.AddStatement($"var providerMethod = typeof({validatorProvider}).GetMethod(nameof({validatorProvider}.GetValidator))!.MakeGenericMethod(request.GetType());");
                        method.AddStatement($"return providerMethod.Invoke(validatorProvider, null) as {UseType("FluentValidation.IValidator")};");
                    });
                });
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
