using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Controllers.JsonPatch.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.JsonPatch.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MediatRCommandExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Controllers.JsonPatch.MediatRCommandExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Command);
            foreach (var template in templates)
            {
                if (!template.TryGetModel<object>(out var model))
                {
                    continue;
                }

                if (model is not IElementWrapper modelElement)
                {
                    continue;
                }

                var element = modelElement.InternalElement;
                if (element is null || element.SpecializationType != "Command")
                {
                    continue;
                }

                if (!HttpEndpointModelFactory.TryGetEndpoint(element, null, false, out var endpoint)
                    || endpoint.Verb != HttpVerb.Patch)
                {
                    continue;
                }

                var routeParameterIds = endpoint.Inputs
                    .Where(i => i.Source == HttpInputSource.FromRoute)
                    .Select(i => i.Id)
                    .ToHashSet(System.StringComparer.OrdinalIgnoreCase);

                template.CSharpFile.OnBuild(file =>
                {
                    var cls = file.Classes.First(x => x.Interfaces.Any(y => y == "IRequest"));

                    if (!template.TryGetTypeName("Intent.Application.MediatR.FluentValidation.BypassPipelineValidationInterface",
                            out var bypassValidationInterface))
                    {
                        return;
                    }

                    cls.ImplementsInterface(bypassValidationInterface);

                    // Get the constructor
                    var ctor = cls.Constructors.FirstOrDefault();
                    if (ctor == null)
                    {
                        return;
                    }

                    // Remove all parameters that are NOT route parameters
                    var parametersToRemove = ctor.Parameters
                        .Where(param => param.TryGetMetadata<DTOFieldModel>("model", out var m) && !routeParameterIds.Contains(m.Id))
                        .ToList();

                    foreach (var param in parametersToRemove)
                    {
                        ctor.Parameters.Remove(param);
                    }

                    // Remove corresponding assignment statements from constructor body
                    if (ctor.Statements == null)
                    {
                        return;
                    }

                    var statementsToRemove = ctor.Statements
                        .Where(stmt => parametersToRemove.Any(param => stmt.ToString().Contains($"{param.Name.ToPascalCase()} = {param.Name}")))
                        .ToList();

                    foreach (var stmt in statementsToRemove)
                    {
                        ctor.Statements.Remove(stmt);
                    }

                    ctor.InsertParameter(routeParameterIds.Count, $"{template.GetPatchExecutorInterfaceName()}<{cls.Name}>", "patchExecutor",
                        param => param.IntroduceProperty());
                });
            }
        }
    }
}