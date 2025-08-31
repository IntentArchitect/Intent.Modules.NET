using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Aws.Lambda.Functions.Api;

public class TraditionalServiceLambdaFunctionContainerModel : ILambdaFunctionContainerModel
{
    public TraditionalServiceLambdaFunctionContainerModel(ServiceModel service, ISoftwareFactoryExecutionContext context)
    {
        Id = service.Id;
        Name = service.Name;
        TypeReference = service.InternalElement.TypeReference;
        Folder = service.Folder;
        Stereotypes = service.Stereotypes;
        InternalElement = service.InternalElement;
        Endpoints = service.Operations
            .Select(op => new TraditionalServiceLambdaFunctionModel(this, op, context))
            .ToList();
    }

    public string Id { get; }
    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public IFolder Folder { get; }
    public IEnumerable<IStereotype> Stereotypes { get; }
    public IElement? InternalElement { get; }
    public IReadOnlyCollection<ILambdaFunctionModel> Endpoints { get; }
}