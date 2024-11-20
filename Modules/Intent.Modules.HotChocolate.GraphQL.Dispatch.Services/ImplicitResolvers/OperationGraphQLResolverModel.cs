#nullable enable
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.HotChocolate.GraphQL.Models;
using Intent.Modules.Metadata.Security.Models;

namespace Intent.Modules.HotChocolate.GraphQL.Dispatch.Services.ImplicitResolvers;

public class OperationGraphQLResolverModel : IGraphQLResolverModel
{
    public OperationGraphQLResolverModel(OperationModel operation)
    {
        Name = operation.Name;
        TypeReference = operation.TypeReference;
        Parameters = operation.Parameters.Select(x => new GraphQLParameterModel(
            name: x.Name.ToCamelCase(),
            typeReference: x.TypeReference,
            mappedElement: x.InternalElement,
            mappedPath: [x.Name],
            description: x.Comment));
        MappedElement = operation.InternalElement;
        Description = operation.Comment;
        SecurityModels = null;
    }
    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public IReadOnlyCollection<ISecurityModel>? SecurityModels { get; }
    public IEnumerable<IGraphQLParameterModel> Parameters { get; }
    public IElement? MappedElement { get; }
    public string Description { get; }
}