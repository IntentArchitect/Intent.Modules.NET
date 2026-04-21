using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.JsonPatch.Templates.Templates.JsonMergePatchExecutor;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class JsonMergePatchExecutorTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.AspNetCore.Controllers.JsonPatch.Templates.JsonMergePatchExecutor";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public JsonMergePatchExecutorTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
        AddNugetDependency(NugetPackages.MorcatkoAspNetCoreJsonMergePatchNewtonsoftJson(outputTarget));

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("Morcatko.AspNetCore.JsonMergePatch")
            .AddUsing("System.Threading")
            .AddUsing("System.Threading.Tasks")
            .AddClass("JsonMergePatchExecutor", @class =>
            {
                @class
                    .AddGenericParameter("T", out var T)
                    .AddGenericTypeConstraint(T, constraint => constraint.AddType("class"));

                @class.ImplementsInterface($"{this.GetPatchExecutorInterfaceName()}<T>");

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

                @class.AddMethod("Task", "ApplyToAsync", method =>
                {
                    method.Async();
                    method.AddParameter(T, "target");
                    method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                    method.AddStatement("ArgumentNullException.ThrowIfNull(target);")
                        .AddStatement("_document.ApplyTo(target);", stmt => stmt.SeparatedFromPrevious());

                    if (HasFluentValidation())
                    {
                        method.AddStatement($"var validator = _validatorProvider.GetValidator<{T}>();", stmt => stmt.SeparatedFromPrevious())
                            .AddStatement("var validationResult = await validator.ValidateAsync(target, cancellationToken);", stmt => stmt.SeparatedFromPrevious());

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

    public override void AfterTemplateRegistration()
    {
        var templates = ExecutionContext.FindTemplateInstances<IAppStartupTemplate>(IAppStartupTemplate.RoleName);

        foreach (var template in templates)
        {
            template.CSharpFile.OnBuild(file =>
            {
                var startup = template.StartupFile;
                startup.ConfigureServices((statements, context) =>
                {
                    if (statements.FindStatement(s => s.HasMetadata("configure-services-controllers-generic")) is not CSharpInvocationStatement)
                    {
                        return;
                    }

                    template.AddUsing("Morcatko.AspNetCore.JsonMergePatch");

                    // Until we can make the "AddController" statement in the Intent.AspNetCore.Controllers be
                    // a CSharpInvocationStatement that supports method chaining, this will have to do.
                    // It's our original hack approach anyway and turning this into a CSharpMethodChainStatement will
                    // only make the CSharpInvocationStatement change later difficult. 
                    template.CSharpFile.AfterBuild(_ =>
                    {
                        var statementsToCheck = new List<CSharpStatement>();
                        ExtractPossibleStatements(statements, statementsToCheck);

                        var lastConfigStatement = (CSharpInvocationStatement)statementsToCheck.Last(p => p.HasMetadata("configure-services-controllers"));
                        var jsonmergeStatement =
                            statements.FindStatement(s => s.TryGetMetadata<string>("configure-services-controllers", out var v) && v == "json-merge")
                                as CSharpInvocationStatement;
                        if (jsonmergeStatement is null)
                        {
                            jsonmergeStatement = new CSharpInvocationStatement(".AddNewtonsoftJsonMergePatch");
                            jsonmergeStatement.AddMetadata("configure-services-controllers", "json-merge");
                            lastConfigStatement.InsertBelow(jsonmergeStatement);
                        }

                        lastConfigStatement.WithoutSemicolon();
                    });
                });
            }, 14);
        }
    }

    private static void ExtractPossibleStatements(IHasCSharpStatements targetBlock, List<CSharpStatement> statementsToCheck)
    {
        foreach (var statement in targetBlock.Statements)
        {
            if (statement is CSharpInvocationStatement)
            {
                statementsToCheck.Add(statement);
            }
            else if (statement is IHasCSharpStatements container)
            {
                foreach (var nested in container.Statements)
                {
                    statementsToCheck.Add(nested);
                }
            }
        }
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