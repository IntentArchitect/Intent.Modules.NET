using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.HotChocolate.GraphQL.Models;

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
            mappedPath: new[] { x.Name },
            description: x.Comment));
        MappedElement = operation.InternalElement;
        Description = operation.Comment;
        RequiresAuthorization = false;
        AuthorizationDetails = null;
    }
    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public bool RequiresAuthorization { get; }
    public IAuthorizationModel AuthorizationDetails { get; }
    public IEnumerable<IGraphQLParameterModel> Parameters { get; }
    public IElement MappedElement { get; }
    public string Description { get; }
}