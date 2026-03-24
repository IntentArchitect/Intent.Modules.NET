using System.Linq;
using Intent.Engine;
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

                        var commandParamIndex = method.Parameters.FindIndex(p =>
                            p.TryGetMetadata<IControllerParameterModel>("model", out var m) &&
                            m.TypeReference.Element?.SpecializationType == "DTO");

                        if (commandParamIndex < 0)
                        {
                            continue;
                        }

                        if (!method.Attributes.Any(a => a.Name.Contains("Consumes")))
                        {
                            method.AddAttribute("Consumes", attr => attr.AddArgument("JsonMergePatchDocument.ContentType"));
                        }

                        var commandTypeName = template.GetTypeName(payloadParam.TypeReference);

                        method.Parameters.RemoveAt(commandParamIndex);

                        method.InsertParameter(commandParamIndex,
                            $"JsonMergePatchDocument<{commandTypeName}>",
                            "mergePatchDocument",
                            param => param.AddAttribute("FromBody"));

                        
                    }
                }, 20);
            }
        }
    }
}