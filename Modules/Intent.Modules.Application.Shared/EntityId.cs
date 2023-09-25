using Intent.Modelers.Domain.Api;

namespace Intent.Modules.Application.Shared;

// This is duplicated in Intent.Modules.Application.MediatR.CRUD
// Once we go through the Intent 4.1 we will need to upgrade and reconcile.
public interface IEntityId
{
    string IdName { get; }
    string Type { get; }
    AttributeModel Attribute { get; }
}

// This is duplicated in Intent.Modules.Application.MediatR.CRUD
// Once we go through the Intent 4.1 we will need to upgrade and reconcile.
public record EntityIdAttribute(string IdName, string Type, AttributeModel Attribute) : IEntityId;

// This is duplicated in Intent.Modules.Application.MediatR.CRUD
// Once we go through the Intent 4.1 we will need to upgrade and reconcile.
public record EntityNestedCompositionalIdAttribute(string IdName, string Type, AttributeModel Attribute) : IEntityId;