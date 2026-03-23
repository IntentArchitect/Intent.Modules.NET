using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Controllers.JsonPatch.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.JsonPatch.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TraditionalServiceDtoExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Controllers.JsonPatch.TraditionalServiceDtoExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var services = application.MetadataManager.Services(application.Id).GetServiceModels();
            var operationsWithPatch = services
                .SelectMany(service => service.Operations)
                .Where(operation => HttpEndpointModelFactory.TryGetEndpoint(operation.InternalElement, null, false, out var endpoint) &&
                             endpoint.Verb == HttpVerb.Patch)
                .ToArray();
            foreach (var operationModel in operationsWithPatch)
            {
                var payloads = operationModel.Parameters
                    .Where(x => x.TypeReference.Element?.IsDTOModel() == true)
                    .Select(x => x.TypeReference.Element.AsDTOModel())
                    .ToArray();
                if (payloads.Length != 1)
                {
                    throw new ElementException(operationModel.InternalElement, $"Patch operation must have only one body payload parameter");
                }

                var payloadDto = payloads.First();
                var dtoTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Contracts.Dto, payloadDto);
                if (dtoTemplate is null)
                {
                    continue;
                }

                dtoTemplate.CSharpFile.OnBuild(file =>
                {
                    var classType = file.TypeDeclarations.First();
                    classType.AddProperty($"{dtoTemplate.GetPatchExecutorInterfaceName()}<{classType.Name}>", "PatchExecutor", prop =>
                    {
                        prop.Init();
                        prop.Required();
                    });
                });
            }
        }
    }
}