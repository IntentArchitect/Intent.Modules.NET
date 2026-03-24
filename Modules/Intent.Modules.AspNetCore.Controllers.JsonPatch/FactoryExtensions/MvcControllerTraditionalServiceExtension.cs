using System.Linq;
using Intent.Engine;
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
    public class MvcControllerTraditionalServiceExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Controllers.JsonPatch.MvcControllerTraditionalServiceExtension";

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

                        if (operation.InternalElement.SpecializationType != "Operation")
                        {
                            continue;
                        }

                        var payloadParam = operation.Parameters.FirstOrDefault(p =>
                            p.TypeReference.Element.SpecializationType == "DTO");

                        if (payloadParam == null)
                        {
                            continue;
                        }

                        if (method.Parameters.Any(p => p.Type.Contains("JsonMergePatchDocument")))
                        {
                            continue;
                        }

                        var payloadParamIndex = method.Parameters.FindIndex(p =>
                            p.TryGetMetadata<IControllerParameterModel>("model", out var m) &&
                            m.TypeReference.Element?.SpecializationType == "DTO");

                        if (payloadParamIndex < 0)
                        {
                            continue;
                        }

                        if (!method.Attributes.Any(a => a.Name.Contains("Consumes")))
                        {
                            method.AddAttribute("Consumes", attr => attr.AddArgument("JsonMergePatchDocument.ContentType"));
                        }

                        var payloadTypeName = template.GetTypeName(payloadParam.TypeReference)!;

                        method.Parameters.RemoveAt(payloadParamIndex);

                        method.InsertParameter(payloadParamIndex,
                            $"JsonMergePatchDocument<{payloadTypeName}>",
                            "mergePatchDocument",
                            param => param.AddAttribute("FromBody"));

                        var validationStatement = method.Statements.FirstOrDefault(x => x.GetText("").Contains("_validationService.Handle"));
                        if (validationStatement is not null)
                        {
                            method.RemoveStatement(validationStatement);
                        }

                        method.InsertStatements(0,
                        [
                            new CSharpIfStatement("mergePatchDocument == null")
                                .AddStatement("return BadRequest(\"Merge patch document cannot be null\");"),
                            GetJsonMergePatchExecutorStatement(template, payloadTypeName, fluentValidatorProviderName != null)
                                .SeparatedFromPrevious(),
                            new CSharpAssignmentStatement(
                                lhs: new CSharpVariableDeclaration(payloadParam.Name),
                                rhs: new CSharpObjectInitializerBlock($"new {payloadTypeName}")
                                    .AddInitStatement("PatchExecutor", "patchExecutor"))
                                .WithSemicolon()
                                .SeparatedFromPrevious()
                                .SeparatedFromNext()
                        ]);
                    }
                }, 20);
            }
        }
        
        private static CSharpAssignmentStatement GetJsonMergePatchExecutorStatement(
            ICSharpFileBuilderTemplate template,
            string payloadType,
            bool addValidatorProvider)
        {
            var instantiation = new CSharpInvocationStatement($"new {template.GetJsonMergePatchExecutorName()}<{payloadType}>");
            instantiation.AddArgument("mergePatchDocument");
            if (addValidatorProvider)
            {
                instantiation.AddArgument("_validatorProvider");
            }
            return new CSharpAssignmentStatement(new CSharpVariableDeclaration("patchExecutor"), instantiation);
        }
    }
}