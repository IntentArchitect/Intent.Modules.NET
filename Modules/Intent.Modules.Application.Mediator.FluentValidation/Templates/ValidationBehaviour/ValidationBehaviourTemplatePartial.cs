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

namespace Intent.Modules.Application.Mediator.FluentValidation.Templates.ValidationBehaviour
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ValidationBehaviourTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Mediator.FluentValidation.ValidationBehaviour";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ValidationBehaviourTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.FluentValidation(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass("ValidationBehaviour", @class =>
                {
                    @class.AddGenericParameter("TRequest", out var tRequest);
                    @class.AddGenericParameter("TResponse", out var tResponse);
                    @class.ImplementsInterface(UseType($"Mediator.IPipelineBehavior<{tRequest}, {tResponse}>"));
                    @class.AddGenericTypeConstraint(tRequest, c => c.AddType(UseType("Mediator.IMessage")));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(
                            UseType($"System.Collections.Generic.IEnumerable<{UseType($"FluentValidation.IValidator<{tRequest}>")}>"),
                            "validators",
                            param => param.IntroduceReadonlyField());
                    });

                    @class.AddMethod($"{UseType("System.Threading.Tasks.ValueTask")}<{tResponse}>", "Handle", method =>
                    {
                        method.Async(asValueTask: true);
                        method.AddParameter(tRequest, "request");
                        method.AddParameter(UseType($"Mediator.MessageHandlerDelegate<{tRequest}, {tResponse}>"), "next");
                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");

                        method.AddIfStatement($"request is {this.GetBypassValidationInterfaceName()}", bypass =>
                        {
                            bypass.AddReturn("await next(request, cancellationToken)");
                        });
                        method.AddIfStatement("_validators.Any()", anyValidator =>
                        {
                            anyValidator.AddStatement($"var context = new {UseType("FluentValidation.ValidationContext")}<{tRequest}>(request);");
                            anyValidator.AddStatement($"var failures = new {UseType("System.Collections.Generic.List")}<{UseType("FluentValidation.Results.ValidationFailure")}>();", s => s.SeparatedFromNext());

                            anyValidator.AddStatement("// One validator at a time: async rules (such as unique constraint checks) share the scoped DbContext, which does not support concurrent operations.");
                            anyValidator.AddForEachStatement("validator", "_validators", loop =>
                            {
                                loop.AddStatement("var result = await validator.ValidateAsync(context, cancellationToken);");
                                loop.AddStatement("failures.AddRange(result.Errors.Where(f => f != null));");
                            });
                            anyValidator.AddIfStatement("failures.Count != 0", failed =>
                            {
                                failed.AddStatement($"throw new {UseType("FluentValidation.ValidationException")}(failures);");
                            }).SeparatedFromPrevious();
                        });
                        method.AddStatement("return await next(request, cancellationToken);", s => s.SeparatedFromPrevious());
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