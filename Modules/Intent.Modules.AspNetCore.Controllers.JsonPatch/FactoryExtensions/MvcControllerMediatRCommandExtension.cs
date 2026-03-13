using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Controllers.JsonPatch.Templates;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.JsonPatch.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MvcControllerMediatRCommandExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Controllers.JsonPatch.MvcControllerMediatRCommandExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateRoles.Distribution.WebApi.Controller);
            foreach (var template in templates)
            {
                if (!template.TryGetModel<IControllerModel>(out var controllerModel))
                {
                    continue;
                }

                template.CSharpFile.OnBuild(file =>
                {
                    file.AddUsing("Morcatko.AspNetCore.JsonMergePatch");

                    var cls = file.Classes.First();

                    foreach (var method in cls.Methods)
                    {
                        if (!method.TryGetMetadata<IControllerOperationModel>("model", out var operation))
                        {
                            continue;
                        }

                        if (operation.Verb != HttpVerb.Patch)
                        {
                            continue;
                        }

                        var commandParam = operation.Parameters.FirstOrDefault(p =>
                            p.TypeReference.Element.SpecializationType == "Command");

                        if (commandParam == null)
                        {
                            continue;
                        }

                        if (method.Parameters.Any(p => p.Type.Contains("JsonMergePatchDocument")))
                        {
                            continue;
                        }

                        var commandParamIndex = method.Parameters.FindIndex(p =>
                            p.TryGetMetadata<IControllerParameterModel>("model", out var m) &&
                            m.TypeReference.Element?.SpecializationType == "Command");

                        if (commandParamIndex < 0)
                        {
                            continue;
                        }

                        if (!method.Attributes.Any(a => a.Name.Contains("Consumes")))
                        {
                            method.AddAttribute("Consumes", attr => attr.AddArgument("JsonMergePatchDocument.ContentType"));
                        }

                        var commandTypeName = template.GetTypeName(commandParam.TypeReference);

                        method.Parameters.RemoveAt(commandParamIndex);

                        method.InsertParameter(commandParamIndex,
                            $"JsonMergePatchDocument<{commandTypeName}>",
                            "mergePatchDocument",
                            param => param.AddAttribute("FromBody"));

                        var targetStatementIndex = GetIndexOf(method, "_mediator.Send");
                        if (targetStatementIndex < 0)
                        {
                            continue;
                        }

                        if (template.TryGetTypeName("Application.Common.ValidatorProviderInterface", out var fluentValidatorProviderName))
                        {
                            var ctor = cls.Constructors.First();
                            if (ctor.Parameters.All(x => x.Type != "IValidatorProvider"))
                            {
                                ctor.AddParameter(fluentValidatorProviderName, "validatorProvider", param =>
                                {
                                    param.IntroduceReadonlyField((_, stmt) => stmt.ThrowArgumentNullException());
                                });
                            }
                        }

                        RemoveStatementsUptoIndexOf(targetStatementIndex, method);

                        var commandType = template.GetTypeName(TemplateRoles.Application.Command, commandParam.TypeReference.Element)!;

                        method.InsertStatements(0,
                        [
                            new CSharpIfStatement("mergePatchDocument == null").AddReturn(@"BadRequest(""Merge patch document cannot be null"")"),
                            GetJsonMergePatchExecutorStatement(template, commandType, fluentValidatorProviderName != null),
                            GetCommandInstantiationStatement(
                                commandType: commandType,
                                routeParameters: operation.Parameters.Where(x => x.Source == HttpInputSource.FromRoute).ToArray())
                        ]);
                    }
                }, 20);
            }
        }

        private static CSharpAssignmentStatement GetJsonMergePatchExecutorStatement(
            ICSharpFileBuilderTemplate template,
            string commandType,
            bool addValidatorProvider)
        {
            var instantiation = new CSharpInvocationStatement($"new {template.GetJsonMergePatchExecutorName()}<{commandType}>");
            instantiation.AddArgument("mergePatchDocument");
            if (addValidatorProvider)
            {
                instantiation.AddArgument("_validatorProvider");
            }
            return new CSharpAssignmentStatement(new CSharpVariableDeclaration("patchExecutor"), instantiation);
        }

        private static CSharpAssignmentStatement GetCommandInstantiationStatement(string commandType, IReadOnlyList<IControllerParameterModel> routeParameters)
        {
            var instantiation = new CSharpInvocationStatement($"new {commandType}");
            foreach (var routeParameter in routeParameters)
            {
                instantiation.AddArgument(routeParameter.Name);
            }

            instantiation.AddArgument("patchExecutor");
            return new CSharpAssignmentStatement(new CSharpVariableDeclaration("command"), instantiation);
        }

        private static void RemoveStatementsUptoIndexOf(int targetStatementIndex, CSharpClassMethod method)
        {
            for (var index = targetStatementIndex - 1; index > -1; index--)
            {
                method.Statements.RemoveAt(index);
            }
        }

        private static int GetIndexOf(CSharpClassMethod method, string statementStr)
        {
            for (var statementIndex = 0; statementIndex <= method.Statements.Count - 1; statementIndex++)
            {
                var statement = method.Statements[statementIndex];
                if (statement.GetText("").Contains(statementStr))
                {
                    return statementIndex;
                }
            }

            return -1;
        }
    }
}