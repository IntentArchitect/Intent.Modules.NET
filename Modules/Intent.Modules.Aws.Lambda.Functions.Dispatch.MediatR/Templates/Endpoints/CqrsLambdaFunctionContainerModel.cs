using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Aws.Lambda.Functions.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Aws.Lambda.Functions.Dispatch.MediatR.Templates.Endpoints;

public class CqrsLambdaFunctionContainerModel : ILambdaFunctionContainerModel
{
    public CqrsLambdaFunctionContainerModel(
        IElement? parentElement,
        IEnumerable<IElement> elements, 
        ISoftwareFactoryExecutionContext context)
    {
        Id = parentElement?.Id ?? Guid.Empty.ToString();
        Name = parentElement is not null
            ? string.Join(string.Empty,
                parentElement.GetParentPath()
                    .Append(parentElement)
                    .Select(s => s.Name?.Replace(".", "_").ToPascalCase() ?? string.Empty))
            : "Default";
        Folder = parentElement?.ParentElement?.AsFolderModel();
        InternalElement = parentElement;
        Endpoints = elements
            .Select(element => new CqrsLambdaFunctionModel(
                container: this,
                endpointElement: element,
                context: context))
            .ToArray();
    }

    public string Id { get; }
    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public IFolder Folder { get; }
    public IEnumerable<IStereotype> Stereotypes { get; }
    public IElement? InternalElement { get; }
    public IReadOnlyCollection<ILambdaFunctionModel> Endpoints { get; }
}